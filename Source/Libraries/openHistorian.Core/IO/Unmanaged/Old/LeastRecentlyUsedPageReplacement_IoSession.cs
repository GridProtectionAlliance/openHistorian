////******************************************************************************************************
////  LeastRecentlyUsedPageReplacement_IoSession.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  4/18/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;

//namespace GSF.IO.Unmanaged
//{
//    /// <summary>
//    /// A page replacement algorithm that utilizes a quasi LRU algorithm.
//    /// </summary>
//    /// <remarks>
//    /// This class is used by <see cref="BufferedFileStream"/> to decide which pages should be replaced.
//    /// </remarks>
//    public partial class LeastRecentlyUsedPageReplacement : IDisposable
//    {
//        /// <summary>
//        /// This class is used to keep track of the pages that are currently referenced.
//        /// </summary>
//        public class IoSession : IDisposable
//        {
//            private readonly LeastRecentlyUsedPageReplacement m_lru;

//            private bool m_disposed;

//            /// <summary>
//            /// The ID associated with this IoSession
//            /// </summary>
//            private readonly int m_ioSessionId;

//            /// <summary>
//            /// Creates a new instance of <see cref="IoSession"/>.
//            /// </summary>
//            /// <param name="lru">the base class.</param>
//            /// <param name="ioSessionId">the ID value assigned by <see cref="lru"/>.</param>
//            internal IoSession(LeastRecentlyUsedPageReplacement lru, int ioSessionId)
//            {
//                m_lru = lru;
//                m_ioSessionId = ioSessionId;
//            }

//            /// <summary>
//            /// Attempts to get a sub page. 
//            /// </summary>
//            /// <param name="position">the absolute position in the stream to get the page for.</param>
//            /// <param name="isWriting">a bool indicating if this individual page will be written to.</param>
//            /// <param name="subPage">an output field of the resulting sub page</param>
//            /// <returns>False if the page does not exists and needs to be added.</returns>
//            public bool TryGetSubPage(long position, bool isWriting, out SubPageMetaData subPage)
//            {
//                return m_lru.TryGetSubPage(position, m_ioSessionId, isWriting, out subPage);
//            }

//            /// <summary>
//            /// Adds a new page to the list of available pages unless it alread exists.
//            /// NOTE: The page added must be the entire page and cannot be a subset.
//            /// I.E. Equal to the buffer pool size.
//            /// </summary>
//            /// <param name="position">The position of the first byte in the page</param>
//            /// <param name="data">the data to be copied to the internal buffer</param>
//            /// <param name="startIndex">the starting index of <see cref="data"/> to copy</param>
//            /// <param name="length">the length to copy, must be equal to the buffer pool page size</param>
//            /// <returns>True if the page was sucessfully added. False if it already exists and was not added.</returns>
//            public bool TryAddNewPage(long position, byte[] data, int startIndex, int length)
//            {
//                return m_lru.TryAddNewPage(position, data, startIndex, length);
//            }

//            /// <summary>
//            /// De-references the current IoSession's page.
//            /// </summary>
//            public void Clear()
//            {
//                m_lru.ClearIoSession(m_ioSessionId);
//            }

//            /// <summary>
//            /// Removes the current IoSession from the list of available session IDs
//            /// This is done on a dispose operation.
//            /// </summary>
//            public void Dispose()
//            {
//                if (!m_disposed)
//                {
//                    try
//                    {
//                        if (!m_lru.IsDisposed)
//                        {
//                            m_lru.ReleaseIoSession(m_ioSessionId);
//                        }
//                    }
//                    finally
//                    {
//                        m_disposed = true; // Prevent duplicate dispose.
//                    }
//                }
//            }
//        }
//    }
//}