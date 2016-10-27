//******************************************************************************************************
//  MessageFlags.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Various flags that can be attributed to a <see cref="LogMessage"/>. 
    /// </summary>
    [Flags]
    public enum MessageFlags 
        : byte
    {
        None = 0,
        /// <summary>
        /// Indicates that a segment of code is not being used properly or ideally.
        /// </summary>
        UsageIssue = 1 << 1,
        /// <summary>
        /// Indicates that a bug in the code exists somewhere. This is helpful when the programmer suspects that 
        /// certain exceptions were not properly handled.
        /// </summary>
        BugReport = 1 << 2,
        /// <summary>
        /// A flag indicating that a performance related issue has occurred. 
        /// </summary>
        PerformanceIssue = 1 << 3,
        /// <summary>
        /// Indicates this message has security implications with it. Such as 
        /// a successful/unsuccessful authentication.
        /// </summary>
        SecurityMessage = 1 << 4,
        /// <summary>
        /// These messages in generally should always be logged because they report the state of the current system's health. Normally this 
        /// will be when the system health is abnormal, for example, something abnormal is happening in the background that is important to 
        /// note and can assist debugging other components. Messages raised here would include First Chance Exceptions 
        /// and exceptions in log message routing (such as Message Suppression).
        /// </summary>
        SystemHealth = 1 << 5,

    }
}