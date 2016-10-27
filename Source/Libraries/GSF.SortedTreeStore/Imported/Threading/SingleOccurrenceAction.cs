//******************************************************************************************************
//  SingleOccurrenceAction.cs - Gbtc
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
using System.Threading;

#pragma warning disable 420

namespace GSF.Threading
{
    /// <summary>
    /// A helper class that will properly coordinate any number of <see cref="Action"/>s
    /// from different threads.
    /// </summary>
    /// <remarks>
    /// This class is commonly used to properly coordinate the disposal of object.
    /// 
    /// Remarks: Adapted from GSF.Threading.ReaderWriterSpinLock
    /// </remarks>
    public class SingleOccurrenceAction
    {
        #region [ Members ]

        static class State
        {
            /// <summary>
            /// Means that no attempt to signal this class has been made.
            /// </summary>
            public const int Unsignaled = 0;
            /// <summary>
            /// Means that a <see cref="SingleOccurrenceAction.ExecuteWithoutWait"/> was called and the 
            /// current state variables are invalid. Spin and wait for a valid state.
            /// </summary>
            public const int Invalid = 1;
            /// <summary>
            /// Means that <see cref="SingleOccurrenceAction.ExecuteAndWait"/> was called and the callback
            /// will be called by this thread once all blockers have finished.
            /// </summary>
            public const int RunCallbackOnSignalThread = 2;
            /// <summary>
            /// Means that <see cref="SingleOccurrenceAction.ExecuteWithoutWait"/> was called and
            /// if there were any blockers, the last blocker will execute the callback.
            /// Otherwise, the signaling thread will execute the callback
            /// </summary>
            public const int RunCallbackByBlockerThread = 3;
            /// <summary>
            /// Indicates that the callback is executing. Only a valid state when
            /// SignalWithoutWait was called.
            /// </summary>
            public const int RunningCallback = 5;
            /// <summary>
            /// The completed signaled state.
            /// </summary>
            public const int Signaled = 6;
        }

        // Fields
        private StateMachine m_state = new StateMachine(State.Unsignaled);
        private volatile int m_blockerCount = 0;
        private volatile Action m_callback;
        private volatile bool m_isCallbackPendingOnBlocks = false;
        private volatile int m_callbackThreadId = -1;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the signal action is currently pending. 
        /// This means all future calls to <see cref="TryBlockAction"/> will fail.
        /// </summary>
        public bool IsCallbackPendingOnBlocks
        {
            get
            {
                return m_isCallbackPendingOnBlocks;
            }
        }


        /// <summary>
        /// Gets if the callback is executing
        /// </summary>
        public bool IsCallbackExecuting { get; private set; }

        /// <summary>
        /// Gets if all actions associated with the signaling of this class
        /// is complete.
        /// </summary>
        public bool IsSignalingCompleted { get; private set; }

        #endregion

        #region [ Methods ]


        /// <summary>
        /// Signals this class and waits for all blockers to exit before calling the <see cref="callback"/>..
        /// If called concurrently, the calling thread blocks until the <see cref="callback"/> completes.
        /// If called from within the <see cref="callback"/>, the function returns immediately.
        /// 
        /// The same thread calling this method will always call the <see cref="callback"/>.
        /// 
        /// Only one callback will be executed by this class and it will be the first one to call 
        /// either <see cref="ExecuteWithoutWait"/> or <see cref="ExecuteAndWait"/>.
        /// </summary>
        /// <param name="callback">the callback to execute</param>
        /// <returns></returns>
        public void ExecuteAndWait(Action callback = null)
        {
            m_isCallbackPendingOnBlocks = true;
            SpinWait sw = new SpinWait();
            if (m_state.TryChangeState(State.Unsignaled, State.RunCallbackOnSignalThread))
            {
                // We now prevent any additional blocks from occuring in this class
                // Wait for all blockers to exit.
                while (m_blockerCount != 0) //A race condition that exists here is handled in TryEnterSignalBlock class.
                {
                    sw.SpinOnce();
                }

                m_callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                IsCallbackExecuting = true;
                if (callback != null)
                    callback();
                IsSignalingCompleted = true;
                m_state.SetState(State.Signaled);
                return;
            }
            if (Thread.CurrentThread.ManagedThreadId == m_callbackThreadId)
                return;
            while (m_state != State.Signaled)
            {
                sw.SpinOnce();
            }
        }


        /// <summary>
        /// Registers this callback to occur once all blocking call complete.
        /// This function will return immediately and the callback may or may not be run.
        /// 
        /// Only one callback will be executed by this class and it will be the first one to call 
        /// either <see cref="ExecuteWithoutWait"/> or <see cref="ExecuteAndWait"/>.
        /// </summary>
        /// <param name="callback">the callback to execute</param>
        public void ExecuteWithoutWait(Action callback = null)
        {
            m_isCallbackPendingOnBlocks = true;
            if (m_state.TryChangeState(State.Unsignaled, State.Invalid))
            {
                m_callback = callback;
                m_state.SetState(State.RunCallbackByBlockerThread);

                if (m_blockerCount == 0)
                {
                    RaceForCallback();
                }
            }
        }

        /// <summary>
        /// Attempts to block this class from being signaled. Be sure to call within a using block.
        /// </summary>
        /// <param name="success">A bool to notify if entering the block was a success</param>
        public DisposableCallback TryBlockAction(out bool success)
        {
            if (!m_isCallbackPendingOnBlocks)
            {
                Interlocked.Increment(ref m_blockerCount);
                if (!m_isCallbackPendingOnBlocks)
                {
                    success = true;
                    return new DisposableCallback(ExitSignalBlock);
                }
                ExitSignalBlock();
            }
            success = false;
            return default(DisposableCallback);
        }

        /// <summary>
        /// Attempts to block this class from being signaled. 
        /// </summary>
        /// <param name="executeIfSuccessful">A callback to execute if the block was successful</param>
        /// <returns>
        /// True if the method was executed, false otherwise.
        /// </returns>
        public bool TryBlockAction(Action executeIfSuccessful)
        {
            bool success;
            using (TryBlockAction(out success))
            {
                if (success)
                {
                    executeIfSuccessful();
                }
                return success;
            }
        }

        /// <summary>
        /// Attempts to block this class from being signaled. 
        /// </summary>
        /// <param name="executeIfSuccessful">A callback to execute if the block was successful</param>
        /// <param name="executeIfFailed">A callback to execute if the block was unsuccessful</param>
        public void BlockAction(Action executeIfSuccessful, Action executeIfFailed)
        {
            bool success;
            using (TryBlockAction(out success))
            {
                if (success)
                {
                    executeIfSuccessful();
                }
                else
                {
                    executeIfFailed();
                }
            }
        }

        /// <summary>
        /// Attempts to block this class from being signaled. 
        /// </summary>
        /// <param name="executeIfSuccessful">A callback to execute if the block was successful</param>
        /// <param name="executeIfFailed">A callback to execute if the block was unsuccessful</param>
        public T BlockAction<T>(Func<T> executeIfSuccessful, Func<T> executeIfFailed)
        {
            bool success;
            using (TryBlockAction(out success))
            {
                if (success)
                {
                    return executeIfSuccessful();
                }
                else
                {
                    return executeIfFailed();
                }
            }
        }

        /// <summary>
        /// Exits the block on the reader.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot exit read lock when there are no readers.</exception>
        void ExitSignalBlock()
        {
            //If I'm the last blocker and there is a signal pending, I need to do better coordination.
            if (Interlocked.Decrement(ref m_blockerCount) == 0 && m_isCallbackPendingOnBlocks)
            {
                RaceForCallback();
            }
        }

        void RaceForCallback()
        {
            SpinWait sw = new SpinWait();
            while (true)
            {
                switch (m_state)
                {
                    case State.Unsignaled:
                    case State.Invalid:
                        break;
                    case State.RunCallbackByBlockerThread:
                        if (m_state.TryChangeState(State.RunCallbackByBlockerThread, State.RunningCallback))
                        {
                            m_callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                            IsCallbackExecuting = true;
                            if (m_callback != null)
                                m_callback();
                            m_callback = null;
                            IsSignalingCompleted = true;
                            m_state.SetState(State.Signaled);
                            return;
                        }
                        break;
                    case State.RunningCallback:
                    case State.Signaled:
                    case State.RunCallbackOnSignalThread:
                        return;
                    default:
                        throw new Exception("Unknown State");
                }
                sw.SpinOnce();
            }
        }

        #endregion
    }
}

