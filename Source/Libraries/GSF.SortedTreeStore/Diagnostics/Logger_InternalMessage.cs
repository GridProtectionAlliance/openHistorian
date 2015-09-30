//******************************************************************************************************
//  Logger_InternalMessage.cs - Gbtc
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
//  9/12/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Diagnostics
{
    public static partial class Logger
    {
        /// <summary>
        /// This class is to help prevent improper use of <see cref="LogMessage"/>
        /// </summary>
        private class InternalMessage
            : LogMessage
        {

            public InternalMessage(VerboseLevel level, InternalSource source, InternalType type, string eventName, string message, string details, Exception exception)
                : base(level, source, type, eventName, message, details, exception)
            {

            }

            public new InternalSource Source
            {
                get
                {
                    return base.Source as InternalSource;
                }
            }

            public new InternalType Type
            {
                get
                {
                    return base.Type as InternalType;
                }
            }
        }
    }
}
