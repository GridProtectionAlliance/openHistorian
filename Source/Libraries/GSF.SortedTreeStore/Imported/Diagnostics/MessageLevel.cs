//******************************************************************************************************
//  MessageLevel.cs - Gbtc
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
    /// The level of importance of a message that is being raised. Increasing from Debug, Info, Warning, Error, Critical.
    /// None can be specified, but this indicates an importance level cannot be properly identified, and the message
    /// will be routed based on <see cref="MessageFlags"/>.
    /// </summary>
    public enum MessageLevel 
        : byte
    {
        /// <summary>
        /// Indicates a level that cannot be generally subscribed to. In this case,
        /// the message can only be subscribed to if a <see cref="MessageFlags"/> is subscribed to.
        /// Example: First Chance AppDomain Exceptions could fall under here because it's impossible
        /// to assign the risk of this king of exception. Instead the SystemHealth flag will be raised.
        /// </summary>
        NA = 0,
        /// <summary>
        /// Indicates a message that may assist in debugging code and generally
        /// serves no additional purpose. 
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Indicates that the message is informational. No action should be taken
        /// for these type of messages.
        /// </summary>
        Info = 2,
        /// <summary>
        /// Indicates that something happened that might adversely affect the system's operation.
        /// This level can also be used for expected errors. Warnings can be precursors
        /// to errors in the system. 
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Indicates that something happened that might adversely affect the system's operation.
        /// This level should be reserved for errors that are not expected to occur. 
        /// These are non-urgent failures to the system.
        /// </summary>
        Error = 4,
        /// <summary>
        /// Indicates that something happened that will render certain components useless. These
        /// errors can be recovered from. An example case would be one of those 
        /// "this should never happen" errors that were likely not handled properly and thus could
        /// eventually make the system unstable or unusable.
        /// </summary>
        Critical = 5,
    }

    /// <summary>
    /// The level of importance of a message that is being raised. Increasing from Debug, Info, Warning, Error, Critical.
    /// None can be specified, but this indicates an importance level cannot be properly identified, and the message
    /// will be routed based on <see cref="MessageFlags"/>.
    /// </summary>
    [Flags]
    public enum MessageLevelFlags
        : byte
    {
        /// <summary>
        /// Indicates a level that cannot be generally subscribed to. In this case,
        /// the message can only be subscribed to if a <see cref="MessageFlags"/> is subscribed to.
        /// Example: First Chance AppDomain Exceptions could fall under here because it's impossible
        /// to assign the risk of this king of exception. Instead the SystemHealth flag will be raised.
        /// </summary>
        NA = 0,
        /// <summary>
        /// Indicates a message that may assist in debugging code and generally
        /// serves no additional purpose. 
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Indicates that the message is informational. No action should be taken
        /// for these type of messages.
        /// </summary>
        Info = 2,
        /// <summary>
        /// Indicates that something happened that might adversely affect the system's operation.
        /// This level can also be used for expected errors. Warnings can be precursors
        /// to errors in the system. 
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Indicates that something happened that might adversely affect the system's operation.
        /// This level should be reserved for errors that are not expected to occur. 
        /// These are non-urgent failures to the system.
        /// </summary>
        Error = 8,
        /// <summary>
        /// Indicates that something happened that will render certain components useless. These
        /// errors can be recovered from. An example case would be one of those 
        /// "this should never happen" errors that were likely not handled properly and thus could
        /// eventually make the system unstable or unusable.
        /// </summary>
        Critical = 16,
    }
}