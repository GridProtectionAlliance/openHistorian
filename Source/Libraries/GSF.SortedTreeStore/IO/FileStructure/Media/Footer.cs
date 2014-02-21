//******************************************************************************************************
//  Footer.cs - Gbtc
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
//  2/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF;
using GSF.SortedTreeStore;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// Since exceptions are very expensive, this enum will be returned for basic
    /// I/O operations to let the reader know what to do with the data.  
    /// </summary>
    /// <remarks>There two overarching conditions.  Valid or not Valid.  
    /// If not valid, the reason why the page failed will be given.
    /// If a page is returned as valid, this does not mean that the 
    /// page being referenced is the correct page, it is up to the class
    /// to check the footer of the page to verify that the page being read
    /// is the correct page.</remarks>
    internal enum IoReadState
    {
        /// <summary>
        /// Indicates that the read completed sucessfully.
        /// </summary>
        Valid,

        /// <summary>
        /// The checksum failed to compute
        /// </summary>
        ChecksumInvalid,

        /// <summary>
        /// The page that was requested came from a newer version of the file.
        /// </summary>
        PageNewerThanSnapshotSequenceNumber,

        /// <summary>
        /// The page came from a different file.
        /// </summary>
        FileIdNumberDidNotMatch,

        /// <summary>
        /// The index value did not match that of the file.
        /// </summary>
        IndexNumberMissmatch,

        /// <summary>
        /// The page type requested did not match what was received
        /// </summary>
        BlockTypeMismatch
    }

    internal static unsafe class Footer
    {
        public const int ChecksumIsNotComputed = 0;
        public const int ChecksumIsValid = 1;
        public const int ChecksumIsNotValid = 2;
        public const int ChecksumMustBeRecomputed = 3;


        /// <summary>
        /// Computes the custom checksum of the data.
        /// </summary>
        /// <param name="data">the data to compute the checksum for.</param>
        /// <param name="checksum1">the 64 bit component of this checksum</param>
        /// <param name="checksum2">the 32 bit component of this checksum</param>
        /// <param name="length">the number of bytes to have in the checksum,Must </param>
        /// <remarks>This checksum is similiar to Adler</remarks>
        public static void ComputeChecksum(IntPtr data, out long checksum1, out int checksum2, int length)
        {
            Stats.ChecksumCount++;
            ulong a;
            ulong b;
            Murmur3.ComputeHash((byte*)data, length, out a, out b);
            checksum1 = (long)a;
            checksum2 = (int)b ^ (int)(b >> 32);
        }

        ///// <summary>
        ///// Computes the custom checksum of the data.
        ///// </summary>
        ///// <param name="data">the data to compute the checksum for.</param>
        ///// <param name="checksum1">the 64 bit component of this checksum</param>
        ///// <param name="checksum2">the 32 bit component of this checksum</param>
        ///// <param name="length">the number of bytes to have in the checksum,Must </param>
        ///// <remarks>This checksum is similiar to Adler</remarks>
        //public static void ComputeChecksum(IntPtr data, out long checksum1, out int checksum2, int length)
        //{
        //    if ((length & 7) != 0)
        //        throw new ArgumentOutOfRangeException("length", "Length must be divisible by 8");
        //    Statistics.ChecksumCount++;
        //    ulong* ptr = (ulong*)data;
        //    ulong a = 1;
        //    ulong b = 0;
        //    int iterationCount = length >> 3;
        //    for (int x = 0; x < iterationCount; x++)
        //    {
        //        a += ptr[x];
        //        b += a;
        //    }
        //    checksum1 = (long)b;
        //    checksum2 = (int)a ^ (int)(a >> 32);
        //}

        /// <summary>
        /// This event occurs any time new data is added to the BinaryStream's 
        /// internal memory. It gives the consumer of this class an opportunity to 
        /// properly initialize the data before it is handed to an IoSession.
        /// </summary>
        public static void WriteChecksumResultsToFooter(IntPtr data, int blockSize, int length)
        {
            if (!BitMath.IsPowerOfTwo(blockSize))
                throw new ArgumentOutOfRangeException("blockSize", "Must be a power of two.");
            if (blockSize > length)
                throw new ArgumentException("Must be greater than blockSize", "length");
            if ((length & (blockSize - 1)) != 0)
                throw new ArgumentException("Length is not a multiple of the block size", "length");

            for (int offset = 0; offset < length; offset += blockSize)
            {
                WriteChecksumResultsToFooter(data + offset, blockSize);
            }
        }

        /// <summary>
        /// This event occurs any time new data is added to the BinaryStream's 
        /// internal memory. It gives the consumer of this class an opportunity to 
        /// properly initialize the data before it is handed to an IoSession.
        /// </summary>
        public static void WriteChecksumResultsToFooter(IntPtr data, int blockSize)
        {
            long checksum1;
            int checksum2;
            byte* lpData = (byte*)data;
            ComputeChecksum(data, out checksum1, out checksum2, blockSize - 16);
            long checksumInData1 = *(long*)(lpData + blockSize - 16);
            int checksumInData2 = *(int*)(lpData + blockSize - 8);
            if (checksum1 == checksumInData1 && checksum2 == checksumInData2)
            {
                //Record checksum is valid and put zeroes in all other fields.
                *(int*)(lpData + blockSize - 4) = ChecksumIsValid;
            }
            else
            {
                //Record checksum is not valid and put zeroes in all other fields.
                *(int*)(lpData + blockSize - 4) = ChecksumIsNotValid;
            }
        }

        /// <summary>
        /// This event occurs right before something is committed to the disk. 
        /// This gives the opportunity to finalize the data, such as updating checksums.
        /// </summary>
        public static void ComputeChecksumAndClearFooter(IntPtr data, int blockSize)
        {
            byte* lpData = (byte*)data;

            //Determine if the checksum needs to be recomputed.
            if (lpData[blockSize - 4] == ChecksumMustBeRecomputed)
            {
                long checksum1;
                int checksum2;
                ComputeChecksum(data, out checksum1, out checksum2, blockSize - 16);
                *(long*)(lpData + blockSize - 16) = checksum1;
                *(int*)(lpData + blockSize - 8) = checksum2;
            }
            //reset value to null;
            *(int*)(lpData + blockSize - 4) = ChecksumIsNotComputed;
        }


        /// <summary>
        /// This event occurs right before something is committed to the disk. 
        /// This gives the opportunity to finalize the data, such as updating checksums.
        /// </summary>
        public static void ComputeChecksumAndClearFooter(IntPtr data, int blockSize, int length)
        {
            if (!BitMath.IsPowerOfTwo(blockSize))
                throw new ArgumentOutOfRangeException("blockSize", "Must be a power of two.");
            if (blockSize > length)
                throw new ArgumentException("Must be greater than blockSize", "length");
            if ((length & (blockSize - 1)) != 0)
                throw new ArgumentException("Length is not a multiple of the block size", "length");

            for (int offset = 0; offset < length; offset += blockSize)
            {
                ComputeChecksumAndClearFooter(data + offset, blockSize);
            }
        }

        //ToDo: Fix to support the new header
        ////ToDo: Modify ints to uints.
        ///// <summary>
        ///// Determines if the footer data for the following page is valid.
        ///// </summary>
        ///// <param name="data">the block data to check</param>
        ///// <param name="blockSize">The size of a block</param>
        ///// <param name="blockType">the type of this block.</param>
        ///// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        ///// <param name="fileIdNumber">the file number this block is associated with</param>
        ///// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        ///// <returns>State information about the state of the footer data</returns>
        //public static IoReadState IsFooterValid(IntPtr data, int blockSize, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        //{
        //    byte* lpdata = (byte*)data;
        //    int checksumState = lpdata[blockSize - 4];
        //    if (checksumState == ChecksumIsNotValid)
        //        return IoReadState.ChecksumInvalid;
        //    if (checksumState == ChecksumIsValid || checksumState == ChecksumMustBeRecomputed)
        //    {
        //        if (lpdata[blockSize - 32] != (byte)blockType)
        //            return IoReadState.BlockTypeMismatch;
        //        if (*(int*)(lpdata + blockSize - 28) != indexValue)
        //            return IoReadState.IndexNumberMissmatch;
        //        if ((uint)*(int*)(lpdata + blockSize - 20) > snapshotSequenceNumber) //Note: Convert to uint so negative numbers also fall in this category
        //            return IoReadState.PageNewerThanSnapshotSequenceNumber;
        //        if (*(int*)(lpdata + blockSize - 24) != fileIdNumber)
        //            return IoReadState.FileIdNumberDidNotMatch;
        //        return IoReadState.Valid;
        //    }
        //    throw new Exception("Checksum was not computed properly.");
        //}

        ////ToDo: Modify ints to uints.

        ///// <summary>
        ///// Determines if the footer data for the following page is valid.
        ///// </summary>
        ///// <param name="data">the block data to check</param>
        ///// <param name="blockSize">The size of a block</param>
        ///// <param name="blockType">the type of this block.</param>
        ///// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        ///// <param name="fileIdNumber">the file number this block is associated with</param>
        ///// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        ///// <returns>State information about the state of the footer data</returns>
        //public static IoReadState IsFooterCurrentSnapshotAndValid(IntPtr data, int blockSize, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        //{
        //    byte* lpdata = (byte*)data;
        //    int checksumState = lpdata[blockSize - 4];
        //    if (checksumState == ChecksumIsNotValid)
        //        return IoReadState.ChecksumInvalid;
        //    if (checksumState == ChecksumIsValid || checksumState == ChecksumMustBeRecomputed)
        //    {
        //        if (lpdata[blockSize - 32] != (byte)blockType)
        //            return IoReadState.BlockTypeMismatch;
        //        if (*(int*)(lpdata + blockSize - 28) != indexValue)
        //            return IoReadState.IndexNumberMissmatch;
        //        if ((uint)*(int*)(lpdata + blockSize - 20) != snapshotSequenceNumber) //Note: Convert to uint so negative numbers also fall in this category
        //            return IoReadState.PageNewerThanSnapshotSequenceNumber;
        //        if (*(int*)(lpdata + blockSize - 24) != fileIdNumber)
        //            return IoReadState.FileIdNumberDidNotMatch;
        //        return IoReadState.Valid;
        //    }
        //    throw new Exception("Checksum was not computed properly.");
        //}
        ////ToDo: Modify ints to uints.

        ///// <summary>
        ///// Writes the following footer data to the block.
        ///// </summary>
        ///// <param name="data">the block data to write to</param>
        ///// <param name="blockSize">the number of bytes in a block</param>
        ///// <param name="blockType">the type of this block.</param>
        ///// <param name="indexValue">a value put in the footer of the block designating the index of this block</param>
        ///// <param name="fileIdNumber">the file number this block is associated with</param>
        ///// <param name="snapshotSequenceNumber">the file system sequence number that this read must be valid for.</param>
        ///// <returns></returns>
        //public static void WriteFooterData(byte* data, int blockSize, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        //{
        //    if (indexValue < 0 | fileIdNumber < 0 | snapshotSequenceNumber < 0)
        //        throw new Exception();

        //    data[blockSize - 4] = ChecksumMustBeRecomputed;
        //    data[blockSize - 32] = (byte)blockType;
        //    *(int*)(data + blockSize - 28) = indexValue;
        //    *(int*)(data + blockSize - 24) = fileIdNumber;
        //    *(int*)(data + blockSize - 20) = snapshotSequenceNumber;
        //}

        ////ToDo: Modify ints to uints.
        //public static void ClearFooterData(byte* data, int blockSize)
        //{
        //    *(long*)(data + blockSize - 32) = 0;
        //    *(long*)(data + blockSize - 24) = 0;
        //    *(long*)(data + blockSize - 16) = 0;
        //    *(long*)(data + blockSize - 8) = 0;
        //}
    }
}