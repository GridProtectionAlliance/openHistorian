//******************************************************************************************************
//  MemoryStream.cs - Gbtc
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
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using GSF.UnmanagedMemory;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    public partial class MemoryStream : ISupportsBinaryStream
    {
        #region [ Members ]

        MemoryStreamCore m_core;

        /// <summary>
        /// The size of each page.
        /// </summary>
        int m_blockSize;

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryStream"/> object.
        /// </summary>
        bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MemoryStream"/> using the default <see cref="BufferPool"/>.
        /// </summary>
        public MemoryStream()
            : this(Globals.BufferPool)
        {
        }

        /// <summary>
        /// Create a new <see cref="MemoryStream"/>
        /// </summary>
        public MemoryStream(BufferPool pool)
        {
            m_core = new MemoryStreamCore(pool);
            m_blockSize = pool.PageSize;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the unit size of an individual block
        /// </summary>
        public int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

        /// <summary>
        /// Gets if the stream can be written to.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets if the stream has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        public long Length
        {
            get
            {
                return m_core.Length;
            }
        }

        /// <summary>
        /// Gets the number of available simultaneous read/write sessions.
        /// </summary>
        /// <remarks>This value is used to determine if a binary stream can be cloned
        /// to improve read/write/copy performance.</remarks>
        int ISupportsBinaryStream.RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    m_core.Dispose();
                }
                finally
                {
                    m_core = null;
                    m_disposed = true;
                }
            }
        }

        /// <summary>
        /// Aquire an IO Session.
        /// </summary>
        public IBinaryStreamIoSession GetNextIoSession()
        {
            return new IoSession(this);
        }

        /// <summary>
        /// Creates a new binary from an IO session
        /// </summary>
        /// <returns></returns>
        public BinaryStreamBase CreateBinaryStream()
        {
            return new BinaryStream(this);
        }

        #endregion

        #region [ Helper Methods ]

        void GetBlock(long position, out IntPtr firstPointer, out long firstPosition, out int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");
            m_core.GetBlock(position, out firstPointer, out firstPosition, out length);
        }

        #endregion

    }
}
