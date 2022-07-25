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
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

// ReSharper disable LocalizableElement
namespace UpdateCOMTRADECounters
{
    // This application loads needed dependencies from embedded resources so
    // app will run from a single downloaded standalone executable
    internal static class Program
    {
        private const string UriScheme = "comtrade-update-counter";
        private const string FriendlyName = "COMTRADE Update Counter";

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

            if (commandLine.Contains("-registeronly"))
                Environment.Exit(0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static void RegisterUriScheme(string commandLine)
        {
            bool targetAllUsers = false;

            try
            {
                // -AllUsers flag requires admin elevation
                targetAllUsers = commandLine.Contains("-allusers");
                RegistryKey rootKey = targetAllUsers ? Registry.LocalMachine : Registry.CurrentUser;
                bool updateUriScheme;

                using (RegistryKey allUsersKey = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\Classes\\{UriScheme}"))
                using (RegistryKey localUserKey = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\Classes\\{UriScheme}"))
                {
                    updateUriScheme = (allUsersKey is null && (localUserKey is null || targetAllUsers));
                }

                if (updateUriScheme)
                {
                    using RegistryKey uriSchemeKey = rootKey.CreateSubKey($"SOFTWARE\\Classes\\{UriScheme}");

                    if (uriSchemeKey is null)
                        return;

                    uriSchemeKey.SetValue("", $"URL:{FriendlyName}");
                    uriSchemeKey.SetValue("URL Protocol", "");

                    string applicationLocation = CurrentAssembly.Location;

                    using RegistryKey defaultIcon = uriSchemeKey.CreateSubKey("DefaultIcon");
                    defaultIcon?.SetValue("", $"{applicationLocation},1");

                    using RegistryKey commandKey = uriSchemeKey.CreateSubKey(@"shell\open\command");
                    commandKey?.SetValue("", $"\"{applicationLocation}\" \"%1\"");
                }

                if (targetAllUsers)
                {
                    void applyAutoLaunchProtocolPolicy(RegistryKey policyKey, string protocol)
                    {
                        string originPolicies = policyKey.GetValue("AutoLaunchProtocolsFromOrigins")?.ToString();

                        if (string.IsNullOrEmpty(originPolicies))
                            originPolicies = "[]";

                        policyKey.SetValue("AutoLaunchProtocolsFromOrigins",
                            JsonHelpers.InjectProtocolOrigins(originPolicies, new[] { "*" }, protocol));
                    }


                    void applyFileTypeWarningExemptionPolicy(RegistryKey policyKey, string fileType)
                    {
                        void applyExemptionPolicy(string keyName)
                        {
                            string filesTypeDomainExceptions = policyKey.GetValue(keyName)?.ToString();

                            if (string.IsNullOrEmpty(filesTypeDomainExceptions))
                                filesTypeDomainExceptions = "[]";

                            policyKey.SetValue(keyName,
                                JsonHelpers.InjectExemptDomainFilesTypes(filesTypeDomainExceptions, fileType, new[] { "*" }));
                        }

                        // Apply policy to old key name (deprecated)
                        applyExemptionPolicy("ExemptDomainFileTypePairsFromFileTypeDownloadWarnings");

                        // Apply policy to new key name
                        applyExemptionPolicy("ExemptFileTypeDownloadWarnings");
                    }

                    void applyPolicies(RegistryKey policyKey)
                    {
                        if (policyKey is null)
                            return;

                        applyAutoLaunchProtocolPolicy(policyKey, UriScheme);
                        applyFileTypeWarningExemptionPolicy(policyKey, "cfg");
                    }

                    // Apply Chrome policy
                    applyPolicies(
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Google\\Chrome", true) ??
                        Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Google\\Chrome", true));

                    // Apply Edge policy
                    applyPolicies(
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Edge", true) ??
                        Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Edge", true));

                    // Apply Firefox policy
                    applyPolicies(
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Mozilla\\Firefox", true) ??
                        Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Mozilla\\Firefox", true));
                }
            }
            catch (Exception ex)
            {
                if (!commandLine.Contains("-silent"))
                    MessageBox.Show($"Failed to register URI scheme \"{UriScheme}\" for {(targetAllUsers ? "all users" : "current user")}: {ex.Message}", "URI Scheme Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
