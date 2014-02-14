//******************************************************************************************************
//  CustomThreadBase.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Threading
{
    /// <summary>
    /// Functions that might be executed by <see cref="ScheduledTask"/>. 
    /// This allows for multiple types of threads to be created, such as one that uses
    /// the threadpool, or one that uses a dedicated thread that is a foreground thread.
    /// 
    /// All calls to this class need to be properly coordinated.
    /// </summary>
    internal abstract class CustomThreadBase 
        : IDisposable
    {
        /// <summary>
        /// Requests that the callback executes immediately.
        /// </summary>
        public abstract void StartNow();
       
        /// <summary>
        /// Requests that the callback executes after the specified interval in milliseconds.
        /// </summary>
        /// <param name="delay">the delay in milliseconds</param>
        public abstract void StartLater(int delay);

        /// <summary>
        /// Requests that a previous delay be canceled and the callback be executed immediately
        /// </summary>
        public abstract void ShortCircuitDelayRequest();

        /// <summary>
        /// A reset will return the thread to a non-executing/ready state.
        /// </summary>
        public abstract void ResetTimer();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}