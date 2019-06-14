//******************************************************************************************************
//  GrafanaAuthProxyController.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/02/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Security;
using GSF.Security.Cryptography;
using GSF.Security.Model;
using GSF.Threading;
using Newtonsoft.Json.Linq;
using CancellationToken = System.Threading.CancellationToken;
using Http = System.Net.WebRequestMethods.Http;

#pragma warning disable 169, 414, 649
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming
namespace openHistorian.Adapters
{
    /// <summary>
    /// Creates a reverse proxy to a hosted Grafana instance that handles integrated authentication.
    /// </summary>
    public class GrafanaAuthProxyController : ApiController
    {
        #region [ Members ]

        // Nested Types
        private class UserDetail
        {
            public int id;
            public string email;
            public string name;
            public string login;
            public string theme;
            public int orgId;
            public bool isGrafanaAdmin;
        }

        private class OrgUserDetail
        {
            public int orgId;
            public int userId;
            public string email;
            public string login;
            public string role;
        }

        // Constants

        /// <summary>
        /// Defines the default installation server path for Grafana.
        /// </summary>
        public const string DefaultServerPath = "Grafana\\bin\\grafana-server.exe";

        /// <summary>
        /// Default URL for the hosted Grafana process.
        /// </summary>
        public const string DefaultHostedURL = "http://localhost:8185";

        /// <summary>
        /// Default timeout, in seconds, for system initialization.
        /// </summary>
        public const int DefaultInitializationTimeout = 10;

        /// <summary>
        /// Default Grafana organization ID.
        /// </summary>
        public const int DefaultOrganizationID = 1;

        /// <summary>
        /// Default cookie name for last visited Grafana dashboard.
        /// </summary>
        public const string DefaultLastDashboardCookieName = "x-last-dashboard";

        /// <summary>
        /// Grafana admin role name.
        /// </summary>
        public const string GrafanaAdminRoleName = "GrafanaAdmin";

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Handle proxy of the root Grafana URL, e.g., http://localhost:8180/grafana.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Proxied response.</returns>
        [AcceptVerbs(Http.Get, Http.Head, Http.Post, Http.Put, Http.MkCol), HttpDelete, HttpPatch]
        [SuppressMessage("Security", "SG0016", Justification = "CSRF vulnerability handled by Grafana")]
        public async Task<HttpResponseMessage> ProxyRoot(CancellationToken cancellationToken)
        {
            return await ProxyPage("", cancellationToken);
        }

        /// <summary>
        /// Handle proxy of specified Grafana URL.
        /// </summary>
        /// <param name="url">URL to proxy.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Proxied response.</returns>
        [AcceptVerbs(Http.Get, Http.Head, Http.Post, Http.Put, Http.MkCol), HttpDelete, HttpPatch]
        [SuppressMessage("Security", "SG0016", Justification = "CSRF vulnerability handled by Grafana")]
        public async Task<HttpResponseMessage> ProxyPage(string url, CancellationToken cancellationToken)
        {
            // Handle special URL commands
            switch (url.ToLowerInvariant())
            {
                case "syncusers":
                    return HandleSynchronizeUsersRequest(Request, RequestContext.Principal as SecurityPrincipal);
                case "servertime":
                    return HandleServerTimeRequest(Request);
                case "logout":
                    return HandleGrafanaLogoutRequest(Request);
                case "api/login/ping":
                    return HandleGrafanaLoginPingRequest(Request, RequestContext.Principal as SecurityPrincipal);
            }

            if (url.StartsWith("avatar/", StringComparison.OrdinalIgnoreCase))
                return HandleGrafanaAvatarRequest(Request);

            // Proxy all other requests
            SecurityPrincipal securityPrincipal = RequestContext.Principal as SecurityPrincipal;

            if ((object)securityPrincipal == null || (object)securityPrincipal.Identity == null)
                throw new SecurityException($"User \"{RequestContext.Principal?.Identity.Name}\" is unauthorized.");

            Request.Headers.Add(s_authProxyHeaderName, securityPrincipal.Identity.Name);
            Request.RequestUri = new Uri($"{s_baseUrl}/{url}{Request.RequestUri.Query}");

            if (Request.Method == HttpMethod.Get)
                Request.Content = null;
            
            HttpResponseMessage response = await s_http.SendAsync(Request, cancellationToken);
            HttpStatusCode statusCode = response.StatusCode;

            if (statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.Unauthorized)
            {
                // HACK: Internet Explorer sometimes applies cached authorization headers to concurrent AJAX requests
                if ((object)Request.Headers.Authorization != null)
                {
                    // Clone request to allow modification
                    HttpRequestMessage retryRequest = await CloneRequest();
                    retryRequest.Headers.Authorization = null;

                    HttpResponseMessage retryResponse = await s_http.SendAsync(retryRequest, cancellationToken);
                    HttpStatusCode retryStatusCode = retryResponse.StatusCode;

                    if (retryStatusCode != HttpStatusCode.NotFound && retryStatusCode != HttpStatusCode.Unauthorized)
                    {
                        response = retryResponse;
                        statusCode = retryStatusCode;
                    }
                }
            }

            if (statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.Unauthorized)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    string userName = (state as SecurityPrincipal)?.Identity?.Name;

                    if (string.IsNullOrEmpty(userName))
                        return;

                    try
                    {
                        // Validate user has a role defined in latest security context
                        Dictionary<string, string[]> securityContext = s_latestSecurityContext;

                        if ((object)securityContext == null)
                            return;

                        Dictionary<string, string[]> userRoles = StartUserSynchronization(userName);
                        string newUserMessage = securityContext.ContainsKey(userName) ? "" : $"New user \"{userName}\" encountered. ";

                        OnStatusMessage($"{newUserMessage}Security context with {userRoles.Count} users and associated roles queued for Grafana user synchronization.");
                    }
                    catch (Exception ex)
                    {
                        OnStatusMessage($"ERROR: Failed while queuing Grafana user synchronization for new user \"{userName}\": {ex.Message}");
                    }
                },
                RequestContext.Principal);
            }

            // Always keep last visited Grafana dashboard in a client session cookie
            if (url.StartsWith("api/dashboards/", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Request.Headers?.Referrer.AbsolutePath))
                response.Headers.AddCookies(new[] { new CookieHeaderValue(s_lastDashboardCookieName, $"{Request.Headers.Referrer.AbsolutePath}") { Path = "/" } });

            return response;
        }

        private async Task<HttpRequestMessage> CloneRequest()
        {
            HttpRequestMessage clone = new HttpRequestMessage(Request.Method, Request.RequestUri)
            {
                Version = Request.Version
            };

            foreach (KeyValuePair<string, object> property in Request.Properties)
                clone.Properties.Add(property);

            foreach (KeyValuePair<string, IEnumerable<string>> header in Request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            if (Request.Content != null)
            {
                MemoryStream content = new MemoryStream();
                
                await Request.Content.CopyToAsync(content);
                content.Position = 0;
                
                clone.Content = new StreamContent(content);

                if (Request.Content.Headers != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> header in Request.Content.Headers)
                        clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }

        #endregion

        #region [ Static ]

        // Static Events

        /// <summary>
        /// Raised when the Grafana authentication proxy reports an important status message.
        /// </summary>
        public static event EventHandler<EventArgs<string>> StatusMessage;

        // Static Fields
        private static readonly HttpClient s_http;
        private static readonly string s_baseUrl;
        private static readonly string s_authProxyHeaderName;
        private static readonly int s_initializationTimeout;
        private static readonly string s_adminUser;
        private static readonly string s_logoutResource;
        private static readonly string s_avatarResource;
        private static readonly int s_organizationID;
        private static readonly string s_lastDashboardCookieName;
        private static readonly ShortSynchronizedOperation s_synchronizeUsers;
        private static readonly ManualResetEventSlim s_initializationWaitHandle;
        private static Dictionary<string, string[]> s_lastSecurityContext;
        private static Dictionary<string, string[]> s_latestSecurityContext;
        private static bool s_manualSynchronization;

        // Static Constructor
        static GrafanaAuthProxyController()
        {
            // Create a shared HTTP client instance
            s_http = new HttpClient(new HttpClientHandler { UseCookies = false });

            // Make sure openHistorian specific default service settings exist
            CategorizedSettingsElementCollection grafanaHosting = ConfigurationFile.Current.Settings["grafanaHosting"];

            // Make sure needed settings exist
            grafanaHosting.Add("ServerPath", DefaultServerPath, "Defines the path to the Grafana server to host - set to empty string to disable hosting.");
            grafanaHosting.Add("AuthProxyHeaderName", "X-WEBAUTH-USER", "Defines the authorization header name used for Grafana user authentication.");
            grafanaHosting.Add("InitializationTimeout", DefaultInitializationTimeout, "Defines the timeout, in seconds, for the Grafana system to initialize.");
            grafanaHosting.Add("AdminUser", "admin", "Defines the Grafana Administrator user required for managing configuration.");
            grafanaHosting.Add("LogoutResource", "Logout.cshtml", "Defines the relative URL to the logout page to use for Grafana users.");
            grafanaHosting.Add("AvatarResource", "Images/Icons/openHistorian.png", "Defines the relative URL to the 40x40px avatar image to use for Grafana users.");
            grafanaHosting.Add("HostedURL", DefaultHostedURL, "Defines the local URL to the hosted Grafana server instance. Setting is for internal use, external access to Grafana instance will be proxied via WebHostURL.");
            grafanaHosting.Add("OrganizationID", DefaultOrganizationID, "Defines the database ID of the target organization used for user synchronization.");
            grafanaHosting.Add("LastDashboardCookieName", DefaultLastDashboardCookieName, "Defines the session cookie name used to save the last visited Grafana dashboard.");

            // Get settings as currently defined in configuration file
            s_authProxyHeaderName = grafanaHosting["AuthProxyHeaderName"].Value;
            s_initializationTimeout = grafanaHosting["InitializationTimeout"].ValueAs(DefaultInitializationTimeout) * 1000;
            s_adminUser = grafanaHosting["AdminUser"].Value;
            s_logoutResource = grafanaHosting["LogoutResource"].Value;
            s_avatarResource = grafanaHosting["AvatarResource"].Value;
            s_baseUrl = grafanaHosting["HostedURL"].Value;
            s_organizationID = grafanaHosting["OrganizationID"].ValueAs(DefaultOrganizationID);
            s_lastDashboardCookieName = grafanaHosting["LastDashboardCookieName"].ValueAs(DefaultLastDashboardCookieName);

            if (File.Exists(grafanaHosting["ServerPath"].ValueAs(DefaultServerPath)))
            {
                s_initializationWaitHandle = new ManualResetEventSlim();

                // Establish a synchronized operation for handling Grafana user synchronizations
                s_synchronizeUsers = new ShortSynchronizedOperation(SynchronizeUsers, ex => {
                    AggregateException aggregate = ex as AggregateException;

                    if ((object)aggregate != null)
                    {
                        foreach (Exception innerException in aggregate.Flatten().InnerExceptions)
                            OnStatusMessage($"ERROR: {innerException.Message}");
                    }                    
                    else
                    {
                        OnStatusMessage($"ERROR: {ex.Message}");
                    }
                });

                // Attach to event for notifications of when security context has been refreshed
                AdoSecurityProvider.SecurityContextRefreshed += AdoSecurityProvider_SecurityContextRefreshed;
            }
        }

        // Static Methods

        /// <summary>
        /// Signal authentication proxy that initialization is complete.
        /// </summary>
        public static void InitializationComplete()
        {
            s_initializationWaitHandle.Set();
        }

        /// <summary>
        /// Gets a flag that determines if configured Grafana server is responding.
        /// </summary>
        /// <returns><c>true</c> if Grafana server is responding; otherwise, <c>false</c>.</returns>
        public static bool ServerIsResponding()
        {
            try
            {
                // Test server response by hitting root page
                dynamic result = CallAPIFunction(HttpMethod.Get, s_baseUrl).Result;
                return (object)result != null;
            }
            catch
            {
                return false;
            }
        }

        private static void OnStatusMessage(string status)
        {
            StatusMessage?.Invoke(typeof(GrafanaAuthProxyController), new EventArgs<string>(status));
        }

        private static void AdoSecurityProvider_SecurityContextRefreshed(object sender, EventArgs<Dictionary<string, string[]>> e)
        {
            Interlocked.Exchange(ref s_latestSecurityContext, e.Argument);
            s_synchronizeUsers?.RunOnceAsync();
        }

        private static void SynchronizeUsers()
        {
            Dictionary<string, string[]> securityContext = s_latestSecurityContext;

            if ((object)securityContext == null)
                return;

            // Skip user synchronization if security context has not changed
            if (!s_manualSynchronization && SecurityContextsAreEqual(securityContext, s_lastSecurityContext))
                return;

            s_manualSynchronization = false;
            Interlocked.Exchange(ref s_lastSecurityContext, securityContext);

            // Give initialization - which includes starting Grafana server process - a chance to complete
            s_initializationWaitHandle.Wait(s_initializationTimeout);

            // Lookup Grafana Administrative user
            if (!LookupUser(s_adminUser, out UserDetail userDetail, out string message))
            {
                OnStatusMessage($"WARNING: Failed to synchronize Grafana users, cannot find Grafana Administrator \"{s_adminUser}\": {message}");
                return;
            }
            
            // Get user list for target organization
            OrgUserDetail[] organizationUsers = GetOrganizationUsers(s_organizationID, out message);

            if (!string.IsNullOrEmpty(message))
                OnStatusMessage($"Issue retrieving user list for default organization: {message}");

            // Make sure Grafana Administrator has an admin role in the default organization
            bool success = organizationUsers.Any(user => user.userId == userDetail.id) ? 
                UpdateUserOrganizationalRole(s_organizationID, userDetail.id, "Admin", out message) : 
                AddUserToOrganization(s_organizationID, s_adminUser, "Admin", out message);
            
            if (!success)
                OnStatusMessage($"Issue validating organizational admin role for Grafana Administrator \"{s_adminUser}\" - Grafana user synchronization may not succeed: {message}");

            foreach (KeyValuePair<string, string[]> item in securityContext)
            {
                string userName = item.Key;
                string[] roles = item.Value;
                bool createdUser = false;

                if (userName.Equals("_logout", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Check if user exists
                if (!LookupUser(userName, out userDetail, out message))
                {
                    createdUser = CreateUser(userName, s_organizationID, out userDetail, out message);
                    OnStatusMessage($"Encountered new user \"{userName}\": {message}");
                }

                if (userDetail.id == 0)
                    continue;

                // Update user's Grafana admin role status if needed
                bool userIsGrafanaAdmin = UserIsGrafanaAdmin(roles);

                if (userDetail.isGrafanaAdmin != userIsGrafanaAdmin)
                {
                    JObject content = JObject.FromObject(new
                    {
                        isGrafanaAdmin = userIsGrafanaAdmin
                    });

                    message = CallAPIFunction(HttpMethod.Put, $"{s_baseUrl}/api/admin/users/{userDetail.id}/permissions", content.ToString()).Result.message;

                    if (!message.Equals("User permissions updated", StringComparison.OrdinalIgnoreCase))
                        OnStatusMessage($"Issue updating permissions for user \"{userName}\": {message}");
                }

                // Attempt to lookup user in default organization
                OrgUserDetail orgUserDetail = organizationUsers.FirstOrDefault(user => user.userId == userDetail.id);

                // Get user's organizational role: Admin / Editor / Viewer
                string organizationalRole = TranslateRole(roles);

                // Update user's organizational status / role as needed
                if ((object)orgUserDetail == null && !createdUser)
                    success = AddUserToOrganization(s_organizationID, userName, organizationalRole, out message);
                else if (createdUser || !orgUserDetail.role.Equals(organizationalRole, StringComparison.OrdinalIgnoreCase))
                    success = UpdateUserOrganizationalRole(s_organizationID, userDetail.id, organizationalRole, out message);
                else
                    success = true;

                if (!success)
                    OnStatusMessage($"Issue assigning organizational role \"{organizationalRole}\" for user \"{userName}\": {message}");
            }

            OnStatusMessage($"Synchronized security context with {securityContext.Count} users to Grafana.");
        }

        private static bool LookupUser(string userName, out UserDetail userDetail, out string message)
        {
            JObject result = CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}/api/users/lookup?loginOrEmail={userName}").Result;
            message = null;

            if (result.TryGetValue("id", out JToken token))
            {
                try
                {
                    userDetail = result.ToObject<UserDetail>();
                    return true;
                }
                catch (Exception ex)
                {
                    userDetail = null;
                    message = ex.Message;
                    return false;
                }
            }

            userDetail = null;

            if (result.TryGetValue("message", out token))
                message = token.Value<string>();

            return false;
        }

        private static bool CreateUser(string userName, int orgID, out UserDetail userDetail, out string message)
        {
            // Create a new user
            JObject content = JObject.FromObject(new
            {
                name = userName,
                email = "",
                login = userName,
                password = PasswordGenerator.Default.GeneratePassword()
            });

            userDetail = new UserDetail
            {
                name = userName,
                login = userName,
                orgId = orgID
            };

            dynamic result = CallAPIFunction(HttpMethod.Post, $"{s_baseUrl}/api/admin/users", content.ToString()).Result;

            message = result.message;

            if (result.id == null)
                return false;

            userDetail.id = (int)result.id;
            return true;
        }

        private static OrgUserDetail[] GetOrganizationUsers(int orgID, out string message)
        {
            try
            {
                message = null;
                return ((JArray)CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}/api/orgs/{orgID}/users", responseIsArray: true).Result).ToObject<OrgUserDetail[]>();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new OrgUserDetail[0];
            }
        }

        private static bool UpdateUserOrganizationalRole(int orgID, int userID, string organizationalRole, out string message)
        {
            JObject content = JObject.FromObject(new
            {
                role = organizationalRole
            });

            message = CallAPIFunction(new HttpMethod("PATCH"), $"{s_baseUrl}/api/orgs/{orgID}/users/{userID}", content.ToString()).Result.message;

            return message.Equals("Organization user updated", StringComparison.OrdinalIgnoreCase);
        }

        private static bool AddUserToOrganization(int orgID, string userName, string organizationalRole, out string message)
        {
            JObject content = JObject.FromObject(new
            {
                loginOrEmail = userName,
                role = organizationalRole
            });

            message = CallAPIFunction(HttpMethod.Post, $"{s_baseUrl}/api/orgs/{orgID}/users", content.ToString()).Result.message;

            return message.Equals("User added to organization", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<dynamic> CallAPIFunction(HttpMethod method, string url, string content = null, bool responseIsArray = false)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add(s_authProxyHeaderName, s_adminUser);

            if ((object)content != null)
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await s_http.SendAsync(request);

            content = await response.Content.ReadAsStringAsync();

            if (responseIsArray)
                return JArray.Parse(content);
                
            return JObject.Parse(content);
        }

        private static bool SecurityContextsAreEqual(Dictionary<string, string[]> left, Dictionary<string, string[]> right)
        {
            if (left == right)
                return true;

            if ((object)left == null || (object)right == null)
                return false;

            if (left.Count != right.Count)
                return false;

            foreach (KeyValuePair<string, string[]> item in left)
            {
                if (!right.TryGetValue(item.Key, out string[] value))
                    return false;

                if (item.Value.CompareTo(value) != 0)
                    return false;
            }

            return true;
        }

        private static bool UserIsGrafanaAdmin(string[] roles)
        {
            return roles.Any(role => role.Equals(GrafanaAdminRoleName, StringComparison.OrdinalIgnoreCase));
        }

        private static string TranslateRole(string[] roles)
        {
            if (roles.Any(role => role.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
                return "Admin";

            if (roles.Any(role => role.Equals("Editor", StringComparison.OrdinalIgnoreCase)))
                return "Editor";

            return "Viewer";
        }

        private static Dictionary<string, string[]> StartUserSynchronization(string currentUserName)
        {
            Dictionary<string, string[]> userRoles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            using (UserRoleCache userRoleCache = UserRoleCache.GetCurrentCache())
            {
                TableOperations<UserAccount> userAccountTable = new TableOperations<UserAccount>(connection);
                string[] roles;

                foreach (UserAccount user in userAccountTable.QueryRecords())
                {
                    string userName = user.AccountName;

                    if (userRoleCache.TryGetUserRole(userName, out roles))
                        userRoles[userName] = roles;
                }

                // Also make sure current user is added since user may have implicit rights based on group
                if (!string.IsNullOrEmpty(currentUserName))
                {
                    if (!userRoles.ContainsKey(currentUserName) && userRoleCache.TryGetUserRole(currentUserName, out roles))
                        userRoles[currentUserName] = roles;
                }

                if (userRoles.Count > 0)
                {
                    Interlocked.Exchange(ref s_latestSecurityContext, userRoles);
                    s_manualSynchronization = true;
                    s_synchronizeUsers.RunOnceAsync();
                }
            }

            return userRoles;
        }

        private static HttpResponseMessage HandleSynchronizeUsersRequest(HttpRequestMessage request, SecurityPrincipal securityPrincipal)
        {
            Dictionary<string, string[]> userRoles = StartUserSynchronization(securityPrincipal?.Identity?.Name);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StringContent($@"
                    <html>
                    <head>
                        <title>Grafana User Synchronization</title>
                        <link rel=""shortcut icon"" href=""@GSF/Web/Shared/Images/Icons/favicon.ico"" />
                    </head>
                    <body>
                        Security context with {userRoles.Count} users and associated roles queued for Grafana user synchronization.
                    </body>
                    </html>
                ",
                Encoding.UTF8, "text/html")
            };
        }

        private static HttpResponseMessage HandleServerTimeRequest(HttpRequestMessage request)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StringContent(JObject.FromObject(new
                {
                    serverTime = $"{DateTime.UtcNow:MM/dd/yyyy HH:mm:ss.fff} UTC"
                })
                .ToString(), Encoding.UTF8, "application/json")
            };
        }

        private static HttpResponseMessage HandleGrafanaLogoutRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Redirect) { RequestMessage = request };
            Uri requestUri = request.RequestUri, referrerUri = request.Headers.Referrer;

            if (referrerUri.AbsolutePath.ToLowerInvariant().Contains("/grafana/"))
            {
                // Handle user requested logout
                response.Headers.Location = new Uri($"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}/{s_logoutResource}");
            }
            else
            {
                // Handle automated logout, returning to original Grafana page
                string lastDashboard = null;

                if (!string.IsNullOrWhiteSpace(referrerUri.Query))
                {
                    Dictionary<string, string> parameters = referrerUri.ParseQueryString().ToDictionary();

                    if (parameters.TryGetValue("referrer", out string referrer))
                    {
                        string base64Path = WebUtility.UrlDecode(referrer);
                        byte[] pathBytes = Convert.FromBase64String(base64Path);
                        referrerUri = new Uri(Encoding.UTF8.GetString(pathBytes));
                        lastDashboard = referrerUri.PathAndQuery;
                    }
                }

                if (string.IsNullOrWhiteSpace(lastDashboard))
                {
                    // Without knowing the original referrer, the best we can do is restore last screen only
                    CookieHeaderValue lastDashboardCookie = request.Headers.GetCookies(s_lastDashboardCookieName).FirstOrDefault();            
                    lastDashboard = lastDashboardCookie?[s_lastDashboardCookieName].Value;
                }

                if (string.IsNullOrWhiteSpace(lastDashboard))
                {
                    // As a last resort (i.e., no cookie or original referrer), return to the Grafana home page
                    response.Headers.Location = new Uri($"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}/grafana");
                }
                else
                {
                    if (!lastDashboard.Contains("?"))
                        lastDashboard += $"?orgId={s_organizationID}";

                    response.Headers.Location = new Uri($"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}{lastDashboard}");
                    OnStatusMessage($"Reloading previous Grafana dashboard: {lastDashboard}");
                }
            }

            return response;
        }

        private static HttpResponseMessage HandleGrafanaLoginPingRequest(HttpRequestMessage request, SecurityPrincipal securityPrincipal)
        {
            string userLoginState = securityPrincipal?.Identity == null ? "Unauthorized" : "Logged in";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StringContent(JObject.FromObject(new
                {
                    message = userLoginState
                })
                .ToString(), Encoding.UTF8, "application/json")
            };
        }

        private static HttpResponseMessage HandleGrafanaAvatarRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.MovedPermanently) { RequestMessage = request };
            Uri uri = request.RequestUri;

            response.Headers.Location = new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}/{s_avatarResource}");

            return response;
        }

        #endregion
    }
}