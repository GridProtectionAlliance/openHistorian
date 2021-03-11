//******************************************************************************************************
//  Footer.cs - Gbtc
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
//  2/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.Snap;

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
            Murmur3.ComputeHash((byte*)data, length, out ulong a, out ulong b);
            checksum1 = (long)a;
            checksum2 = (int)b ^ (int)(b >> 32);
        }

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
            byte* lpData = (byte*)data;
            ComputeChecksum(data, out long checksum1, out int checksum2, blockSize - 16);
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
                ComputeChecksum(data, out long checksum1, out int checksum2, blockSize - 16);
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
    }
}