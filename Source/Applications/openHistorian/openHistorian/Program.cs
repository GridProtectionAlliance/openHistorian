//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
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

#if !DEBUG
//#define RunAsService
#endif

using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace openHistorian
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            bool runAsService;
#if RunAsService
            runAsService = true;
#else
            runAsService = false;
#endif
            bool argRunAsService = args.Any(s => s.Equals("/RunAsService", StringComparison.OrdinalIgnoreCase));
            bool argRunAsApplication = args.Any(s => s.Equals("/RunAsApplication", StringComparison.OrdinalIgnoreCase));

            if (argRunAsApplication && argRunAsService)
            {
                MessageBox.Show("Too many arguments specified: Cannot run as an applcation and a service");
                return;
            }
            if (argRunAsService)
                runAsService = true;
            if (argRunAsApplication)
                runAsService = false;


            ServiceHost host = new ServiceHost();

            if (runAsService)
            {
                // Run as Windows Service.
                ServiceBase.Run(new ServiceBase[] { host });
            }
            else
            {
                // Run as Windows Application.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DebugHost(host));
            }
        }
    }
}