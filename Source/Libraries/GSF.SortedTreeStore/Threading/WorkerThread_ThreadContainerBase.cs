//******************************************************************************************************
//  WorkerThread_ThreadContainer.cs - Gbtc
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
//  3/8/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// A weak referenced <see cref="Thread"/> that will enter a pause state behind a weak reference
    /// so it can be garbaged collected if it's in a sleep state.
    /// </summary>
    public partial class WorkerThread
    {
        class ThreadContainerArgs
        {
            public WorkerThreadTimeoutResults TimeoutResults;

            public bool ShouldQuit;
            public bool RepeatNow;
            public bool RepeatAfterDelay;
            public int RepeatAfterDelayTime;

            public void Clear()
            {
                ShouldQuit = false;
                RepeatNow = false;
                RepeatAfterDelay = false;
                RepeatAfterDelayTime = -1;
            }
        }

        private abstract class ThreadContainerBase
            : WeakReference
        {

            protected ThreadContainerBase(Action<ThreadContainerArgs> target) 
                : base(target)
            {
            }

            /// <summary>
            /// Attempts to call the weak delegate. 
            /// </summary>
            /// <param name="state">state variables to pass</param>
            /// <returns>True if successful and the delegate returned true. False if the code needs to exit</returns>
            /// <remarks>
            /// This method must not be inlined because a strong reference to this weak reference exists in this function. 
            /// Inlining would put the strong reference in the sleeping method, preventing collection.
            /// </remarks>
            [MethodImpl(MethodImplOptions.NoInlining)]
            protected void TryInvoke(ThreadContainerArgs state)
            {
                Action<ThreadContainerArgs> callback = (Action<ThreadContainerArgs>)Target;
                if (callback != null)
                {
                    callback(state);
                }
                else
                {
                    state.ShouldQuit = true;
                }
            }

            public abstract void Dispose();
            public abstract void Reset();
            public abstract void StartLater(int delay);
            public abstract void CancelTimer();
            public abstract void StartNow();
        }
        
    }
}
