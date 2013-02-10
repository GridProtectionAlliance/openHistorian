//******************************************************************************************************
//  DiskMediumBase.cs - Gbtc
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
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.IO.Unmanaged;

namespace openHistorian.FileStructure.IO
{
    internal abstract class DiskMediumBase : ISupportsBinaryStream, IDisposable
    {
        FileHeaderBlock m_header;
        
        protected DiskMediumBase(int diskBlockSize, int fileStructureBlockSize)
        {
            if (diskBlockSize <= 0 || diskBlockSize % fileStructureBlockSize != 0)
                throw new ArgumentException("The block size of the stream must be a multiple of " + fileStructureBlockSize.ToString() + ".", "stream");
            FileStructureBlockSize = fileStructureBlockSize;
            DiskBlockSize = diskBlockSize;
        }

        protected void Initialize(FileHeaderBlock header)
        {
            m_header = header;
        }

        public int RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Gets if the stream can be written to.
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Gets if the stream has been disposed.
        /// </summary>
        public abstract bool IsDisposed { get; }

        public BinaryStreamBase CreateBinaryStream()
        {
            return new BinaryStream(this);
        }

        /// <summary>
        /// Aquire an IO Session.
        /// </summary>
        public abstract IBinaryStreamIoSession GetNextIoSession();

        public void CommitChanges(FileHeaderBlock header)
        {
            header.IsReadOnly = true;
            FlushWithHeader(header);
            m_header = header;
        }

        public FileHeaderBlock Header
        {
            get
            {
                return m_header;
            }
        }

        public int FileStructureBlockSize { get; private set; }

        /// <summary>
        /// Gets the unit size of an individual block
        /// </summary>
        public int DiskBlockSize { get; private set; }

        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// Writes all current data to the disk subsystem.
        /// </summary>
        protected abstract void FlushWithHeader(FileHeaderBlock headerBlock);

        /// <summary>
        /// Reverts to a state where data was not changed.
        /// </summary>
        public abstract void RollbackChanges();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}
