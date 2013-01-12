////******************************************************************************************************
////  DiskIOMemoryStream.cs - Gbtc
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
////  1/1/2012 - Steven E. Chisholm
////       Generated original version of source code.
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;

//namespace openHistorian.Core.StorageSystem.File
//{
//    /// <summary>
//    /// Provides a completely in memory implemention of a <see cref="DiskIoBase"/>.
//    /// </summary>
//    internal class DiskIoMemoryStream : DiskIoBase
//    {
//        /// <summary>
//        /// Contains all of the blocks of data.
//        /// </summary>
//        protected List<byte[]> DataBytes;


//        /// <summary>
//        /// Creates an empty <see cref="DiskIoMemoryStream"/>.
//        /// </summary>
//        public DiskIoMemoryStream()
//        {
//            DataBytes = new List<byte[]>();
//        }

//        /// <summary>
//        /// Always returns false.
//        /// </summary>
//        public override bool IsReadOnly
//        {
//            get { return false; }
//        }

//        /// <summary>
//        /// Resizes the file to the requested size
//        /// </summary>
//        /// <param name="requestedSize">The size to resize to</param>
//        /// <returns>The actual size of the file after the resize</returns>
//        protected override long SetFileLength(long requestedSize)
//        {
//            while (FileSize < requestedSize)
//            {
//                DataBytes.Add(null);
//            }
//            while (FileSize > requestedSize)
//            {
//                DataBytes.RemoveAt(DataBytes.Count - 1);
//            }
//            return FileSize;
//        }

//        /// <summary>
//        /// Gets the current size of the file.
//        /// </summary>
//        public override long FileSize
//        {
//            get { return DataBytes.Count * ArchiveConstants.BlockSize; }
//        }

//        /// <summary>
//        /// Writes the following data to the stream
//        /// </summary>
//        /// <param name="blockIndex">the block where to write the data</param>
//        /// <param name="data">the data to write</param>
//        protected override void WriteBlock(uint blockIndex, byte[] data)
//        {
//            while (blockIndex >= DataBytes.Count)
//            {
//                DataBytes.Add(null);
//            }
//            byte[] localPage = DataBytes[(int)blockIndex];
//            if (localPage == null)
//            {
//                localPage = new byte[ArchiveConstants.BlockSize];
//                DataBytes[(int)blockIndex] = localPage;
//            }
//            Array.Copy(data, 0, localPage, 0, data.Length);
//        }

//        /// <summary>
//        /// Tries to read data from the following file
//        /// </summary>
//        /// <param name="blockIndex">the block where to write the data</param>
//        /// <param name="data">the data to write</param>
//        /// <returns>A status whether the read was sucessful. See <see cref="IoReadState"/>.</returns>
//        protected override IoReadState ReadBlock(uint blockIndex, byte[] data)
//        {
//            if (blockIndex > DataBytes.Count)
//                return IoReadState.ReadPastThenEndOfTheFile;

//            byte[] localPage = DataBytes[(int)blockIndex];
//            if (localPage == null)
//                return IoReadState.ChecksumInvalidBecausePageIsNull;

//            Array.Copy(localPage, 0, data, 0, data.Length);
//            return IoReadState.Valid;
//        }

//        public override void Dispose()
//        {
            
//        }
//    }
//}
