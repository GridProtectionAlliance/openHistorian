//******************************************************************************************************
//  PageLock.cs - Gbtc
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
//  2/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.FileStructure.IO
{
    /// <summary>
    /// Used to hold a lock on a page to prevent it from being collected by the collection engine.
    /// </summary>
    internal class PageLock
    {
        private const int ConstIsCleared = -2;
        private const int ConstIsDisposed = -3;

        private int m_currentBlock;

        /// <summary>
        /// Creates an unallocated block.
        /// </summary>
        public PageLock()
        {
            m_currentBlock = ConstIsDisposed;
        }

        /// <summary>
        /// Gets the block that has a lock on it. 
        /// Returns a -1 if no blocks are locked.
        /// </summary>
        public int CurrentBlock
        {
            get
            {
                return Math.Max(m_currentBlock, -1);
            }
        }

        /// <summary>
        /// Gets if the lock has been disposed by the session.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_currentBlock == ConstIsDisposed;
            }
        }

        /// <summary>
        /// Acquires a lock. This should be called in a 
        /// synchronized environment.
        /// </summary>
        /// <param name="block"></param>
        public void SetActiveBlock(int block)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (block < 0)
                throw new ArgumentOutOfRangeException("block", "Cannot be a negative number");
            m_currentBlock = block;
        }

        /// <summary>
        /// Releases a lock, May be call unsynchronized.
        /// </summary>
        public void Clear()
        {
            if (IsDisposed)
                throw new Exception("Block has been released and cannot be cleared.");
            m_currentBlock = ConstIsCleared;
        }

        /// <summary>
        /// Disposes a lock. This lock can be immediately reused by another session. Therefore
        /// set the reference to null immediately after calling this function. Otherwise this
        /// will create a bug that will be very difficult to find.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                throw new Exception("Block cannot be released more than once.");
            m_currentBlock = ConstIsDisposed;
        }

        /// <summary>
        /// Prepares this lock to be used by another session. 
        /// Only to be called by <see cref="PageReplacementAlgorithm"/>.
        /// </summary>
        public void ResurrectLock()
        {
            if (!IsDisposed)
                throw new Exception("Block is currently in use");
            m_currentBlock = ConstIsCleared;
        }
    }
}