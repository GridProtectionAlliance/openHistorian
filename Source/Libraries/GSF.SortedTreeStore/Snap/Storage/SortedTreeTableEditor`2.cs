//******************************************************************************************************
//  SortedTreeTableEditor`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.Snap.Tree;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// A single instance editor that is used
    /// to modifiy an archive file.
    /// </summary>
    public abstract class SortedTreeTableEditor<TKey, TValue>
        : IDisposable
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private bool m_disposed;

        /// <summary>
        /// Releases all the resources used by the <see cref="SortedTreeTableEditor{TKey,TValue}"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commits the edits to the current archive file and disposes of this class.
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Rolls back all edits that are made to the archive file and disposes of this class.
        /// </summary>
        public abstract void Rollback();

        /// <summary>
        /// Gets the lower and upper bounds of this tree.
        /// </summary>
        /// <param name="firstKey">The first key in the tree</param>
        /// <param name="lastKey">The final key in the tree</param>
        /// <remarks>
        /// If the tree contains no data. <see cref="firstKey"/> is set to it's maximum value
        /// and <see cref="lastKey"/> is set to it's minimum value.
        /// </remarks>
        public abstract void GetKeyRange(TKey firstKey, TKey lastKey);

        /// <summary>
        /// Adds a single point to the archive file.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">the value</param>
        public abstract void AddPoint(TKey key, TValue value);

        /// <summary>
        /// Adds all of the points to this archive file.
        /// </summary>
        /// <param name="stream"></param>
        public abstract void AddPoints(TreeStream<TKey, TValue> stream);

        /// <summary>
        /// Opens a tree scanner for this archive file
        /// </summary>
        /// <returns></returns>
        public abstract SortedTreeScannerBase<TKey, TValue> GetRange();

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SortedTreeTableEditor{TKey,TValue}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }
    }
}