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
using openHistorian.V2.Unmanaged;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// A page replacement algorithm that utilizes a quasi LRU algorithm.
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="BufferedFileStream"/> to decide which pages should be replaced.
    /// </remarks>
    public partial class LeastRecentlyUsedPageReplacement : IDisposable
    {
        /// <summary>
        /// This class is used to keep track of the pages that are currently referenced.
        /// </summary>
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

            public SubPageMetaData TryGetSubPageOrCreateNew(long position, bool isWriting, Action<IntPtr, long> delLoadFromFile)
            {
                SubPageMetaData subPage;
                if (m_lru.TryGetSubPage(position,IoSessionId,isWriting,out subPage))
                {
                    return subPage;
                }
                return m_lru.CreateNewSubPage(position, IoSessionId, isWriting, delLoadFromFile);
            }

            public void Clear()
            {
                m_lru.ClearIoSession(IoSessionId);
            }

            public void Dispose()
            {
                if (!m_disposed)
                {
                    try
                    {
                        if (!m_lru.IsDisposed)
                        {
                            m_lru.ReleaseIoSession(IoSessionId);
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
