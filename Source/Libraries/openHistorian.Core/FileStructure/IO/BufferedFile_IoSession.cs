//******************************************************************************************************
//  BufferedFile_IoStream.cs - Gbtc
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
//  2/1/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;

namespace openHistorian.FileStructure.IO
{
    internal partial class BufferedFile
    {
        /// <summary>
        /// The <see cref="BinaryStreamIoSessionBase"/> utilized by the <see cref="BufferedFile"/>.
        /// </summary>
        private class IoSession 
            : BinaryStreamIoSessionBase
        {
            /// <summary>
            /// The base stream
            /// </summary>
            private readonly BufferedFile m_stream;

            /// <summary>
            /// The lock is used when doing I/O.
            /// </summary>
            private PageLock m_pageLock;

            /// <summary>
            /// Creates a new <see cref="IoSession"/>
            /// </summary>
            /// <param name="stream">the base class</param>
            /// <param name="pageLock">The LRU IO Session</param>
            internal IoSession(BufferedFile stream, PageLock pageLock)
            {
                m_stream = stream;
                m_pageLock = pageLock;
            }

            /// <summary>
            /// Releases the unmanaged resources before the <see cref="IoSession"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~IoSession()
            {
                Dispose(false);
            }

            /// <summary>
            /// Gets a block for the following Io session.
            /// </summary>
            /// <param name="args">the <see cref="BlockArguments"/> to use to read and write to a block</param>
            public override void GetBlock(BlockArguments args)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);
                m_stream.GetBlock(m_pageLock, args);
            }

            /// <summary>
            /// Sets the current usage of the <see cref="BinaryStreamIoSessionBase"/> to null.
            /// </summary>
            public override void Clear()
            {
                m_pageLock.Clear();
                base.Clear();
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public override void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                base.Dispose();
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="IoSession"/> object and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            protected void Dispose(bool disposing)
            {
                if (!IsDisposed)
                {
                    try
                    {
                        m_pageLock.Dispose();
                    }
                    finally
                    {
                        m_pageLock = null;
                        IsDisposed = true;
                    }
                }
            }
        }
    }
}