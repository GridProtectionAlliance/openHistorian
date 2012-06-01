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
        int m_newBlocksStartAtThisAddress;
        /// <summary>
        /// The file being read.
        /// </summary>
        FileMetaData m_fileMetaData;
        /// <summary>
        /// The disk to make the IO requests to.
        /// </summary>
        DiskIoEnhanced m_diskIo;
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
        public ShadowCopyAllocator(DiskIoEnhanced dataReader, FileAllocationTable fileAllocationTable, FileMetaData fileMetaData, IndexParser parser)
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
            m_newBlocksStartAtThisAddress = fileAllocationTable.NextUnallocatedBlock;
            m_fileAllocationTable = fileAllocationTable;
            m_fileMetaData = fileMetaData;
            m_diskIo = dataReader;
        }
        #endregion

        #region [ Properties ]

        IndexBufferPool BufferPool
        {
            get
            {
                return m_parser.BufferPool;
            }

        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// This will make a shadow copy of the block that contains the position provided.  If the block does not exist, space is allocated and the indexes are 
        /// set up to allow the block to be writen to.
        /// </summary>
        /// <param name="position">The position the application intents to write to.</param>
        ///// <param name="SkipDataBlockShadow">Tells the shadow copier to skip the read/write operation associated with copying the data page.
        ///// This should only be set to true if the entire block is written at one time. 
        ///// If no data is going to be read and all of it will be overwritten, there is no reason to read the old data.</param>
        /// <remarks>Calling this function automatically updates the underlying parser.</remarks>
        public void ShadowDataBlock(long position)//, bool SkipDataBlockShadow = false)
        {
            //SkipDataBlockShadow is only valid if there is only 1 block per cluster.
            //if (SkipDataBlockShadow && m_FileMetaData.BlocksPerCluster != 1)
            //    SkipDataBlockShadow = false;

            m_parser.SetPosition(position);
            int dataBlockAddress; //The address to the shadowed data page.
            int firstIndirectAddress = 0;
            int secondIndirectAddress = 0;
            int thirdIndirectAddress = 0;

            //Make a copy of the data page referenced
            if (m_parser.DataClusterAddress == 0)
            {
                //if the page does not exist, create it.
                dataBlockAddress = m_fileAllocationTable.AllocateFreeBlocks(1);
                //if (!SkipDataBlockShadow)
                ShadowCopyDataCluster(m_parser.DataClusterAddress, m_parser.BaseVirtualAddressIndexValue, dataBlockAddress);
            }
            else if (m_parser.DataClusterAddress < m_newBlocksStartAtThisAddress)
            {
                //if the data page is an old page, allocate space to create a new copy
                dataBlockAddress = m_fileAllocationTable.AllocateFreeBlocks(1);
                //if (!SkipDataBlockShadow)
                ShadowCopyDataCluster(m_parser.DataClusterAddress, m_parser.BaseVirtualAddressIndexValue, dataBlockAddress);
            }
            else
            {
                //The page has already been copied, use the existing address.
                dataBlockAddress = m_parser.DataClusterAddress;
            }

            switch (m_parser.IndirectNumber)
            {
                case 0:
                    m_fileMetaData.DirectCluster = dataBlockAddress;
                    break;
                case 1:
                    firstIndirectAddress = ShadowCopyIndexIndirect1(dataBlockAddress);
                    m_fileMetaData.SingleIndirectCluster = firstIndirectAddress;
                    break;
                case 2:
                    secondIndirectAddress = ShadowCopyIndexIndirect2(dataBlockAddress);
                    firstIndirectAddress = ShadowCopyIndexIndirect1(secondIndirectAddress);
                    m_fileMetaData.DoubleIndirectCluster = firstIndirectAddress;
                    break;
                case 3:
                    thirdIndirectAddress = ShadowCopyIndexIndirect3(dataBlockAddress);
                    secondIndirectAddress = ShadowCopyIndexIndirect2(thirdIndirectAddress);
                    firstIndirectAddress = ShadowCopyIndexIndirect1(secondIndirectAddress);
                    m_fileMetaData.TripleIndirectCluster = firstIndirectAddress;
                    break;
                default:
                    throw new Exception("invalid redirect number");
            }
            m_parser.UpdateAddressesFromShadowCopy(dataBlockAddress, firstIndirectAddress, secondIndirectAddress, thirdIndirectAddress);
        }

        /// <summary>
        /// Helper function to make a shadow index of a first indirect block.
        /// </summary>
        /// <param name="remoteBlockAddress">The remote address that needs to be referenced by the shadow copied page.</param>
        /// <returns></returns>
        int ShadowCopyIndexIndirect1(int remoteBlockAddress)
        {
            return ShadowCopyIndexIndirect(BufferPool.FirstIndirect, m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, remoteBlockAddress);
        }

        /// <summary>
        /// Helper function to make a shadow index of a second indirect block.
        /// </summary>
        /// <param name="remoteBlockAddress">The remote address that needs to be referenced by the shadow copied page.</param>
        /// <returns></returns>
        int ShadowCopyIndexIndirect2(int remoteBlockAddress)
        {
            return ShadowCopyIndexIndirect(BufferPool.SecondIndirect, m_parser.SecondIndirectBlockAddress, m_parser.SecondIndirectBaseIndex, 2, m_parser.SecondIndirectOffset, remoteBlockAddress);
        }

        /// <summary>
        /// Helper function to make a shadow index of a third indirect block.
        /// </summary>
        /// <param name="remoteBlockAddress">The remote address that needs to be referenced by the shadow copied page.</param>
        /// <returns></returns>
        int ShadowCopyIndexIndirect3(int remoteBlockAddress)
        {
            return ShadowCopyIndexIndirect(BufferPool.ThirdIndirect, m_parser.ThirdIndirectBlockAddress, m_parser.ThirdIndirectBaseIndex, 3, m_parser.ThirdIndirectOffset, remoteBlockAddress);
        }

  /// <summary>
        /// Makes a shadow copy of the indirect index passed to this function. If the block does not exists, it creates it.
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="sourceBlockAddress">The block to be copied</param>
        /// <param name="indexValue">the index value that goes in the footer of the file.</param>
        /// <param name="indexIndirectNumber">the indirect number {1,2,3,4} that goes in the footer of the block.</param>
        /// <param name="remoteAddressOffset">the offset of the remote address that needs to be updated.</param>
        /// <param name="remoteBlockAddress">the value of the remote address.</param>
        /// <returns>The address of the shadow copy.</returns>
        int ShadowCopyIndexIndirect(IMemoryUnit buffer, int sourceBlockAddress, int indexValue, byte indexIndirectNumber, int remoteAddressOffset, int remoteBlockAddress)
        {
            int indexIndirectBlock; //The address to the shadowed index block.

            //Make a copy of the index block referenced
            if (sourceBlockAddress == 0)
            {
                //if the block does not exist, create it.
                indexIndirectBlock = m_fileAllocationTable.AllocateFreeBlocks(1);
                m_diskIo.AquireBlockForWrite(indexIndirectBlock, buffer);

                Memory.Clear(buffer.Pointer,buffer.Length);

                WriteIndexIndirectBlock(buffer, indexIndirectBlock, indexValue, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
            }
            else if (sourceBlockAddress < m_newBlocksStartAtThisAddress)
            {
                //if the data page is an old page, allocate space to create a new copy
                indexIndirectBlock = m_fileAllocationTable.AllocateFreeBlocks(1);
                ReadThenWriteIndexIndirectBlock(buffer, sourceBlockAddress, indexIndirectBlock, indexValue, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress, false);
            }
            else
            {
                //The page has already been copied, use the existing address.
                ReadThenWriteIndexIndirectBlock(buffer, sourceBlockAddress, sourceBlockAddress, indexValue, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress, true);
                indexIndirectBlock = sourceBlockAddress;
            }
            return indexIndirectBlock;
        }

        /// <summary>
        /// Makes a shadow copy of an index indirect block and updates a remote address. 
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="sourceBlockAddress">the address of the source.</param>
        /// <param name="destinationBlockAddress">the address of the destination. This can be the same as the source.</param>
        /// <param name="indexValue">the index value that goes in the footer of the file.</param>
        /// <param name="indexIndirectNumber">the indirect number {1,2,3,4} that goes in the footer of the block.</param>
        /// <param name="remoteAddressOffset">the offset of the remote address that needs to be updated.</param>
        /// <param name="remoteBlockAddress">the value of the remote address.</param>
        /// <param name="isCurrentRevision">If this is an inplace edit, set to true. If it is a true shadow copy, set false.</param>
        private void ReadThenWriteIndexIndirectBlock(IMemoryUnit buffer, int sourceBlockAddress, int destinationBlockAddress, int indexValue, byte indexIndirectNumber, int remoteAddressOffset, int remoteBlockAddress, bool isCurrentRevision)
        {
            int fileIdNumber = m_fileMetaData.FileIdNumber;
            int snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;

            if (buffer.BlockIndex != sourceBlockAddress)
            {
                IoReadState readState;
                if (isCurrentRevision)
                    readState = m_diskIo.AquireBlockForRead(sourceBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber, buffer);
                else
                    readState = m_diskIo.AquireBlockForRead(sourceBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber - 1, buffer);
                if (readState != IoReadState.Valid)
                    throw new Exception("Error Reading File " + readState.ToString());
            }
            if (buffer.Pointer[ArchiveConstants.BlockSize - 22] != indexIndirectNumber)
                throw new Exception("The redirect value of this page is incorrect");

            //we only need to update the base address if something has changed.
            //Therefore, if the source and the destination are the same, and the remote block is the same
            //everything else is going to be the same.
            if (sourceBlockAddress != destinationBlockAddress || *(int*)(buffer.Pointer + remoteAddressOffset) != remoteBlockAddress)
            {
                using (IMemoryUnit data = m_diskIo.GetMemoryUnit())
                {
                    m_diskIo.AquireBlockForWrite(destinationBlockAddress, data);
                    Memory.Copy(buffer.Pointer,data.Pointer,data.Length);
                    WriteIndexIndirectBlock(data, destinationBlockAddress, indexValue, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                }

            }
        }

        /// <summary>
        /// Writes an Indirect Block to the drive. This sets the indexIndirectNumber and updates one of the addresses within this index.
        /// </summary>
        /// <param name="buffer"> </param>
        /// <param name="blockAddress">the address of the index indirect to write to.</param>
        /// <param name="indexValue">the index value that goes in the footer of the file.</param>
        /// <param name="indexIndirectNumber">the indirect number {1,2,3,4} that goes in the footer of the block</param>
        /// <param name="remoteAddressOffset">the offset of the remote address that needs to be updated</param>
        /// <param name="remoteBlockAddress">the value of the remote address</param>
        private void WriteIndexIndirectBlock(IMemoryUnit buffer, int blockAddress, int indexValue, byte indexIndirectNumber, int remoteAddressOffset, int remoteBlockAddress)
        {
            int fileIdNumber = m_fileMetaData.FileIdNumber;
            int snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;

            buffer.Pointer[ArchiveConstants.BlockSize - 22] = indexIndirectNumber;
            *(int*)(buffer.Pointer + remoteAddressOffset) = remoteBlockAddress;

            if (buffer.BlockIndex != blockAddress )
                throw new Exception("Addresses do not match");

            m_diskIo.WriteBlock(BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber, buffer);
        }

        /// <summary>
        /// Makes a shadow copy of a data cluster.
        /// </summary>
        /// <param name="sourceClusterAddress">the address of the first block in the cluster. 
        /// If address is zero, it simply creates an empty cluster.</param>
        /// <param name="indexValue">the index value of this first block</param>
        /// <param name="destinationClusterAddress">the first block of the destination cluster</param>
        private void ShadowCopyDataCluster(int sourceClusterAddress, int indexValue, int destinationClusterAddress)
        {
            IMemoryUnit sourceData = m_diskIo.GetMemoryUnit();
            IMemoryUnit destinationData = m_diskIo.GetMemoryUnit();
            int fileIdNumber = m_fileMetaData.FileIdNumber;
            int snapshotSequenceNumber = m_fileAllocationTable.SnapshotSequenceNumber;

            //if source exist
            if (sourceClusterAddress != 0)
            {
                IoReadState readState;
                readState = m_diskIo.AquireBlockForRead(sourceClusterAddress, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber - 1, sourceData);
                if (readState != IoReadState.Valid)
                    throw new Exception("Error Reading File " + readState.ToString());

                m_diskIo.AquireBlockForWrite(destinationClusterAddress, destinationData);
                Memory.Copy(sourceData.Pointer, destinationData.Pointer, sourceData.Length);
            }
            else //if source cluster does not exist.
            {
                m_diskIo.AquireBlockForWrite(destinationClusterAddress, destinationData);
                Memory.Clear(destinationData.Pointer, destinationData.Length);
            }
            sourceData.Dispose();
            destinationData.Dispose();
        }

        #endregion

    }
}
