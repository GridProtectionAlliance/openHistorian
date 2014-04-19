//******************************************************************************************************
//  LogMessage.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  4/11/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Text;

namespace GSF.Diagnostics
{
    /// <summary>
    /// An individual log message.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// The verbose level associated with the message
        /// </summary>
        public VerboseLevel Level;
        /// <summary>
        /// The source of the log message
        /// </summary>
        public LogSource Source;
        /// <summary>
        /// The event ID assigned by the source
        /// </summary>
        public int EventID;
        /// <summary>
        /// The event code assigned by the source
        /// </summary>
        public string EventCode;
        /// <summary>
        /// The message of the log
        /// </summary>
        public string Message;
        /// <summary>
        /// Extra values with the log. 
        /// </summary>
        public string[] Values;
        /// <summary>
        /// An exception object if one is provided.
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public string GetMessage(bool details)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time: ");
            sb.Append(DateTime.Now.ToString());
            switch (Level)
            {
                case VerboseLevel.Debug:
                    sb.Append(" Level: Debug ");
                    break;
                case VerboseLevel.Information:
                    sb.Append(" Level: Debug ");
                    break;
                case VerboseLevel.Warning:
                    sb.Append(" Level: Debug ");
                    break;
                case VerboseLevel.Error:
                    sb.Append(" Level: Debug ");
                    break;
                case VerboseLevel.Fatal:
                    sb.Append(" Level: Debug ");
                    break;
                default:
                    sb.Append(" Level: Unknown ");
                    break;
            }

            sb.AppendLine();
            sb.Append("Source: ");
            sb.AppendLine(Source.GetString(details));
            sb.Append("Event ID: ");
            sb.AppendLine(EventID.ToString());
            sb.AppendLine("Event Code:");
            sb.AppendLine(EventCode);
            sb.AppendLine("Message:");
            sb.AppendLine(Message);

            if (Exception != null)
            {
                sb.AppendLine("Exception: ");
                sb.AppendLine(Exception.ToString());
            }
            return sb.ToString();
        }
    }
}