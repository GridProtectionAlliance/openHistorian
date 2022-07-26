//******************************************************************************************************
//  ShellHelpers.cs - Gbtc
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
//  05/16/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF.Reflection;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UpdateCOMTRADECounters
{
    internal static class ShellHelpers
    {
        private static readonly Guid DownloadsFolderID = new("374DE290-123F-4565-9164-39C4925E467B");

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

        public static string GetDownloadsFolder()
        {
            try
            {
                // Attempt to locate user's "Downloads" folder
                if (SHGetKnownFolderPath(DownloadsFolderID, 0, IntPtr.Zero, out string path) != 0)
                    throw new InvalidOperationException("Failed to locate user's \"Downloads\" folder");

                return path;
            }
            catch
            {
                // Fall-back on user's "Documents" folder
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public static string GetApplicationDataFolder()
        {
            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(rootFolder, AssemblyInfo.EntryAssembly.Company, AssemblyInfo.EntryAssembly.Name);    
        }
    }
}
