//******************************************************************************************************
//  JsonHelpers.cs - Gbtc
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
//  05/18/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable ClassNeverInstantiated.Local
namespace UpdateCOMTRADECounters
{
    internal static class JsonHelpers
    {
        private class ProtocolOrigins
        {
            public IList<string> allowed_origins { get; set; }
            public string protocol { get; set; }
        }

        private class ExemptDomainFilesTypes
        {
            public string file_extension { get; set; }
            public IList<string> domains { get; set; }  
        }

        public static string InjectProtocolOrigins(string protocolOriginsJson, string targetAllowedOrigin, string protocol)
        {
            List<ProtocolOrigins> protocolOrigins = JsonConvert.DeserializeObject<List<ProtocolOrigins>>(protocolOriginsJson);
            bool foundProtocol = false;

            foreach (ProtocolOrigins current in protocolOrigins!)
            {
                if (!string.Equals(current.protocol, protocol, StringComparison.OrdinalIgnoreCase))
                    continue;

                foundProtocol = true;

                // Disallow single wildcard without port
                if (current.allowed_origins.Count == 1 && current.allowed_origins[0] == "*")
                {
                    current.allowed_origins[0] = targetAllowedOrigin;
                }
                else
                {
                    if (current.allowed_origins.Any(allowedOrigin => string.Equals(allowedOrigin, targetAllowedOrigin, StringComparison.OrdinalIgnoreCase)))
                        return protocolOriginsJson;

                    current.allowed_origins.Add(targetAllowedOrigin);
                }
                break;
            }

            if (!foundProtocol)
            {
                protocolOrigins.Add(new ProtocolOrigins
                {
                    allowed_origins = new[] { targetAllowedOrigin },
                    protocol = protocol
                });
            }

            return JsonConvert.SerializeObject(protocolOrigins, Formatting.None);
        }

        public static string InjectExemptDomainFilesTypes(string exemptDomainFilesTypesJson, string fileExtension, string targetDomain)
        {
            List<ExemptDomainFilesTypes> exemptDomainFilesTypes = JsonConvert.DeserializeObject<List<ExemptDomainFilesTypes>>(exemptDomainFilesTypesJson);
            bool foundFileExtension = false;

            foreach (ExemptDomainFilesTypes current in exemptDomainFilesTypes!)
            {
                if (!string.Equals(current.file_extension, fileExtension, StringComparison.OrdinalIgnoreCase))
                    continue;

                foundFileExtension = true;

                // Disallow single wildcard without port
                if (current.domains.Count == 1 && current.domains[0] == "*")
                {
                    current.domains[0] = targetDomain;
                }
                else
                {
                    if (current.domains.Any(domain => string.Equals(domain, targetDomain, StringComparison.OrdinalIgnoreCase)))
                        return exemptDomainFilesTypesJson;

                    current.domains.Add(targetDomain);
                }
                break;
            }

            if (!foundFileExtension)
            {
                exemptDomainFilesTypes.Add(new ExemptDomainFilesTypes
                {
                    file_extension = fileExtension,
                    domains = new[] { targetDomain }
                });
            }

            return JsonConvert.SerializeObject(exemptDomainFilesTypes, Formatting.None);
        }

        public static string RemoveProtocolOrigins(string protocolOriginsJson, string targetAllowedOrigin, string protocol)
        {
            List<ProtocolOrigins> protocolOrigins = JsonConvert.DeserializeObject<List<ProtocolOrigins>>(protocolOriginsJson);

            for (int i = protocolOrigins!.Count - 1; i >= 0; i--)
            {
                ProtocolOrigins current = protocolOrigins[i];

                if (!string.Equals(current.protocol, protocol, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Remove the target origin from the allowed origins list
                for (int j = current.allowed_origins.Count - 1; j >= 0; j--)
                {
                    if (string.Equals(current.allowed_origins[j], targetAllowedOrigin, StringComparison.OrdinalIgnoreCase))
                    {
                        current.allowed_origins.RemoveAt(j);
                        break; // Only remove one matching instance
                    }
                }

                // If no allowed origins remain for this protocol, remove the entire protocol entry
                if (current.allowed_origins.Count == 0) 
                    protocolOrigins.RemoveAt(i);
            }

            return JsonConvert.SerializeObject(protocolOrigins, Formatting.None);
        }

        public static string RemoveExemptDomainFilesTypes(string exemptDomainFilesTypesJson, string fileExtension, string targetDomain)
        {
            List<ExemptDomainFilesTypes> exemptDomainFilesTypes = JsonConvert.DeserializeObject<List<ExemptDomainFilesTypes>>(exemptDomainFilesTypesJson);

            for (int i = exemptDomainFilesTypes!.Count - 1; i >= 0; i--)
            {
                ExemptDomainFilesTypes current = exemptDomainFilesTypes[i];

                if (!string.Equals(current.file_extension, fileExtension, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Remove the target domain from the domains list
                for (int j = current.domains.Count - 1; j >= 0; j--)
                {
                    if (string.Equals(current.domains[j], targetDomain, StringComparison.OrdinalIgnoreCase))
                    {
                        current.domains.RemoveAt(j);
                        break; // Only remove one matching instance
                    }
                }

                // If no domains remain for this file extension, remove the entire file extension entry
                if (current.domains.Count == 0) 
                    exemptDomainFilesTypes.RemoveAt(i);
            }

            return JsonConvert.SerializeObject(exemptDomainFilesTypes, Formatting.None);
        }
    }
}
