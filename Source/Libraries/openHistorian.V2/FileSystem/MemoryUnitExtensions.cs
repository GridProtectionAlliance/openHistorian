//******************************************************************************************************
//  MemoryUnitExtensions.cs - Gbtc
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
using System.Runtime.InteropServices;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.FileSystem
{
    unsafe internal static class MemoryUnitExtensions
    {
        
        public static void Read(this MemoryUnit memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] destination)
        {
            memoryUnit.Read(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            Marshal.Copy(memoryUnit.IntPtr, destination, 0, ArchiveConstants.BlockSize);
        }

        public static void WriteToExistingBlock(this MemoryUnit memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] data)
        {
            memoryUnit.BeginWriteToExistingBlock(blockIndex, blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
            Marshal.Copy(data, 0, memoryUnit.IntPtr, ArchiveConstants.BlockSize);
            memoryUnit.EndWrite(blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
        }

        public static void WriteToNewBlock(this MemoryUnit memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber, byte[] data)
        {
            memoryUnit.BeginWriteToNewBlock(blockIndex);
            Marshal.Copy(data, 0, memoryUnit.IntPtr, ArchiveConstants.BlockSize);
            memoryUnit.EndWrite(blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
        }

        public static void WriteZeroesToNewBlock(this MemoryUnit memoryUnit, int blockIndex, BlockType blockType, int indexValue, int fileIdNumber, int snapshotSequenceNumber)
        {
            memoryUnit.BeginWriteToNewBlock(blockIndex);
            Memory.Clear(memoryUnit.Pointer, memoryUnit.Length);
            memoryUnit.EndWrite(blockType, indexValue, fileIdNumber, snapshotSequenceNumber);
        }

    }
}
