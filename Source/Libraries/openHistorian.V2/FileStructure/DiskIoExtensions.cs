//******************************************************************************************************
//  DiskIoExtensions.cs - Gbtc
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
//  6/15/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using openHistorian.UnmanagedMemory;

namespace openHistorian.FileStructure
{
    unsafe internal static class DiskIoExtensions
    {

        public static void Read(this DiskIo memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] destination)
        {
            using (var buffer = memoryUnit.CreateDiskIoSession())
            {
                buffer.Read(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber, destination);
            }
        }

        public static void WriteToExistingBlock(this DiskIo memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] data)
        {
            using (var buffer = memoryUnit.CreateDiskIoSession())
            {
                buffer.WriteToExistingBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber, data);
            }
        }

        public static void WriteToNewBlock(this DiskIo memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] data)
        {
            using (var buffer = memoryUnit.CreateDiskIoSession())
            {
                buffer.WriteToNewBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber, data);
            }
        }

        /// <summary>
        /// Clears an entire block by writing zeroes to it.
        /// </summary>
        /// <param name="memoryUnit">the extension object</param>
        /// <param name="blockIndex">The block address</param>
        /// <param name="blockType">The type of block that will be copied.</param>
        /// <param name="indexValue">The block's index value.</param>
        /// <param name="fileIdNumber">The block's fileIdNumber.</param>
        /// <param name="snapshotSequenceNumber">The blocks's snapshot sequence number.</param>
        public static void WriteZeroesToNewBlock(this DiskIo memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            using (var buffer = memoryUnit.CreateDiskIoSession())
            {
                buffer.WriteZeroesToNewBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            }
        }

        /// <summary>
        /// Copies one block from one address to another.
        /// </summary>
        /// <param name="memoryUnit">the extension object</param>
        /// <param name="sourceAddress">The source address</param>
        /// <param name="destinationAddress">The destination address.</param>
        /// <param name="blockType">The type of block that will be copied.</param>
        /// <param name="indexValue">The block's index value.</param>
        /// <param name="fileIdNumber">The block's fileIdNumber.</param>
        /// <param name="snapshotSequenceNumber">The blocks's snapshot sequence number.</param>
        public static void CopyBlock(this DiskIo memoryUnit, int sourceAddress, int destinationAddress, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            using (DiskIoSession destinationData = memoryUnit.CreateDiskIoSession())
            using (DiskIoSession sourceData = memoryUnit.CreateDiskIoSession())
            {
                sourceData.Read(sourceAddress, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
                destinationData.BeginWriteToNewBlock(destinationAddress);
                Memory.Copy(sourceData.Pointer, destinationData.Pointer, sourceData.Length);
                destinationData.EndWrite(blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            }

        }
           
    }
}
