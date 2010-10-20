//******************************************************************************************************
//  ServiceClient.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  10/20/2010 - Mihir Brahmbhatt
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using TimeSeriesFramework;
using TVA;
using TVA.IO;
using TVA.Services;

namespace openHistorian
{
    public class ServiceClient : ServiceClientBase
    {
        protected override void ClientHelper_ReceivedServiceResponse(object sender, EventArgs<ServiceResponse> e)
        {
            List<object> attachments = e.Argument.Attachments;

            // Handle any special attachments coming in from service
            if (attachments != null)
            {
                foreach (object attachment in attachments)
                {
                    //Need to implement a code for incoming request.
                }
            }

            // Allow base class to handle common response
            base.ClientHelper_ReceivedServiceResponse(sender, e);
        }
    }
}
