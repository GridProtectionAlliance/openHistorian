//******************************************************************************************************
//  GenAzureADComponentGroup.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
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
//  02/25/2023 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WiXFolderGen
{
    internal static class GenAzureADComponentGroup
    {
        private static readonly string[] SourceFiles =
        {
            "Azure.Core.dll",
            "Microsoft.AspNetCore.Cryptography.Internal.dll",
            "Microsoft.AspNetCore.DataProtection.Abstractions.dll",
            "Microsoft.AspNetCore.DataProtection.dll",
            "Microsoft.Bcl.AsyncInterfaces.dll",
            "Microsoft.Extensions.Caching.Abstractions.dll",
            "Microsoft.Extensions.Caching.Memory.dll",
            "Microsoft.Extensions.Configuration.Abstractions.dll",
            "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
            "Microsoft.Extensions.DependencyInjection.dll",
            "Microsoft.Extensions.FileProviders.Abstractions.dll",
            "Microsoft.Extensions.Hosting.Abstractions.dll",
            "Microsoft.Extensions.Logging.Abstractions.dll",
            "Microsoft.Extensions.Logging.dll",
            "Microsoft.Extensions.Options.dll",
            "Microsoft.Extensions.Primitives.dll",
            "Microsoft.Graph.Core.dll",
            "Microsoft.Graph.dll",
            "Microsoft.Identity.Client.Broker.dll",
            "Microsoft.Identity.Client.Desktop.dll",
            "Microsoft.Identity.Client.dll",
            "Microsoft.Identity.Client.NativeInterop.dll",
            "Microsoft.Identity.Web.TokenCache.dll",
            "Microsoft.IdentityModel.Abstractions.dll",
            "Microsoft.IdentityModel.JsonWebTokens.dll",
            "Microsoft.IdentityModel.Logging.dll",
            "Microsoft.IdentityModel.Protocols.dll",
            "Microsoft.IdentityModel.Protocols.OpenIdConnect.dll",
            "Microsoft.IdentityModel.Tokens.dll",
            "Microsoft.Web.WebView2.Core.dll",
            "Microsoft.Web.WebView2.WinForms.dll",
            "Microsoft.Web.WebView2.Wpf.dll",
            "Microsoft.Win32.Registry.dll",
            "System.Diagnostics.DiagnosticSource.dll",
            "System.Drawing.Common.dll",
            "System.IdentityModel.Tokens.Jwt.dll",
            "System.Memory.Data.dll",
            "System.Net.Http.Json.dll",
            "System.Net.Http.WinHttpHandler.dll",
            "System.Numerics.Vectors.dll",
            "System.Security.AccessControl.dll",
            "System.Security.Cryptography.Xml.dll",
            "System.Security.Permissions.dll",
            "System.Security.Principal.Windows.dll",
            "System.Text.Encodings.Web.dll",
            "System.Text.Json.dll"
        };

        public static void Generate()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("    <ComponentGroup Id=\"AzureADDependencyComponents\" Directory=\"INSTALLFOLDER\">");
            
            foreach (string file in SourceFiles)
            {
                builder.AppendLine($"      <Component Id=\"{Path.GetFileNameWithoutExtension(file)}\">");
                builder.AppendLine($"        <File Id=\"{file}\" Name=\"{file}\" Source=\"$(var.SolutionDir)\\Dependencies\\GSF\\{file}\">");
                builder.AppendLine($"          <netfx:NativeImage Id=\"ngen_{file}\"  Platform=\"64bit\" Priority=\"0\" AppBaseDirectory=\"INSTALLFOLDER\"/>");
                builder.AppendLine("        </File>");
                builder.AppendLine("      </Component>");
            }

            builder.AppendLine("    </ComponentGroup>");
            
            string output = builder.ToString();

            Console.WriteLine(output);
            Console.WriteLine();

            try
            {
                Clipboard.SetText(output);
                Console.WriteLine("Content copied to clipboard.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to copy text to clipboard: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
        }
    }
}
