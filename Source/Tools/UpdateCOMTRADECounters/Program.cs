//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/17/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using GSF;
using GSF.Console;
using Microsoft.Win32;

// ReSharper disable LocalizableElement
namespace UpdateCOMTRADECounters
{
    // This application loads needed dependencies from embedded resources so
    // application will run from a single downloaded standalone executable
    internal static class Program
    {
        private const string UriScheme = "comtrade-update-counter";
        private const string FriendlyName = "COMTRADE Update Counter";
        private const string ChromePolicies = "SOFTWARE\\Policies\\Google\\Chrome";
        private const string EdgePolicies = "SOFTWARE\\Policies\\Microsoft\\Edge";
        private const string FirefoxPolicies = "SOFTWARE\\Policies\\Mozilla\\Firefox";
        private const string AutoLaunchProtocolPolicy = "AutoLaunchProtocolsFromOrigins";
        private const string ExemptFileTypeWarningPolicyOld = "ExemptDomainFileTypePairsFromFileTypeDownloadWarnings";
        private const string ExemptFileTypeWarningPolicy = "ExemptFileTypeDownloadWarnings";

        private static Assembly s_currentAssembly;
        private static Dictionary<string, Assembly> s_assemblyCache;
        
        private static Assembly CurrentAssembly => s_currentAssembly ??= typeof(Program).Assembly;

        private static Dictionary<string, Assembly> AssemblyCache => s_assemblyCache ??= new Dictionary<string, Assembly>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Hook into assembly resolve event so assemblies can be loaded from embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyFromResource;

            // Load needed assemblies from embedded resources in dependency order
            AppDomain.CurrentDomain.Load("Antlr3.Runtime");
            AppDomain.CurrentDomain.Load("ExpressionEvaluator");
            AppDomain.CurrentDomain.Load("GSF.Core");
            AppDomain.CurrentDomain.Load("GSF.Communication");
            AppDomain.CurrentDomain.Load("GSF.Net");
            AppDomain.CurrentDomain.Load("GSF.Security");
            AppDomain.CurrentDomain.Load("GSF.ServiceProcess");
            AppDomain.CurrentDomain.Load("GSF.TimeSeries");
            AppDomain.CurrentDomain.Load("GSF.PhasorProtocols");
            AppDomain.CurrentDomain.Load("Newtonsoft.Json");
            AppDomain.CurrentDomain.Load("GSF.COMTRADE");

            string commandLine = Environment.CommandLine.ToLowerInvariant();

            RegisterUriScheme(commandLine);

            if (commandLine.Contains("-registeronly") || commandLine.Contains("-forceupdate"))
                Environment.Exit(0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static void RegisterUriScheme(string commandLine)
        {
            bool targetAllUsers = false;
            bool silent = commandLine.Contains("-silent");

            try
            {
                // -AllUsers flag requires admin elevation
                targetAllUsers = commandLine.Contains("-allusers");
                bool unregister = commandLine.Contains("-unregister");
                bool forceUpdate = commandLine.Contains("-forceupdate");

                if (unregister || forceUpdate)
                {
                    if (targetAllUsers)
                        DeleteRegistration(Registry.LocalMachine, silent);

                    DeleteRegistration(Registry.CurrentUser, silent);

                    if (unregister)
                        Environment.Exit(0);
                }

                RegistryKey rootKey = targetAllUsers ? Registry.LocalMachine : Registry.CurrentUser;
                string applicationFolder = ShellHelpers.GetApplicationDataFolder();
                string applicationFilePath = Path.Combine(applicationFolder, $"{nameof(UpdateCOMTRADECounters)}.exe");

                if (forceUpdate)
                {
                    try
                    {
                        File.Delete(applicationFilePath);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (targetAllUsers)
                {
                    applicationFilePath = CurrentAssembly.Location;
                }
                else
                {
                    try
                    {
                        // Make sure target application folder exists
                        if (!Directory.Exists(applicationFolder))
                            Directory.CreateDirectory(applicationFolder);

                        // Copy executable to application data folder as primary install location
                        if (!File.Exists(applicationFilePath))
                            File.Copy(CurrentAssembly.Location, applicationFilePath);
                    }
                    catch
                    {
                        applicationFilePath = CurrentAssembly.Location;
                    }
                }

                using (RegistryKey allUsersKey = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\Classes\\{UriScheme}"))
                using (RegistryKey localUserKey = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\Classes\\{UriScheme}"))
                {
                    if (allUsersKey is null && (localUserKey is null || targetAllUsers))
                    {
                        using RegistryKey uriSchemeKey = rootKey.CreateSubKey($"SOFTWARE\\Classes\\{UriScheme}");

                        if (uriSchemeKey is null)
                            return;

                        uriSchemeKey.SetValue("", $"URL:{FriendlyName}");
                        uriSchemeKey.SetValue("URL Protocol", "");

                        using RegistryKey defaultIcon = uriSchemeKey.CreateSubKey("DefaultIcon");
                        defaultIcon?.SetValue("", $"{applicationFilePath},1");

                        using RegistryKey commandKey = uriSchemeKey.CreateSubKey(@"shell\open\command");
                        commandKey?.SetValue("", $"\"{applicationFilePath}\" \"%1\"");
                    }
                }

                if (targetAllUsers)
                {
                    RegisterBrowserPolicies(Registry.LocalMachine);
                }
                else
                {
                    using RegistryKey chromeKeyAllUsers = Registry.LocalMachine.OpenSubKey(ChromePolicies);
                    using RegistryKey edgeKeyAllUsers = Registry.LocalMachine.OpenSubKey(EdgePolicies);
                    using RegistryKey firefoxKeyAllUsers = Registry.LocalMachine.OpenSubKey(FirefoxPolicies);
                    using RegistryKey chromeKeyLocalUser = Registry.CurrentUser.OpenSubKey(ChromePolicies);
                    using RegistryKey edgeKeyLocalUser = Registry.CurrentUser.OpenSubKey(EdgePolicies);
                    using RegistryKey firefoxKeyLocalUser = Registry.CurrentUser.OpenSubKey(FirefoxPolicies);

                    if ((chromeKeyAllUsers?.GetValue(AutoLaunchProtocolPolicy) is null ||
                         edgeKeyAllUsers?.GetValue(AutoLaunchProtocolPolicy) is null ||
                         firefoxKeyAllUsers?.GetValue(AutoLaunchProtocolPolicy) is null) &&
                        (chromeKeyLocalUser?.GetValue(AutoLaunchProtocolPolicy) is null ||
                         edgeKeyLocalUser?.GetValue(AutoLaunchProtocolPolicy) is null ||
                         firefoxKeyLocalUser?.GetValue(AutoLaunchProtocolPolicy) is null))
                    {
                        RegisterBrowserPolicies(Registry.CurrentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show($"Failed to register URI scheme \"{UriScheme}\" for {(targetAllUsers ? "all users" : "current user")}: {ex.Message}", "URI Scheme Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void DeleteRegistration(RegistryKey keyRoot, bool silent)
        {
            try
            {
                DeleteRegistryKey(keyRoot, $"SOFTWARE\\Classes\\{UriScheme}");
                DeleteRegistryKey(keyRoot, ChromePolicies);
                DeleteRegistryKey(keyRoot, EdgePolicies);
                DeleteRegistryKey(keyRoot, FirefoxPolicies);
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show($"Failed to unregister URI scheme \"{UriScheme}\" for {(keyRoot == Registry.LocalMachine ? "all users" : "current user")}: {ex.Message}", "URI Scheme Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void DeleteRegistryKey(RegistryKey key, string name)
        {
            RegistryKey subKey = key.OpenSubKey(name, true);

            if (subKey is null)
                return;

            string[] keyNames = subKey.GetSubKeyNames();
            
            foreach (string keyName in keyNames)
                DeleteRegistryKey(subKey, keyName);

            key.DeleteSubKey(name);
        }

        private static void RegisterBrowserPolicies(RegistryKey keyRoot)
        {
            void applyAutoLaunchProtocolPolicy(RegistryKey policyKey, string allowedOrigin, string protocol)
            {
                string originPolicies = policyKey.GetValue(AutoLaunchProtocolPolicy)?.ToString();

                if (string.IsNullOrEmpty(originPolicies))
                    originPolicies = "[]";

                policyKey.SetValue(AutoLaunchProtocolPolicy,
                    JsonHelpers.InjectProtocolOrigins(originPolicies, allowedOrigin, protocol));
            }


            void applyFileTypeWarningExemptionPolicy(RegistryKey policyKey, string fileType, string domain)
            {
                void applyExemptionPolicy(string keyName)
                {
                    string filesTypeDomainExceptions = policyKey.GetValue(keyName)?.ToString();

                    if (string.IsNullOrEmpty(filesTypeDomainExceptions))
                        filesTypeDomainExceptions = "[]";

                    policyKey.SetValue(keyName,
                        JsonHelpers.InjectExemptDomainFilesTypes(filesTypeDomainExceptions, fileType, domain));
                }

                // Apply policy to old key name (deprecated)
                applyExemptionPolicy(ExemptFileTypeWarningPolicyOld);

                // Apply policy to new key name
                applyExemptionPolicy(ExemptFileTypeWarningPolicy);
            }

            void applyPolicies(RegistryKey policyKey)
            {
                if (policyKey is null)
                    return;

                string targetUri = $"*:{GetTargetWebPort()}";

                applyAutoLaunchProtocolPolicy(policyKey, targetUri, UriScheme);
                applyFileTypeWarningExemptionPolicy(policyKey, "cfg", targetUri);
            }

            // Apply Chrome policy
            applyPolicies(
                keyRoot.CreateSubKey(ChromePolicies, true) ??
                keyRoot.OpenSubKey(ChromePolicies, true));

            // Apply Edge policy
            applyPolicies(
                keyRoot.CreateSubKey(EdgePolicies, true) ??
                keyRoot.OpenSubKey(EdgePolicies, true));

            // Apply Firefox policy
            applyPolicies(
                keyRoot.CreateSubKey(FirefoxPolicies, true) ??
                keyRoot.OpenSubKey(FirefoxPolicies, true));
        }

        private static int GetTargetWebPort()
        {
            const string SourceApp = "openHistorian";
            const int DefaultPort = 8180;

            try
            {
                bool tryParseUrl(string url, out string callback)
                {
                    callback = null;

                    try
                    {
                        string query = new Uri(url).Query;

                        while (query.Length > 1 && query[0] == '?')
                            query = query.Length == 1 ? string.Empty : query.Substring(1);

                        if (string.IsNullOrEmpty(query))
                            return false;

                        Dictionary<string, string> parameters = query.ParseKeyValuePairs('&');

                        parameters.TryGetValue("callback", out callback);

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                bool tryParseCommandLine(out string callback)
                {
                    callback = null;
                    string[] args = Arguments.ToArgs(Environment.CommandLine);
                    return args.Length > 1 && tryParseUrl(args[1], out callback);
                }

                bool tryParseClipboard(out string callback)
                {
                    callback = null;
                    return Clipboard.ContainsText() && tryParseUrl(Clipboard.GetText(), out callback);
                }

                // Check for callback parameter in protocol URL first - best option for typical use case
                if (!tryParseCommandLine(out string hostUrl) && !tryParseClipboard(out hostUrl))
                {
                    // Fall back on looking for openHistorian installation - useful during installation
                    string configFile = Registry.GetValue($"HKEY_LOCAL_MACHINE\\Software\\Grid Protection Alliance\\{SourceApp}", "InstallPath", $"C:\\Program Files\\{SourceApp}\\") as string;

                    // Return default value if config file cannot be found
                    if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
                        return DefaultPort;

                    // Load web host URL that includes listening port from target config file
                    XDocument serviceConfig = XDocument.Load(configFile);

                    hostUrl = serviceConfig
                        .Descendants("systemSettings")
                        .SelectMany(systemSettings => systemSettings.Elements("add"))
                        .Where(element => "WebHostURL".Equals((string)element.Attribute("name"), StringComparison.Ordinal))
                        .Select(element => (string)element.Attribute("value"))
                        .FirstOrDefault();
                }

                return string.IsNullOrEmpty(hostUrl) ? DefaultPort : 
                    new Uri(hostUrl.Replace("//+:", "//localhost:")).Port;
            }
            catch
            {
                return DefaultPort;
            }
        }

        private static Assembly ResolveAssemblyFromResource(object sender, ResolveEventArgs e)
        {
            string shortName = e.Name.Split(',')[0].Trim();

            if (AssemblyCache.TryGetValue(shortName, out Assembly resourceAssembly))
                return resourceAssembly;

            // Loop through all of the resources in the current assembly
            foreach (string name in CurrentAssembly.GetManifestResourceNames())
            {
                // See if the embedded resource name matches the assembly it is trying to load
                if (!string.Equals(Path.GetFileNameWithoutExtension(name), $"{nameof(UpdateCOMTRADECounters)}.{shortName}", StringComparison.OrdinalIgnoreCase))
                    continue;

                // If so, load embedded resource assembly into a binary buffer
                Stream resourceStream = CurrentAssembly.GetManifestResourceStream(name);

                if (resourceStream is null)
                    break;

                byte[] buffer = new byte[resourceStream.Length];

                // ReSharper disable once MustUseReturnValue
                resourceStream.Read(buffer, 0, (int)resourceStream.Length);
                resourceStream.Close();

                // Load assembly from binary buffer
                resourceAssembly = Assembly.Load(buffer);

                // Add assembly to the cache
                AssemblyCache.Add(shortName, resourceAssembly);
                break;
            }

            return resourceAssembly;
        }
    }
}
