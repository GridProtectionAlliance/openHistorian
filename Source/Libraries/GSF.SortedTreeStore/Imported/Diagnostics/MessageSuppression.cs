//******************************************************************************************************
//  MessageSuppression.cs - Gbtc
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
    /// Indicates the suppression level that this message received
    /// </summary>
    public enum MessageSuppression 
        : byte
    {
        /// <summary>
        /// This message did not receive a suppression level.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that the message rate is slightly over normal levels. Only subscribers specifically
        /// asking for this suppression level will receive this message.
        /// </summary>
        Standard = 1,
        /// <summary>
        /// Indicates that the message rate is heavily over normal levels. Only subscribers specifically
        /// asking for this suppression level will receive this message.
        /// </summary>
        Heavy = 2,
        /// <summary>
        /// Indicates that the message rate is severely over normal levels. Only subscribers specifically
        /// asking for this suppression level will receive this message.
        /// </summary>
        Severe = 3
    }

    /// <summary>
    /// Indicates the suppression level that this message received
    /// </summary>
    [Flags]
    internal enum MessageSuppressionFlags
        : byte
    {
        /// <summary>
        /// This message did not receive a suppression level.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that the message rate is slightly over normal levels. Only subscribers specifically
        /// asking for this suppression level will receive this message.
        /// </summary>
        Standard = 1,
        /// <summary>
        /// Indicates that the message rate is heavily over normal levels. Only subscribers specifically
        /// asking for this suppression level will receive this message.
        /// </summary>
        Heavy = 2,
        /// <summary>
        /// Indicates that the message rate is severely over normal levels. Only subscribers specifically
        /// asking for this suppression level will receive this message.
        /// </summary>
        Severe = 4
    }
}