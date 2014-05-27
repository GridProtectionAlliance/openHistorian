//******************************************************************************************************
//  SubFileDiskIoSessionPool.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  2/21/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Diagnostics;
using GSF.IO.FileStructure.Media;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// Contains a set of <see cref="DiskIoSession"/>s that speed up the I/O operations associated with
    /// reading and writing to an archive disk.
    /// </summary>
    internal class SubFileDiskIoSessionPool
        : IDisposable
    {
        
        public DiskIoSession SourceData;
        /// <summary>
        /// Null if in readonly mode
        /// </summary>
        public DiskIoSession DestinationData;
        public DiskIoSession SourceIndex;
        /// <summary>
        /// Null if in readonly mode
        /// </summary>
        public DiskIoSession DestinationIndex;

        public SubFileMetaData File
        {
            get;
            private set;
        }

        public FileHeaderBlock Header
        {
            get;
            private set;
        }

        /// <summary>
        /// Contains the last block that is considered as read only. This is the same as the end of the committed space
        /// in the transactional file system.
        /// </summary>
        public uint LastReadonlyBlock
        {
            get;
            private set;
        }

        public bool IsReadOnly
        {
            get;
            private set;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public SubFileDiskIoSessionPool(DiskIo diskIo, FileHeaderBlock header, SubFileMetaData file, bool isReadOnly)
        {
            LastReadonlyBlock = diskIo.LastCommittedHeader.LastAllocatedBlock;
            File = file;
            Header = header;
            IsReadOnly = isReadOnly;
            SourceData = diskIo.CreateDiskIoSession(header, file);
            SourceIndex = diskIo.CreateDiskIoSession(header, file);
            if (!isReadOnly)
            {
                DestinationData = diskIo.CreateDiskIoSession(header, file);
                DestinationIndex = diskIo.CreateDiskIoSession(header, file);
            }
        }

#if DEBUG
        ~SubFileDiskIoSessionPool()
        {
            Logger.Default.UniversalReporter.LogMessage(VerboseLevel.Information, "Finalizer Called", GetType().FullName);
        }
#endif

        /// <summary>
        /// Swaps the source and destination index I/O Sessions.
        /// </summary>
        public void SwapIndex()
        {
            if (IsReadOnly)
                throw new NotSupportedException("Not supported when in read only mode");
            DiskIoSession swap = SourceIndex;
            SourceIndex = DestinationIndex;
            DestinationIndex = swap;
        }

        /// <summary>
        /// Swaps the source and destination Data I/O Sessions
        /// </summary>
        public void SwapData()
        {
            if (IsReadOnly)
                throw new NotSupportedException("Not supported when in read only mode");
            DiskIoSession swap = SourceData;
            SourceData = DestinationData;
            DestinationData = swap;
        }

        /// <summary>
        /// Releases all of the data associated with the I/O Sessions.
        /// </summary>
        public void Clear()
        {
            if (SourceData != null)
                SourceData.Clear();
            if (DestinationData != null)
                DestinationData.Clear();
            if (SourceIndex != null)
                SourceIndex.Clear();
            if (DestinationIndex != null)
                DestinationIndex.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            IsDisposed = true;
            if (SourceData != null)
            {
                SourceData.Dispose();
                SourceData = null;
            }
            if (DestinationData != null)
            {
                DestinationData.Dispose();
                DestinationData = null;
            }
            if (SourceIndex != null)
            {
                SourceIndex.Dispose();
                SourceIndex = null;
            }
            if (DestinationIndex != null)
            {
                DestinationIndex.Dispose();
                DestinationIndex = null;
            }
        }
    }
}