//******************************************************************************************************
//  DiskIoEnhancedExtensions.cs - Gbtc
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.FileSystem
{
    unsafe internal static class DiskIoEnhancedExtensions
    {

        public static void Read(this DiskIoEnhanced memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] destination)
        {
            using (var buffer = memoryUnit.GetMemoryUnit())
            {
                buffer.Read(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber, destination);
            }
        }

        public static void WriteToExistingBlock(this DiskIoEnhanced memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] data)
        {
            using (var buffer = memoryUnit.GetMemoryUnit())
            {
                buffer.WriteToExistingBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber, data);
            }
        }

        public static void WriteToNewBlock(this DiskIoEnhanced memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] data)
        {
            using (var buffer = memoryUnit.GetMemoryUnit())
            {
                buffer.WriteToNewBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber, data);
            }
        }

        public static void WriteZeroesToNewBlock(this DiskIoEnhanced memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            using (var buffer = memoryUnit.GetMemoryUnit())
            {
                buffer.WriteZeroesToNewBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            }
        }

        public static void CopyBlock(this DiskIoEnhanced memoryUnit, int sourceAddress, int destinationAddress, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            using (MemoryUnit destinationData = memoryUnit.GetMemoryUnit())
            using (MemoryUnit sourceData = memoryUnit.GetMemoryUnit())
            {
                sourceData.Read(sourceAddress, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber);
                destinationData.BeginWriteToNewBlock(destinationAddress);
                Memory.Copy(sourceData.Pointer, destinationData.Pointer, sourceData.Length);
                destinationData.EndWrite(BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber);
            }

        }
           
    }
}
