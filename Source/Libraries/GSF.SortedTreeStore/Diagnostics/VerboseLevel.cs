//******************************************************************************************************
//  VerboseLevel.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// A level of verbose for certain error messages.
    /// </summary>
    [Flags]
    public enum VerboseLevel : int
    {
        /// <summary>
        /// No messages should be reported. 
        /// </summary>
        /// <remarks>
        /// Use this only to unsubscribe from all system messages.
        /// Specifying this while creating a message will cause it not to be routed.
        /// </remarks>
        None = 0,
        /// <summary>
        /// Indicates a message that may assist in debugging code and generally
        /// serves no additional purpose. Indicates Low Priority Debug Message. 
        /// Any message that can occur at a very high rate should be classified as this
        /// or a message that is generally useless except for step-by-step debugging
        /// should go here.
        /// </summary>
        DebugLow = 1 << 0,
        /// <summary>
        /// Indicates a message that may assist in debugging code and generally
        /// serves no additional purpose. A normal priority message should occur no more
        /// than a few times per second, and have value showing the general flow of the code.
        /// </summary>
        DebugNormal = 1 << 1,
        /// <summary>
        /// Indicates a message that may assist in debugging code and generally
        /// serves no additional purpose. A high priority debug message generally
        /// will occur no more than once a second and has high value for the debugger
        /// </summary>
        DebugHigh = 1 << 2,

        /// <summary>
        /// Gets all messages that are not debug messages.
        /// </summary>
        NonDebug = All ^ DebugLow ^ DebugNormal ^ DebugHigh,

        /// <summary>
        /// Indicates that the message is informational. It has more useful information
        /// than a debug message, but for the most part is indicating basic status.
        /// </summary>
        Information = 1 << 3,
        /// <summary>
        /// Indicates that something happened that might adversely effect the system's operation.
        /// This level can also be used for expected errors.
        /// </summary>
        Warning = 1 << 4,
        /// <summary>
        /// Indicates that something happended that might adversely effect the system's operation.
        /// This level should be reserved for errors that are not expected to occur. 
        /// </summary>
        Error = 1 << 5,
        /// <summary>
        /// Indicates that something happened that will render certain components useless. These
        /// errors can be recovered from. An example case would be one of those 
        /// "this should never happen" errors that were likely not handled properly and thus
        /// leak system resources.
        /// </summary>
        Critical = 1 << 6,
        /// <summary>
        /// Indicates a error has a high likelyhood to compromise the state of the system.  
        /// When these errors occur, it may be recommended to terminate or restart the program.
        /// </summary>
        Fatal = 1 << 7,
        /// <summary>
        /// Indicates that this message should be reported back to the developer. This should be used
        /// if some area of code is entered, or some unhandled exception occurs that the developer
        /// should be made known about, however, it is expected that this will not adversely effect
        /// the system operation. It will give the developer a chance to closely review code that 
        /// may have not responded  as expected to a race condition for instance.
        /// </summary>
        BugReport = 1 << 8,
        /// <summary>
        /// Indicates that an area of code has been entered that reflects a performance related impact
        /// is occurring. This information could be helpful to the developer when diagnosing slow 
        /// code.
        /// </summary>
        PerformanceIssue = 1 << 9,

        /// <summary>
        /// A flag that specifies that all levels will be listened to.  This is an invalid flag to 
        /// assign to a message.
        /// </summary>
        All = -1
    }
}