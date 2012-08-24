//******************************************************************************************************
//  LeastRecentlyUsedPageReplacement.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Unmanaged;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// A page replacement algorithm that utilizes a quasi LRU algorithm.
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="BufferedFileStream"/> to decide which pages should be replaced.
    /// </remarks>
    unsafe public partial class LeastRecentlyUsedPageReplacement : IDisposable
    {
        // Nested Types
        public class IoSession : IDisposable
        {
            LeastRecentlyUsedPageReplacement m_lru;
            bool m_disposed;
            public readonly int IoSessionId;

            public IoSession(LeastRecentlyUsedPageReplacement lru, int ioSessionId)
            {
                m_lru = lru;
                IoSessionId = ioSessionId;
            }

            /// <summary>
            /// Releases the unmanaged resources before the <see cref="IoSession"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~IoSession()
            {
                Dispose(false);
            }

            public SubPageMetaData TryGetSubPageOrCreateNew(long position, bool isWriting, Action<IntPtr, long> delLoadFromFile)
            {
                return m_lru.TryGetSubPageOrCreateNew(position, IoSessionId, isWriting, delLoadFromFile);
            }

            public void Clear()
            {
                m_lru.ClearIoSession(IoSessionId);
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="IoSession"/> object and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.
                        if (!m_lru.IsDisposed)
                        {
                            m_lru.ReleaseIoSession(IoSessionId);
                        }
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
}
