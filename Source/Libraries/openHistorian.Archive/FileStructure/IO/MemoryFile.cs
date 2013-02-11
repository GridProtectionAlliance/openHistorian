//******************************************************************************************************
//  MemoryFile.cs - Gbtc
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
using System.Collections.Generic;
using System.Threading;
using GSF.IO.Unmanaged;
using GSF.UnmanagedMemory;

namespace openHistorian.FileStructure.IO
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    internal partial class MemoryFile : DiskMediumBase
    {
        #region [ Members ]

        MemoryStreamCore m_core;

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryFile"/> object.
        /// </summary>
        bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MemoryFile"/> using the default <see cref="BufferPool"/>.
        /// </summary>
        public MemoryFile(int blockSize)
            : this(GSF.Globals.BufferPool, blockSize)
        {
        }

        /// <summary>
        /// Create a new <see cref="MemoryFile"/>
        /// </summary>
        public MemoryFile(BufferPool pool, int fileStructureBlockSize)
            : base(pool.PageSize, fileStructureBlockSize)
        {
            m_core = new MemoryStreamCore(pool);
            Initialize(FileHeaderBlock.CreateNew(fileStructureBlockSize));
        }
 
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the stream can be written to.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets if the stream has been disposed.
        /// </summary>
        public override bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return m_core.Length;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
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
        public override IBinaryStreamIoSession GetNextIoSession()
        {
            return new IoSession(this);
        }

        protected override void FlushWithHeader(FileHeaderBlock headerBlock)
        {

        }
        public override void RollbackChanges()
        {

        }

        #endregion

        #region [ Helper Methods ]

        void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");
            supportsWriting = true;

            m_core.GetBlock(position, out firstPointer, out firstPosition, out length);
            //firstPosition = position;
            //m_core.ReadBlock(position,out firstPointer, out length);
            //length = FileStructureBlockSize;
        }

        #endregion

    }
}
