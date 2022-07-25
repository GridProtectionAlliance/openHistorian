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
using GSF.Collections;

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

        public static string InjectProtocolOrigins(string protocolOriginsJson, string[] allowedOrigins, string protocol)
        {
            List<ProtocolOrigins> protocolOrigins = JsonConvert.DeserializeObject<List<ProtocolOrigins>>(protocolOriginsJson);
            bool foundProtocol = false;

            foreach (ProtocolOrigins current in protocolOrigins!)
            {
                if (!string.Equals(current.protocol, protocol, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (current.allowed_origins.ToArray().CompareTo(allowedOrigins) == 0)
                    return protocolOriginsJson;

                current.allowed_origins = allowedOrigins.ToList();
                foundProtocol = true;
                break;
            }

            if (!foundProtocol)
            {
                protocolOrigins.Add(new ProtocolOrigins
                {
                    allowed_origins = allowedOrigins.ToList(),
                    protocol = protocol
                });
            }

            return JsonConvert.SerializeObject(protocolOrigins, Formatting.None);
        }

        public static string InjectExemptDomainFilesTypes(string exemptDomainFilesTypesJson, string fileExtension, string[] domains)
        {
            List<ExemptDomainFilesTypes> exemptDomainFilesTypes = JsonConvert.DeserializeObject<List<ExemptDomainFilesTypes>>(exemptDomainFilesTypesJson);
            bool foundFileExtension = false;

            foreach (ExemptDomainFilesTypes current in exemptDomainFilesTypes!)
            {
                if (!string.Equals(current.file_extension, fileExtension, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (current.domains.ToArray().CompareTo(domains) == 0)
                    return exemptDomainFilesTypesJson;

                current.domains = domains.ToList();
                foundFileExtension = true;
                break;
            }

            if (!foundFileExtension)
            {
                exemptDomainFilesTypes.Add(new ExemptDomainFilesTypes
                {
                    file_extension = fileExtension,
                    domains = domains
                });
            }

            return JsonConvert.SerializeObject(exemptDomainFilesTypes, Formatting.None);
        }
    }
}
