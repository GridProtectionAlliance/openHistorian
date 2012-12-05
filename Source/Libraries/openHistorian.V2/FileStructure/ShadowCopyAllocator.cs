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

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// This class will make shadow copies of blocks or, if the block has never been written to, prepare the block to be written to.
    /// </summary>
    unsafe internal class ShadowCopyAllocator : IDisposable
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
        SubFileMetaData m_subFileMetaData;
        /// <summary>
        /// The disk to make the IO requests to.
        /// </summary>
        //DiskIo m_diskIo;
        /// <summary>
        /// The FileAllocationTable that can be used to allocate space.
        /// </summary>
        FileHeaderBlock m_fileHeaderBlock;
        /// <summary>
        /// A parser that is used to navigate to this block.
        /// This parser is in common with the one used in the <see cref="SubFileAddressTranslation"/>.
        /// </summary>
        IndexParser m_parser;

        DiskIoSession m_diskIo1;

        DiskIoSession m_diskIo2;

        int m_blockSize;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="ShadowCopyAllocator"/> that is used make shadow copies of blocks.
        /// </summary>
        /// <param name="blockSize">The number of bytes in a block</param>
        /// <param name="dataReader">A DiskIO that allows writing to the file.</param>
        /// <param name="fileHeaderBlock">The file allocation table that is editable</param>
        /// <param name="subFileMetaData">The file that is used</param>
        /// <param name="parser">The indexparser used by the caller to designate what block needs to be copied.</param>
        public ShadowCopyAllocator(int blockSize, DiskIo dataReader, FileHeaderBlock fileHeaderBlock, SubFileMetaData subFileMetaData, IndexParser parser)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            if (fileHeaderBlock == null)
                throw new ArgumentNullException("fileHeaderBlock");
            if (subFileMetaData == null)
                throw new ArgumentNullException("subFileMetaData");
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (dataReader.IsReadOnly)
                throw new ArgumentException("DataReader is read only", "dataReader");
            if (fileHeaderBlock.IsReadOnly)
                throw new ArgumentException("FileAllocationTable is read only", "fileHeaderBlock");
            if (subFileMetaData.IsReadOnly)
                throw new ArgumentException("FileMetaData is read only", "subFileMetaData");
            m_blockSize = blockSize;
            m_parser = parser;
            m_lastReadOnlyBlock = fileHeaderBlock.LastAllocatedBlock;
            m_fileHeaderBlock = fileHeaderBlock;
            m_subFileMetaData = subFileMetaData;
            //m_diskIo = dataReader;
            m_diskIo1 = dataReader.CreateDiskIoSession();
            m_diskIo2 = dataReader.CreateDiskIoSession();
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
                    m_subFileMetaData.DirectBlock = dataBlockAddress;
                    break;
                case 1:
                    firstIndirectAddress = ShadowCopyIndexIndirect(m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, dataBlockAddress);
                    m_subFileMetaData.SingleIndirectBlock = firstIndirectAddress;
                    break;
                case 2:
                    secondIndirectAddress = ShadowCopyIndexIndirect(m_parser.SecondIndirectBlockAddress, m_parser.SecondIndirectBaseIndex, 2, m_parser.SecondIndirectOffset, dataBlockAddress);
                    firstIndirectAddress = ShadowCopyIndexIndirect(m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, secondIndirectAddress);
                    m_subFileMetaData.DoubleIndirectBlock = firstIndirectAddress;
                    break;
                case 3:
                    thirdIndirectAddress = ShadowCopyIndexIndirect(m_parser.ThirdIndirectBlockAddress, m_parser.ThirdIndirectBaseIndex, 3, m_parser.ThirdIndirectOffset, dataBlockAddress);
                    secondIndirectAddress = ShadowCopyIndexIndirect(m_parser.SecondIndirectBlockAddress, m_parser.SecondIndirectBaseIndex, 2, m_parser.SecondIndirectOffset, thirdIndirectAddress);
                    firstIndirectAddress = ShadowCopyIndexIndirect(m_parser.FirstIndirectBlockAddress, m_parser.FirstIndirectBaseIndex, 1, m_parser.FirstIndirectOffset, secondIndirectAddress);
                    m_subFileMetaData.TripleIndirectBlock = firstIndirectAddress;
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
                var buffer = m_diskIo1;
                indexIndirectBlock = m_fileHeaderBlock.AllocateFreeBlocks(1);
                m_subFileMetaData.TotalBlockCount++;

                buffer.BeginWriteToNewBlock(indexIndirectBlock);
                Memory.Clear(buffer.Pointer, buffer.Length);
                WriteIndexIndirectBlock(buffer.Pointer, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                buffer.EndWrite(BlockType.IndexIndirect, indexValue, m_subFileMetaData.FileIdNumber, m_fileHeaderBlock.SnapshotSequenceNumber);
            }
            //if the data page is an old page, allocate space to create a new copy
            else if (sourceBlockAddress <= m_lastReadOnlyBlock)
            {
                indexIndirectBlock = m_fileHeaderBlock.AllocateFreeBlocks(1);
                m_subFileMetaData.TotalBlockCount++;

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
            var bufferSource = m_diskIo1;
            int fileIdNumber = m_subFileMetaData.FileIdNumber;
            int snapshotSequenceNumber = m_fileHeaderBlock.SnapshotSequenceNumber;

            if (sourceBlockAddress == destinationBlockAddress)
                bufferSource.Read(sourceBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
            else
                bufferSource.Read(sourceBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber - 1);

            if (bufferSource.Pointer[m_blockSize - 31] != indexIndirectNumber)
                throw new Exception("The redirect value of this page is incorrect");


            //we only need to update the base address if something has changed.
            //Therefore, if the source and the destination are the same, and the remote block is the same
            //everything else is going to be the same.
            if (sourceBlockAddress != destinationBlockAddress)
            {
                var destination = m_diskIo2;
                destination.BeginWriteToNewBlock(destinationBlockAddress);
                Memory.Copy(bufferSource.Pointer, destination.Pointer, destination.Length);
                WriteIndexIndirectBlock(destination.Pointer, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                destination.EndWrite(BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
            }
            else if (*(int*)(bufferSource.Pointer + remoteAddressOffset) != remoteBlockAddress)
            {
                bufferSource.BeginWriteToExistingBlock(destinationBlockAddress, BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
                WriteIndexIndirectBlock(bufferSource.Pointer, indexIndirectNumber, remoteAddressOffset, remoteBlockAddress);
                bufferSource.EndWrite(BlockType.IndexIndirect, indexValue, fileIdNumber, snapshotSequenceNumber);
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
            pointer[m_blockSize - 31] = indexIndirectNumber;
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
                dataBlockAddress = m_fileHeaderBlock.AllocateFreeBlocks(1);
                if (m_parser.DataClusterAddress == 0)
                    m_subFileMetaData.DataBlockCount++;

                m_subFileMetaData.TotalBlockCount++;
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
            int fileIdNumber = m_subFileMetaData.FileIdNumber;
            int snapshotSequenceNumber = m_fileHeaderBlock.SnapshotSequenceNumber;

            //if source exist
            if (sourceClusterAddress != 0)
            {
                DiskIoSession destinationData = m_diskIo1;
                DiskIoSession sourceData = m_diskIo2;
                sourceData.Read(sourceClusterAddress, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber - 1);
                destinationData.BeginWriteToNewBlock(destinationClusterAddress);
                Memory.Copy(sourceData.Pointer, destinationData.Pointer, sourceData.Length);
                destinationData.EndWrite(BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber - 1);
            }
            //if source cluster does not exist.
            else
            {
                m_diskIo1.WriteZeroesToNewBlock(destinationClusterAddress, BlockType.DataBlock, indexValue, fileIdNumber, snapshotSequenceNumber);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (m_diskIo1 != null)
                m_diskIo1.Dispose();
            m_diskIo1 = null;
            if (m_diskIo2 != null)
                m_diskIo2.Dispose();
            m_diskIo2 = null;
        }
    }
}
