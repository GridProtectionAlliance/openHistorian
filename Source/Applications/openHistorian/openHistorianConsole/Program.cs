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
//  05/04/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using GSF.Console;

namespace openHistorian
{
    internal class Program
    {
        private static ServiceClient s_serviceClient;

        private static void Main(string[] args)
        {
            // Enable console events.
            Events.ConsoleClosing += OnConsoleClosing;
            Events.EnableRaisingEvents();

            // Start the client component.
            s_serviceClient = new ServiceClient();
            s_serviceClient.Start(args);
            s_serviceClient.Dispose();

            Environment.Exit(0);
        }

        // Dispose the client component.
        private static void OnConsoleClosing(object sender, CancelEventArgs e) => 
            s_serviceClient?.Dispose();
    }
}