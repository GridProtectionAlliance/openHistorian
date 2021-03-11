//******************************************************************************************************
//  SubFileDiskIoSessionPool.cs - Gbtc
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
    /// reading and writing to an archive disk. This class contains two I/O Sessions if the file
    /// supports modification to speed up the copy operation when doing shadow copies.
    /// </summary>
    internal class SubFileDiskIoSessionPool
        : IDisposable
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(SubFileDiskIoSessionPool), MessageClass.Component);
        
        /// <summary>
        /// 
        /// </summary>
        public DiskIoSession SourceData;
        /// <summary>
        /// Null if in readonly mode
        /// </summary>
        public DiskIoSession DestinationData;

        /// <summary>
        /// 
        /// </summary>
        public DiskIoSession SourceIndex;
        /// <summary>
        /// Null if in readonly mode
        /// </summary>
        public DiskIoSession DestinationIndex;

        /// <summary>
        /// The file
        /// </summary>
        public SubFileHeader File
        {
            get;
            private set;
        }

        /// <summary>
        /// The Header
        /// </summary>
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

        /// <summary>
        /// Gets if the file is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets if this class has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates this file with the following data.
        /// </summary>
        /// <param name="diskIo"></param>
        /// <param name="header"></param>
        /// <param name="file"></param>
        /// <param name="isReadOnly"></param>
        public SubFileDiskIoSessionPool(DiskIo diskIo, FileHeaderBlock header, SubFileHeader file, bool isReadOnly)
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
            Log.Publish(MessageLevel.Info, "Finalizer Called", GetType().FullName);
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