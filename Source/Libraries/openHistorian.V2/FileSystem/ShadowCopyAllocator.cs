//******************************************************************************************************
//  ShadowCopyAllocator.cs - Gbtc
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
//  1/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************
using System;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// This class will make shadow copies of blocks or, if the block has never been written to, prepare the block to be written to.
    /// </summary>
    unsafe internal class ShadowCopyAllocator
    {
        #region [ Members ]

        /// <summary>
        /// This address is used to determine if the block being referenced is an old block or a new one. 
        /// Any addresses greater than or equal to this are new blocks for this transaction. Values before this are old.
        /// </summary>
        int m_lastReadOnlyBlock;
        /// <summary>
        /// The file being read.
        /// </summary>
        FileMetaData m_fileMetaData;
        /// <summary>
        /// The disk to make the IO requests to.
        /// </summary>
        DiskIo m_diskIo;
        /// <summary>
        /// The FileAllocationTable that can be used to allocate space.
        /// </summary>
        FileAllocationTable m_fileAllocationTable;
        /// <summary>
        /// A parser that is used to navigate to this block.
        /// This parser is in common with the one used in the <see cref="FileAddressTranslation"/>.
        /// </summary>
        IndexParser m_parser;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="ShadowCopyAllocator"/> that is used make shadow copies of blocks.
        /// </summary>
        /// <param name="dataReader">A DiskIO that allows writing to the file.</param>
        /// <param name="fileAllocationTable">The file allocation table that is editable</param>
        /// <param name="fileMetaData">The file that is used</param>
        /// <param name="parser">The indexparser used by the caller to designate what block needs to be copied.</param>
        public ShadowCopyAllocator(DiskIo dataReader, FileAllocationTable fileAllocationTable, FileMetaData fileMetaData, IndexParser parser)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            if (fileAllocationTable == null)
                throw new ArgumentNullException("fileAllocationTable");
            if (fileMetaData == null)
                throw new ArgumentNullException("fileMetaData");
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (dataReader.IsReadOnly)
                throw new ArgumentException("DataReader is read only", "dataReader");
            if (fileAllocationTable.IsReadOnly)
                throw new ArgumentException("FileAllocationTable is read only", "fileAllocationTable");
            if (fileMetaData.IsReadOnly)
                throw new ArgumentException("FileMetaData is read only", "fileMetaData");

            m_parser = parser;
            m_lastReadOnlyBlock = fileAllocationTable.LastAllocatedBlock;
            m_fileAllocationTable = fileAllocationTable;
            m_fileMetaData = fileMetaData;
            m_diskIo = dataReader;
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// This will make a shadow copy of the block that contains the position provided.  
        /// If the block does not exist, space is allocated and the indexes are 
        /// set up to allow the block to be writen to.
        /// </summary>
        /// <param name="position">The position the application intents to write to.</param>
        /// <remarks>Calling this function automatically updates the underlying parser.</remarks>
        public void ShadowDataBlock(long position)
        {
            m_parser.SetPosition(position);
            int dataBlockAddress; //The address to the shadowed data page.
            int firstIndirectAddress;
            int secondIndirectAddress;
            int thirdIndirectAddress;

            //Make a copy of the data page referenced
            ShadowCopyDataBlock(out dataBlockAddress);
            ShadowCopyIndexIndirectBlocks(dataBlockAddress, out firstIndirectAddress, out secondIndirectAddress, out thirdIndirectAddress);

            m_parser.UpdateAddressesFromShadowCopy(dataBlockAddress, firstIndirectAddress, secondIndirectAddress, thirdIndirectAddress);
        }

        #region [ Shadow Copy Index Blocks ]

        void ShadowCopyIndexIndirectBlocks(int dataBlockAddress, out int firstIndirectAddress, out int secondIndirectAddress, out int thirdIndirectAddress)
        {
            firstIndirectAddress = 0;
            secondIndirectAddress = 0;
            thirdIndirectAddress = 0;
            switch (m_parser.IndirectNumber)
            {
                case 0:
                    m_fileMetaData.DirectBlock = dataBlockAddress;
                    break;
                case 1:
                    firstIndirectAddress = ShadowCopyIndexIndirect(m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, dataBlockAddress);
                    m_fileMetaData.SingleIndirectBlock = firstIndirectAddress;
                    break;
                case 2:
                    secondIndirectAddress = ShadowCopyIndexIndirect(m_parser.SecondIndirectBlockAddress, m_parser.SecondIndirectBaseIndex, 2, m_parser.SecondIndirectOffset, dataBlockAddress);
                    firstIndirectAddress = ShadowCopyIndexIndirect(m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, secondIndirectAddress);
                    m_fileMetaData.DoubleIndirectBlock = firstIndirectAddress;
                    break;
                case 3:
                    thirdIndirectAddress = ShadowCopyIndexIndirect(m_parser.ThirdIndirectBlockAddress, m_parser.ThirdIndirectBaseIndex, 3, m_parser.ThirdIndirectOffset, dataBlockAddress);
                    secondIndirectAddress = ShadowCopyIndexIndirect(m_parser.SecondIndirectBlockAddress, m_parser.SecondIndirectBaseIndex, 2, m_parser.SecondIndirectOffset, thirdIndirectAddress);
                    firstIndirectAddress = ShadowCopyIndexIndirect(m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, secondIndirectAddress);
                    m_fileMetaData.TripleIndirectBlock = firstIndirectAddress;
                    break;
                default:
                    throw new Exception("invalid redirect number");
            }

        }

        /// <summary>
        /// Makes a shadow copy of the indirect index passed to this function. If the block does not exists, it creates it.
        /// </summary>
        /// <param name="sourceBlockAddress">The block to be copied</param>
        /// <param name="indexValue">the index value that goes in the footer of the file.</param>
        /// <param name="indexIndirectNumber">the indirect number {1,2,3} that goes in the footer of the block.</param>
        /// <param name="remoteAddressOffset">the offset of the remote address that needs to be updated.</param>
        /// <param name="remoteBlockAddress">the value of the remote address.</param>
        /// <returns>The address of the shadow copy.</returns>
        int ShadowCopyIndexIndirect(int sourceBlockAddress, int indexValue, byte indexIndirectNumber, int remoteAddressOffset, int remoteBlockAddress)
        {
            int indexIndirectBlock; //The address to the shadowed index block.

            //Make a copy of the index block referenced

            //if the block does not exist, create it.
            if (sourceBlockAddress == 0)
            {
                using (var buffer = m_diskIo.CreateDiskIoSession())
                {
                    indexIndirectBlock = m_fileAllocationTable.AllocateFreeBlocks(1);
                    buffer.BeginWriteToNewBlock(indexIndirectBlock);
                    Memory.Clear(buffer.Pointer, buffer.Length);
                    WriteIndexIndirectBlock(buffer.Pointer, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                    buffer.EndWrite(BlockType.IndexIndirect, indexValue, m_fileMetaData.FileIdNumber, m_fileAllocationTable.SnapshotSequenceNumber);
                }
            }
            //if the data page is an old page, allocate space to create a new copy
            else if (sourceBlockAddress <= m_lastReadOnlyBlock)
            {
                indexIndirectBlock = m_fileAllocationTable.AllocateFreeBlocks(1);
                ReadThenWriteIndexIndirectBlock(sourceBlockAddress, indexIndirectBlock, indexValue, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
            }
            //The page has already been copied, use the existing address.
            else
            {
                indexIndirectBlock = sourceBlockAddress;
                ReadThenWriteIndexIndirectBlock(sourceBlockAddress, sourceBlockAddress, indexValue, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
            }
            return indexIndirectBlock;
        }

        /// <summary>
        /// Makes a shadow copy of an index indirect block and updates a remote address. 
        /// </summary>
        /// <param name="sourceBlockAddress">the address of the source.</param>
        /// <param name="destinationBlockAddress">the address of the destination. This can be the same as the source.</param>
        /// <param name="indexValue">the index value that goes in the footer of the file.</param>
        /// <param name="indexIndirectNumber">the indirect number {1,2,3} that goes in the footer of the block.</param>
        /// <param name="remoteAddressOffset">the offset of the remote address that needs to be updated.</param>
        /// <param name="remoteBlockAddress">the value of the remote address.</param>
        void ReadThenWriteIndexIndirectBlock(int sourceBlockAddress, int destinationBlockAddress, int indexValue, byte indexIndirectNumber, int remoteAddressOffset, int remoteBlockAddress)
        {
            using (var bufferSource = m_diskIo.CreateDiskIoSession())
            {
                int fileIdNumber = m_fileMetaData.FileIdNumber;
                int snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;

                if (sourceBlockAddress == destinationBlockAddress)
                    bufferSource.Read(sourceBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
                else
                    bufferSource.Read(sourceBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber - 1);

                if (bufferSource.Pointer[ArchiveConstants.BlockSize - 22] != indexIndirectNumber)
                    throw new Exception("The redirect value of this page is incorrect");


                //we only need to update the base address if something has changed.
                //Therefore, if the source and the destination are the same, and the remote block is the same
                //everything else is going to be the same.
                if (sourceBlockAddress != destinationBlockAddress)
                {
                    using (DiskIoSession destination = m_diskIo.CreateDiskIoSession())
                    {
                        destination.BeginWriteToNewBlock(destinationBlockAddress);
                        Memory.Copy(bufferSource.Pointer, destination.Pointer, destination.Length);
                        WriteIndexIndirectBlock(destination.Pointer, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                        destination.EndWrite(BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
                    }
                }
                else if (*(int*)(bufferSource.Pointer + remoteAddressOffset) != remoteBlockAddress)
                {
                    bufferSource.BeginWriteToExistingBlock(destinationBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
                    WriteIndexIndirectBlock(bufferSource.Pointer, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                    bufferSource.EndWrite(BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
                }
            }
        }

        /// <summary>
        /// Writes an Indirect Block to the drive. This sets the indexIndirectNumber and updates one of the addresses within this index.
        /// </summary>
        /// <param name="pointer"> </param>
        /// <param name="indexIndirectNumber">the indirect number {1,2,3} that goes in the footer of the block</param>
        /// <param name="remoteAddressOffset">the offset of the remote address that needs to be updated</param>
        /// <param name="remoteBlockAddress">the value of the remote address</param>
        void WriteIndexIndirectBlock(byte* pointer, byte indexIndirectNumber, int remoteAddressOffset, int remoteBlockAddress)
        {
            pointer[ArchiveConstants.BlockSize - 22] = indexIndirectNumber;
            *(int*)(pointer + remoteAddressOffset) = remoteBlockAddress;
        }

        #endregion

        #region [ Data Block Shadow Copy ]

        void ShadowCopyDataBlock(out int dataBlockAddress)
        {
            //if the page does not exist -or-
            //if the data page is an old page, allocate space to create a new copy
            if (m_parser.DataClusterAddress == 0 ||
                m_parser.DataClusterAddress <= m_lastReadOnlyBlock)
            {
                dataBlockAddress = m_fileAllocationTable.AllocateFreeBlocks(1);
                ShadowCopyDataCluster(m_parser.DataClusterAddress, m_parser.BaseVirtualAddressIndexValue, dataBlockAddress);
            }
            else
            {
                //The page has already been copied, use the existing address.
                dataBlockAddress = m_parser.DataClusterAddress;
            }
        }

        /// <summary>
        /// Makes a shadow copy of a data cluster.
        /// </summary>
        /// <param name="sourceClusterAddress">the address of the first block in the cluster. 
        /// If address is zero, it simply creates an empty cluster.</param>
        /// <param name="indexValue">the index value of this first block</param>
        /// <param name="destinationClusterAddress">the first block of the destination cluster</param>
        void ShadowCopyDataCluster(int sourceClusterAddress, int indexValue, int destinationClusterAddress)
        {
            int fileIdNumber = m_fileMetaData.FileIdNumber;
            int snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;

            //if source exist
            if (sourceClusterAddress != 0)
            {
                m_diskIo.CopyBlock(sourceClusterAddress, destinationClusterAddress, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber - 1);
            }
            //if source cluster does not exist.
            else
            {
                m_diskIo.WriteZeroesToNewBlock(destinationClusterAddress, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber);
            }
        }

        #endregion

        #endregion

    }
}
