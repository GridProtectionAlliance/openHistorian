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

namespace openHistorian.Adapters
{
    /// <summary>
    /// Creates a reverse proxy to a hosted Grafana instance that handles integrated authentication.
    /// </summary>
    public class GrafanaAuthProxyController : ApiController
    {
        #region [ Members ]

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
        public const int DefaultInitializationTimeout = 15;

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
        [AcceptVerbs(Http.Get, Http.Head, Http.Post, Http.Put, Http.MkCol)]
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
        [AcceptVerbs(Http.Get, Http.Head, Http.Post, Http.Put, Http.MkCol)]
        public async Task<HttpResponseMessage> ProxyPage(string url, CancellationToken cancellationToken)
        {
            using (HttpClient http = new HttpClient())
            {
                // Handle special URL commands
                if (url.Equals("SyncUsers", StringComparison.OrdinalIgnoreCase))
                    return HandleSynchronizeUsersRequest(Request);

                if (url.Equals("logout", StringComparison.OrdinalIgnoreCase))
                    return HandleGrafanaLogoutRequest(Request);

                if (url.StartsWith("avatar/", StringComparison.OrdinalIgnoreCase))
                    return HandleGrafanaAvatarRequest(Request);

                UpdateRequest(url);

                if (Request.Method == HttpMethod.Get)
                    Request.Content = null;

                return await HandleResponse(http.SendAsync(Request, cancellationToken));
            }
        }

        /// <summary>
        /// Handle proxy of the specified Grafana URL for DELETE commands.
        /// </summary>
        /// <param name="url">URL to proxy.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Proxied response.</returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> ProxyDelete(string url, CancellationToken cancellationToken)
        {
            using (HttpClient http = new HttpClient())
            {
                UpdateRequest(url);

                foreach (KeyValuePair<string, IEnumerable<string>> header in Request.Headers)
                    http.DefaultRequestHeaders.Add(header.Key, header.Value);

                return await HandleResponse(http.DeleteAsync(Request.RequestUri, cancellationToken));
            }
        }

        /// <summary>
        /// Handle proxy of the specified Grafana URL for PATCH commands.
        /// </summary>
        /// <param name="url">URL to proxy.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Proxied response.</returns>
        [HttpPatch]
        public async Task<HttpResponseMessage> ProxyPatch(string url, CancellationToken cancellationToken)
        {
            using (HttpClient http = new HttpClient())
            {
                UpdateRequest(url);
                return await HandleResponse(http.SendAsync(Request, cancellationToken));
            }
        }

        private void UpdateRequest(string url)
        {
            SecurityPrincipal securityPrincipal = RequestContext.Principal as SecurityPrincipal;

            if ((object)securityPrincipal == null || (object)securityPrincipal.Identity == null)
                throw new SecurityException($"User \"{RequestContext.Principal?.Identity.Name}\" is unauthorized.");

            string userName = securityPrincipal.Identity.Name;
            Request.Headers.Add(s_authProxyHeaderName, userName);
            Request.RequestUri = new Uri($"{s_baseUrl}/{url}{Request.RequestUri.Query}");
        }

        private async Task<HttpResponseMessage> HandleResponse(Task<HttpResponseMessage> responseTask)
        {
            HttpResponseMessage response = await responseTask;
            HttpStatusCode statusCode = response.StatusCode;
            
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

                        Dictionary<string, string[]> userRoles = StartUserSynchronization();
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

            return response;
        }

        #endregion

        #region [ Static ]

        // Static Events

        /// <summary>
        /// Raised when the Grafana authentication proxy reports an important status message.
        /// </summary>
        public static event EventHandler<EventArgs<string>> StatusMessage;

        // Static Fields
        private static readonly string s_baseUrl;
        private static readonly string s_authProxyHeaderName;
        private static readonly int s_initializationTimeout;
        private static readonly string s_adminUser;
        private static readonly string s_logoutResource;
        private static readonly string s_avatarResource;
        private static readonly ShortSynchronizedOperation s_synchronizeUsers;
        private static readonly ManualResetEventSlim s_initializationWaitHandle;
        private static Dictionary<string, string[]> s_lastSecurityContext;
        private static Dictionary<string, string[]> s_latestSecurityContext;
        private static bool s_manualSynchronization;

        // Static Constructor
        static GrafanaAuthProxyController()
        {
            // Make sure openHistorian specific default service settings exist
            CategorizedSettingsElementCollection grafanaHosting = ConfigurationFile.Current.Settings["grafanaHosting"];

            // Make sure needed settings exist
            grafanaHosting.Add("ServerPath", DefaultServerPath, "Defines the path to the Grafana server to host - set to empty string to disable hosting.");
            grafanaHosting.Add("AuthProxyHeaderName", "X-WEBAUTH-USER", "Defines the authorization header name used for Grafana user authentication.");
            grafanaHosting.Add("InitializationTimeout", DefaultInitializationTimeout, "Defines the timeout, in seconds, for the Grafana system to initialize.");
            grafanaHosting.Add("AdminUser", "admin", "Defines the admin user for the Grafana configuration.");
            grafanaHosting.Add("LogoutResource", "Logout.cshtml", "Defines the relative URL to the logout page to use for Grafana users.");
            grafanaHosting.Add("AvatarResource", "Images/Icons/openHistorian.png", "Defines the relative URL to the 40x40px avatar image to use for Grafana users.");
            grafanaHosting.Add("HostedURL", DefaultHostedURL, "Defines the local URL to the hosted Grafana server instance. Setting is for internal use, external access to Grafana instance will be proxied via WebHostURL.");

            // Get settings as currently defined in configuration file
            s_authProxyHeaderName = grafanaHosting["AuthProxyHeaderName"].Value;
            s_initializationTimeout = grafanaHosting["InitializationTimeout"].ValueAs(DefaultInitializationTimeout) * 1000;
            s_adminUser = grafanaHosting["AdminUser"].Value;
            s_logoutResource = grafanaHosting["LogoutResource"].Value;
            s_avatarResource = grafanaHosting["AvatarResource"].Value;
            s_baseUrl = grafanaHosting["HostedURL"].Value;

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
                AdoSecurityProvider.SecurtyContextRefreshed += AdoSecurityProvider_SecurtyContextRefreshed;
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

        private static void AdoSecurityProvider_SecurtyContextRefreshed(object sender, EventArgs<Dictionary<string, string[]>> e)
        {
            Interlocked.Exchange(ref s_latestSecurityContext, e.Argument);
            s_synchronizeUsers?.RunOnceAsync();
        }

        private static void SynchronizeUsers()
        {
            Dictionary<string, string[]> securityContext = s_latestSecurityContext;

            // Skip user synchronization if security context has not changed
            if (!s_manualSynchronization && (object)s_latestSecurityContext != null && SecurityContextsAreEqual(securityContext, s_lastSecurityContext))
                return;

            s_manualSynchronization = false;
            Interlocked.Exchange(ref s_lastSecurityContext, securityContext);

            // Give initialization - which includes starting Grafana server process - a chance to start
            s_initializationWaitHandle.Wait(s_initializationTimeout);

            foreach (KeyValuePair<string, string[]> item in securityContext)
            {
                string user = item.Key;
                string[] roles = item.Value;
                JObject content;
                 
                // Check if user exists
                dynamic userResult = CallAPIFunction(HttpMethod.Get, $"{s_baseUrl}/api/users/lookup?loginOrEmail={user}").Result;

                if ((object)userResult.id == null)
                {
                    // Create a new user
                    content = JObject.FromObject(new
                    {
                        name = user,
                        email = "",
                        login = user,
                        password = PasswordGenerator.Default.GeneratePassword()
                    });

                    userResult = CallAPIFunction(HttpMethod.Post, $"{s_baseUrl}/api/admin/users", content.ToString()).Result;

                    OnStatusMessage($"New user \"{user}\" encountered: {userResult.message}");
                }

                if ((object)userResult.id == null)
                    continue;

                // Update user's Grafana admin role status
                content = JObject.FromObject(new
                {
                    isGrafanaAdmin = UserIsGrafanaAdmin(roles)
                });

                string message = CallAPIFunction(HttpMethod.Put, $"{s_baseUrl}/api/admin/users/{userResult.id}/permissions", content.ToString()).Result.message;

                if (!message.Equals("User permissions updated", StringComparison.OrdinalIgnoreCase))
                    OnStatusMessage($"Issue updating permissions for user \"{user}\": {message}");

                // Set organizational role: role = Admin / Editor / Viewer
                content = JObject.FromObject(new
                {
                    role = TranslateRole(roles)
                });

                message = CallAPIFunction(new HttpMethod("PATCH"), $"{s_baseUrl}/api/org/users/{userResult.id}", content.ToString()).Result.message;

                if (!message.Equals("Organization user updated", StringComparison.OrdinalIgnoreCase))
                    OnStatusMessage($"Issue assigning role for user \"{user}\": {message}");
            }

            OnStatusMessage($"Synchronized security context with {securityContext.Count} users to Grafana.");
        }

        private static async Task<dynamic> CallAPIFunction(HttpMethod method, string url, string content = null)
        {
            using (HttpClient http = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(method, url);

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add(s_authProxyHeaderName, s_adminUser);

                if ((object)content != null)
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await http.SendAsync(request);

                content = await response.Content.ReadAsStringAsync();

                return JObject.Parse(content);
            }
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
                string[] value;

                if (!right.TryGetValue(item.Key, out value))
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

        private static Dictionary<string, string[]> StartUserSynchronization()
        {
            Dictionary<string, string[]> userRoles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            using (UserRoleCache userRoleCache = UserRoleCache.GetCurrentCache())
            {
                TableOperations<UserAccount> userAccountTable = new TableOperations<UserAccount>(connection);

                foreach (UserAccount user in userAccountTable.QueryRecords())
                {
                    string userName = user.AccountName;
                    string[] roles;

                    if (userRoleCache.TryGetUserRole(userName, out roles))
                        userRoles[userName] = roles;
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

        private static HttpResponseMessage HandleSynchronizeUsersRequest(HttpRequestMessage request)
        {
            Dictionary<string, string[]> userRoles = StartUserSynchronization();

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };

            response.Content = new StringContent($@"
                <html>
                <head>
                    <title>Grafana User Synchronization</title>
                    <link rel=""shortcut icon"" href=""@GSF/Web/Shared/Images/Icons/favicon.ico"" />
                </head>
                <body>
                    Security context with {userRoles.Count} users and associated roles queued for Grafana user synchronization.
                </body>
                </html>
            ", Encoding.UTF8, "text/html"); 

            return response;
        }

        private static HttpResponseMessage HandleGrafanaLogoutRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Redirect) { RequestMessage = request };
            Uri uri = request.RequestUri;

            response.Headers.Location = new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}/{s_logoutResource}");

            return response;
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