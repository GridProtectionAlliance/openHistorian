//******************************************************************************************************
//  DiskIOBase.cs - Gbtc
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
//  12/30/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.File
{
    internal abstract class DiskIOBase
    {
        public void WriteBlock(uint blockIndex, BlockType blockType, uint indexValue, uint fileIDNumber, uint snapshotSequenceNumber, byte[] data)
        {
            if (IsReadOnly)
                throw new Exception("File system is read only");
            if (data.Length != ArchiveConstants.BlockSize)
                throw new Exception("All page IOs must be performed one page at a time.");

            //If the file is not large enought to set this block, autogrow the file.
            if ((long)(blockIndex + 1) * ArchiveConstants.BlockSize > FileSize)
            {
                SetFileLength(0, blockIndex + 1);
            }

            WriteFooterData(data, blockType, indexValue, fileIDNumber, snapshotSequenceNumber);

            WriteDataBlock(blockIndex, data);
        }

        abstract protected void WriteDataBlock(uint blockIndex, byte[] data);

        public IOReadState ReadBlock(uint blockIndex, BlockType blockType, uint indexValue, uint fileIDNumber, uint snapshotSequenceNumber, byte[] data)
        {
            if (data.Length != ArchiveConstants.BlockSize)
                throw new Exception("All page IOs must be performed one page at a time.");
            if ((long)(blockIndex + 1) * ArchiveConstants.BlockSize > FileSize)
                return IOReadState.ReadPastThenEndOfTheFile;

            IOReadState readState = ReadBlock(blockIndex, data);
            if (readState != IOReadState.Valid)
                return readState;

            return IsFooterValid(data, blockType, indexValue, fileIDNumber, snapshotSequenceNumber);
        }

        /// <summary>
        /// This will resize the file to the provided size in bytes;
        /// If resizing smaller than the allocated space, this number is 
        /// increased to the allocated space.  
        /// If file size is not a multiple of the page size, it is rounded up to
        /// the nearest page boundry
        /// </summary>
        /// <param name="Size">The number of bytes to make the file.</param>
        /// <returns>The size that the file is after this call</returns>
        /// <remarks>Passing 0 to this function will effectively trim out 
        /// all of the free space in this file.</remarks>
        public long SetFileLength(long size, uint nextUnallocatedBlock)
        {
            if (nextUnallocatedBlock * ArchiveConstants.BlockSize > size)
            {
                //if shrinking beyond the allocated space, 
                //adjust the size exactly to the allocated space.
                size = nextUnallocatedBlock * ArchiveConstants.BlockSize;
            }
            else
            {
                long remainder = (size % ArchiveConstants.BlockSize);
                //if there will be a fragmented page remaining
                if (remainder != 0)
                {
                    //if the requested size is not a multiple of the page size
                    //round up to the nearest page
                    size = size + ArchiveConstants.BlockSize - remainder;
                }
            }
            return SetFileLength(size);
        }
        abstract protected long SetFileLength(long requestedSize);

        abstract protected IOReadState ReadBlock(uint blockIndex, byte[] data);

        abstract public bool IsReadOnly { get; }
        abstract public long FileSize { get; }

        /// <summary>
        /// The calculated checksum for a page of all zeros.
        /// </summary>
        const long EMPTY_CHECKSUM = 6845471437889732609;

        static public long checksumcount = 0;
        
        static internal unsafe long ComputeChecksum(byte[] data)
        {
            checksumcount += 1;
            // return 0;
            if (data.Length != 4096)
            {
                throw new Exception("This checksum is only valid for a length of 4096 bytes");
            }

            long A = 1; //Maximum size for A is 20 bits in length
            long B = 0; //Maximum size for B is 31 bits in length
            long C = 0; //Maximum size for C is 42 bits in length
            for (int X = 0; X < ArchiveConstants.BlockSize - 8; X ++)
            {
                A += data[X];
                B += A;
                C += B;
            }
            //Since only 13 bits of C will remain, xor all 42 bits of C into the first 13 bits.
            C = C ^ (C >> 13) ^ (C >> 26) ^ (C >> 39);
            return (C << 51) ^ (B << 20) ^ A;
        }

        static IOReadState IsFooterValid(byte[] data, BlockType pageType, uint indexValue, uint featureSequenceNumber, uint revisionSequenceNumber)
        {
            long checksum = ComputeChecksum(data);
            long checksumInData = BitConverter.ToInt64(data, ArchiveConstants.BlockSize - 8);
            if (checksum == checksumInData)
            {
                if (data[ArchiveConstants.BlockSize - 21] != (byte)pageType)
                    return IOReadState.BlockTypeMismatch;
                if (BitConverter.ToUInt32(data, ArchiveConstants.BlockSize - 20) != indexValue)
                    return IOReadState.IndexNumberMissmatch;
                if (BitConverter.ToUInt32(data, ArchiveConstants.BlockSize - 12) > revisionSequenceNumber)
                    return IOReadState.PageNewerThanSnapshotSequenceNumber;
                if (BitConverter.ToUInt32(data, ArchiveConstants.BlockSize - 16) != featureSequenceNumber)
                    return IOReadState.FileIDNumberDidNotMatch;
                return IOReadState.Valid;
            }
            else if ((checksumInData == 0) && (checksum == EMPTY_CHECKSUM))
            {
                return IOReadState.ChecksumInvalidBecausePageIsNull;
            }
            else
            {
                return IOReadState.ChecksumInvalid;
            }
        }

        static void WriteFooterData(byte[] data, BlockType pageType, uint indexValue, uint featureSequenceNumber, uint revisionSequenceNumber)
        {
            data[ArchiveConstants.BlockSize - 21] = (byte)pageType;
            data[ArchiveConstants.BlockSize - 20] = (byte)(indexValue);
            data[ArchiveConstants.BlockSize - 19] = (byte)(indexValue >> 8);
            data[ArchiveConstants.BlockSize - 18] = (byte)(indexValue >> 16);
            data[ArchiveConstants.BlockSize - 17] = (byte)(indexValue >> 24);
            data[ArchiveConstants.BlockSize - 16] = (byte)(featureSequenceNumber);
            data[ArchiveConstants.BlockSize - 15] = (byte)(featureSequenceNumber >> 8);
            data[ArchiveConstants.BlockSize - 14] = (byte)(featureSequenceNumber >> 16);
            data[ArchiveConstants.BlockSize - 13] = (byte)(featureSequenceNumber >> 24);
            data[ArchiveConstants.BlockSize - 12] = (byte)(revisionSequenceNumber);
            data[ArchiveConstants.BlockSize - 11] = (byte)(revisionSequenceNumber >> 8);
            data[ArchiveConstants.BlockSize - 10] = (byte)(revisionSequenceNumber >> 16);
            data[ArchiveConstants.BlockSize - 9] = (byte)(revisionSequenceNumber >> 24);

            long checksum = ComputeChecksum(data);
            data[ArchiveConstants.BlockSize - 8] = (byte)(checksum);
            data[ArchiveConstants.BlockSize - 7] = (byte)(checksum >> 8);
            data[ArchiveConstants.BlockSize - 6] = (byte)(checksum >> 16);
            data[ArchiveConstants.BlockSize - 5] = (byte)(checksum >> 24);
            data[ArchiveConstants.BlockSize - 4] = (byte)(checksum >> 32);
            data[ArchiveConstants.BlockSize - 3] = (byte)(checksum >> 40);
            data[ArchiveConstants.BlockSize - 2] = (byte)(checksum >> 48);
            data[ArchiveConstants.BlockSize - 1] = (byte)(checksum >> 56);
        }

    }
}
