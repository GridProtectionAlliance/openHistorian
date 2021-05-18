//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
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
//  09/02/2010 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using GSF.Console;
using GSF.Threading;
using GSF.TimeSeries;

namespace openHistorian
{
    public static class Program
    {
        /// <summary>
        /// The service host instance for the application.
        /// </summary>
        public static readonly ServiceHost Host;

        private static readonly Arguments s_args;
        private static readonly Mutex s_singleInstanceMutex;

        static Program()
        {
            s_args = new Arguments(Environment.CommandLine, true);

            if (s_args.Exists("RestoreFiles"))
            {
                ServiceHost.RestoreEmbeddedResources();
                Environment.Exit(0);
            }

            Host = new ServiceHost();

            s_singleInstanceMutex = InterprocessLock.GetNamedMutex(false);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
        #if !DEBUG
            if (!s_args.Exists("NoMutex") && !s_singleInstanceMutex.WaitOne(0, true))
                Environment.Exit(1);
        #endif

            bool runAsService;
            bool runAsApplication;

            if (s_args.Count == 0)
            {
            #if DEBUG
                runAsService = false;
                runAsApplication = true;
            #else
                runAsService = true;
                runAsApplication = false;
            #endif
            }
            else
            {
                runAsService = s_args.Exists("RunAsService");
                runAsApplication = s_args.Exists("RunAsApplication");

                if (!runAsService && !runAsApplication && !s_args.Exists("RunAsConsole"))
                {
                    MessageBox.Show("Invalid argument. If specified, argument must be one of: -RunAsService, -RunAsApplication or -RunAsConsole.");
                    Environment.Exit(1);
                }
            }

            if (runAsService)
            {
                // Run as Windows Service.
                ServiceBase.Run(new ServiceBase[] { Host });
            }
            else if (runAsApplication)
            {
                // Run as Windows Application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DebugHost(Host));
            }
            else
            {
                ConsoleHost consoleHost = new ConsoleHost(Host);
                consoleHost.Run();
                Environment.Exit(0);
            }
        }
    }
}