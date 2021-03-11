//******************************************************************************************************
//  ArchiveListEditor.cs - Gbtc
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
//
//******************************************************************************************************

using System;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Provides a way to edit an <see cref="ArchiveList"/> since all edits must be atomic.
    /// WARNING: Instancing this class on an <see cref="ArchiveList"/> will lock the class
    /// until <see cref="Dispose"/> is called. Therefore, keep locks to a minimum and always
    /// use a Using block.
    /// </summary>
    public abstract class ArchiveListEditor
        : IDisposable  
    {
        private bool m_disposed;

        /// <summary>
        /// Renews the snapshot of the archive file. This will acquire the latest 
        /// read transaction so all new snapshots will use this later version.
        /// </summary>
        /// <param name="archiveId">the ID of the archive snapshot to renew</param>
        /// <returns></returns>
        public abstract void RenewArchiveSnapshot(Guid archiveId);

        /// <summary>
        /// Returns true if the archive list contains the provided file.
        /// </summary>
        /// <param name="archiveId">the file</param>
        /// <returns></returns>
        public abstract bool Contains(Guid archiveId);

        /// <summary>
        /// Removes the <see cref="archiveId"/> from <see cref="ArchiveList{TKey,TValue}"/> and queues it for disposal.
        /// </summary>
        /// <param name="archiveId">the archive to remove</param>
        /// <returns>True if the item was removed, False otherwise.</returns>
        /// <remarks>
        /// Also unlocks the archive file.
        /// </remarks>
        public abstract bool TryRemoveAndDispose(Guid archiveId);

        /// <summary>
        /// Removes the supplied file from the <see cref="ArchiveList{TKey,TValue}"/> and queues it for deletion.
        /// </summary>
        /// <param name="archiveId">file to remove and delete.</param>
        /// <returns>true if deleted, false otherwise</returns>
        public abstract bool TryRemoveAndDelete(Guid archiveId);

        /// <summary>
        /// Releases all the resources used by the <see cref="ArchiveListEditor"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ArchiveListEditor"/> object and optionally releases the managed resources.
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
