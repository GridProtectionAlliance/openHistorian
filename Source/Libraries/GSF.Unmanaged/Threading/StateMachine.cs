//******************************************************************************************************
//  StateMachine.cs - Gbtc
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
//  1/13/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Helps facilitate a multithreaded state machine.
    /// </summary>
    public class StateMachine
    {
        private int m_state;

        public StateMachine(int initialState)
        {
            m_state = initialState;
        }

        /// <summary>
        /// Attempts to change the state of this machine from <see cref="prevState"/> to <see cref="nextState"/>.
        /// </summary>
        /// <param name="prevState">The state to change from</param>
        /// <param name="nextState">The state to change to</param>
        /// <returns>True if the state changed from the previous state to the next state. 
        /// False if unsuccessful.</returns>
        public bool TryChangeState(int prevState, int nextState)
        {
            bool success;
            Thread.MemoryBarrier();
            success = (m_state == prevState && Interlocked.CompareExchange(ref m_state, nextState, prevState) == prevState);
            Thread.MemoryBarrier();
            return success;
        }

        /// <summary>
        /// Attempts to change the state of this machine from any one of the previous states to <see cref="nextState"/>.
        /// </summary>
        /// <param name="prevState1">The state to check first</param>
        /// <param name="prevState2">The state to check Second</param>
        /// <param name="nextState">The state to change to</param>
        /// <returns>True if the state changed from the previous state to the next state. 
        /// False if unsuccessful.</returns>
        /// <remarks>The order here matters. Make sure to list the previous states in order so if the state happens to progress 
        /// in the middle of this operation, it can be caught by the later operation.</remarks>
        public bool TryChangeStates(int prevState1, int prevState2, int nextState)
        {
            return TryChangeState(prevState1, nextState)
                   || TryChangeState(prevState2, nextState);
        }

        /// <summary>
        /// Gets the current state of the State Machine. Provides a full memory fence.
        /// </summary>
        public int State
        {
            get
            {
                Thread.MemoryBarrier();
                int value = m_state;
                Thread.MemoryBarrier();
                return value;
            }
        }

        /// <summary>
        /// Sets the value of the state. Also comes with a full memory fence.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(int state)
        {
            Thread.MemoryBarrier();
            m_state = state;
            Thread.MemoryBarrier();
        }

        public static implicit operator int(StateMachine machine)
        {
            return machine.State;
        }
    }
}