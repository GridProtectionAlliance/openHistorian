//******************************************************************************************************
//  IndexParser.cs - Gbtc
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

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// This class provides passthrough properties for the <see cref="IndexMapper"/> class as well follows the directions
    /// of the Index Mapper to find the data cluster that contains the point in question.
    /// </summary>
    unsafe internal class IndexParser
    {

        #region [ Members ]

        /// <summary>
        /// Contains a set of temporary buffers that can be used by this index parser 
        /// to minimize the amout of reads that must be issued.
        /// </summary>
        IndexBufferPool m_bufferPool;

        /// <summary>
        /// The address of the first indirect block. 
        /// </summary>
        uint m_firstIndirectBlockAddress;

        /// <summary>
        /// The address of the second indirect block. 
        /// </summary>
        uint m_secondIndirectBlockAddress;

        /// <summary>
        /// The address of the third indirect block. 
        /// </summary>
        uint m_thirdIndirectBlockAddress;

        /// <summary>
        /// The address of the forth indirect block. 
        /// </summary>
        uint m_forthIndirectBlockAddress;
        /// <summary>
        /// The address of the first data block of the data cluster.
        /// </summary>
        uint m_dataClusterAddress;
        /// <summary>
        /// The sequence number that represents the version of the archive that is allowed to be read by this parser
        /// </summary>
        /// <remarks>Internal checks use the sequence number to verify the pages being read are not from a later snapshot.</remarks>
        uint m_snapshotSequenceNumber;
        /// <summary>
        /// The internal index mapper that is encapsolated inside this class.
        /// </summary>
        IndexMapper m_mapping;
        /// <summary>
        /// The file that is being read by this parser.
        /// </summary>
        FileMetaData m_file;
        /// <summary>
        /// The disk to issue read requests to.
        /// </summary>
        DiskIoEnhanced m_dataReader;

        #endregion

        #region [ Constructors ]
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="snapshotSequenceNumber">Contains the sequence number of the snapshot that is
        /// being read or the one that is currently being written to.</param>
        /// <param name="dataReader">The disk that will be read to parse the cluster addresses from the file</param>
        /// <param name="file">The file that is to be read.</param>
        public IndexParser(uint snapshotSequenceNumber, DiskIoEnhanced dataReader, FileMetaData file)
        {
            m_snapshotSequenceNumber = snapshotSequenceNumber;
            m_dataReader = dataReader;
            m_mapping = new IndexMapper();
            m_file = file;
            m_bufferPool = new IndexBufferPool(dataReader);
            //m_TempBlockBuffer = new byte[ArchiveConstants.BlockSize];
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Maintains the most current list of each type of data that can be buffered.
        /// This is to minimize the amount of duplicate lookups.
        /// </summary>
        public IndexBufferPool BufferPool
        {
            get
            {
                return m_bufferPool;
            }
        }
        /// <summary>
        /// The address of the first indirect block
        /// </summary>
        public uint FirstIndirectBlockAddress
        {
            get
            {
                return m_firstIndirectBlockAddress;
            }
        }
        /// <summary>
        /// The address of the second indirect block
        /// </summary>
        public uint SecondIndirectBlockAddress
        {
            get
            {
                return m_secondIndirectBlockAddress;
            }
        }
        /// <summary>
        /// The address of the third indirect block
        /// </summary>
        public uint ThirdIndirectBlockAddress
        {
            get
            {
                return m_thirdIndirectBlockAddress;
            }
        }
        /// <summary>
        /// The address of the forth indirect block 
        /// </summary>
        public uint ForthIndirectBlockAddress
        {
            get
            {
                return m_forthIndirectBlockAddress;
            }
        }
        /// <summary>
        /// The address of the first block of the data cluster.
        /// </summary>
        public uint DataClusterAddress
        {
            get
            {
                return m_dataClusterAddress;
            }
        }
        /// <summary>
        /// Gets the number of indirects that must be parsed to get to the data cluster.
        /// </summary>
        public int IndirectNumber
        {
            get
            {
                return m_mapping.IndirectNumber;
            }
        }
        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the first indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (uint))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int FirstIndirectOffset
        {
            get
            {
                return m_mapping.FirstIndirectOffset;
            }
        }
        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the second indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (uint))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int SecondIndirectOffset
        {
            get
            {
                return m_mapping.SecondIndirectOffset;
            }
        }
        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the third indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (uint))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int ThirdIndirectOffset
        {
            get
            {
                return m_mapping.ThirdIndirectOffset;
            }
        }
        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the forth indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (uint))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int ForthIndirectOffset
        {
            get
            {
                return m_mapping.ForthIndirectOffset;
            }
        }
        /// <summary>
        /// Gets the index of the first cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public uint FirstIndirectBaseIndex
        {
            get
            {
                return m_mapping.FirstIndirectBaseIndex;
            }
        }
        /// <summary>
        /// Gets the index of the second cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public uint SecondIndirectBaseIndex
        {
            get
            {
                return m_mapping.SecondIndirectBaseIndex;
            }
        }
        /// <summary>
        /// Gets the index of the third cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public uint ThirdIndirectBaseIndex
        {
            get
            {
                return m_mapping.ThirdIndirectBaseIndex;
            }
        }
        /// <summary>
        /// Gets the index of the forth cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect block will have this address.
        /// </summary>
        public uint ForthIndirectBaseIndex
        {
            get
            {
                return m_mapping.ForthIndirectBaseIndex;
            }
        }
        /// <summary>
        /// Returns the length of the virtual base address for which this lookup map is valid.
        /// </summary>
        public long Length
        {
            get
            {
                return m_mapping.Length;
            }
        }
        /// <summary>
        /// Determines the block index value that will be stored in the footer of the data block.
        /// </summary>
        public uint BaseVirtualAddressIndexValue
        {
            get
            {
                return m_mapping.BaseVirtualAddressIndexValue;
            }
        }
        /// <summary>
        /// Returns the first address that can be referenced by this cluster.
        /// </summary>
        public long BaseVirtualAddress
        {
            get
            {
                return m_mapping.BaseVirtualAddress;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns the <see cref="PositionData"/> structure for the position passed to this class. 
        /// This function will also call <see cref="SetPosition"/> so after it returns, the current block data will be updated.
        /// </summary>
        /// <param name="position">The address that the <see cref="PositionData"/> structure will contain when returned.</param>
        /// <returns></returns>
        public PositionData GetPositionData(long position)
        {
            SetPosition(position);

            PositionData positionData;
            long offset = position - BaseVirtualAddress;
            uint pageIndex = (uint)(offset / ArchiveConstants.DataBlockDataLength);
            positionData.VirtualPosition = BaseVirtualAddress + pageIndex * ArchiveConstants.DataBlockDataLength;
            if (DataClusterAddress == 0)
                positionData.PhysicalBlockIndex = 0;
            else
                positionData.PhysicalBlockIndex = DataClusterAddress + pageIndex;
            positionData.Length = ArchiveConstants.DataBlockDataLength;
            return positionData;
        }

        /// <summary>
        /// Determines if the current sector contains the position passed. If not, it updates the current sector to the one that contains the passed position.
        /// </summary>
        /// <param name="position">The position to navigate to.</param>
        public void SetPosition(long position)
        {
            UpdateBlockInformation(m_mapping.SetPosition(position));
        }

        /// <summary>
        /// This is only to be called by <see cref="ShadowCopyAllocator"/> when it has made shadow copies of data and it needs to update their addresses.
        /// </summary>
        internal void UpdateAddressesFromShadowCopy(uint dataClusterAddress, uint firstIndirectBlockAddress, uint secondIndirectBlockAddress, uint thirdIndirectBlockAddress, uint forthIndirectBlockAddress)
        {
            m_dataClusterAddress = dataClusterAddress;
            m_firstIndirectBlockAddress = firstIndirectBlockAddress;
            m_secondIndirectBlockAddress = secondIndirectBlockAddress;
            m_thirdIndirectBlockAddress = thirdIndirectBlockAddress;
            m_forthIndirectBlockAddress = forthIndirectBlockAddress;

            //m_bufferPool.FirstIndirect.Address = 0;
            //m_bufferPool.SecondIndirect.Address = 0;
            //m_bufferPool.ThirdIndirect.Address = 0;
            //m_bufferPool.ForthIndirect.Address = 0;
            //m_bufferPool.Data.Address = 0;
        }

        /// <summary>
        /// Looks up the physical/virtual block positions for the address given.
        /// </summary>
        /// <param name="lowestChangeRequest">
        ///  0=Immediate, 1=Single, 2=Double, 3=Triple, 4=Quadruple, 5=NoChange
        /// </param>
        /// <returns></returns>
        private void UpdateBlockInformation(int lowestChangeRequest)
        {

            if (IndirectNumber == 0) //Immediate
            {
                m_firstIndirectBlockAddress = 0;
                m_secondIndirectBlockAddress = 0;
                m_thirdIndirectBlockAddress = 0;
                m_forthIndirectBlockAddress = 0;
                m_dataClusterAddress = m_file.DirectCluster;
            }
            else if (IndirectNumber == 1) //Single Indirect
            {
                m_firstIndirectBlockAddress = m_file.SingleIndirectCluster;
                m_secondIndirectBlockAddress = 0;
                m_thirdIndirectBlockAddress = 0;
                m_forthIndirectBlockAddress = 0;
                if (lowestChangeRequest <= 1) //if the single indirect offset did not change, there is no need to relookup the address
                    m_dataClusterAddress = GetBlockIndexValue(m_bufferPool.FirstIndirect, m_firstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
            }
            else if (IndirectNumber == 2) //Double Indirect
            {
                m_firstIndirectBlockAddress = m_file.DoubleIndirectCluster;
                if (lowestChangeRequest <= 1)
                    m_secondIndirectBlockAddress = GetBlockIndexValue(m_bufferPool.FirstIndirect, m_firstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
                m_thirdIndirectBlockAddress = 0;
                m_forthIndirectBlockAddress = 0;
                if (lowestChangeRequest <= 2)
                    m_dataClusterAddress = GetBlockIndexValue(m_bufferPool.SecondIndirect, m_secondIndirectBlockAddress, m_mapping.SecondIndirectOffset, 2, m_mapping.SecondIndirectBaseIndex);
            }
            else if (IndirectNumber == 3) //Triple Indirect
            {
                m_firstIndirectBlockAddress = m_file.TripleIndirectCluster;
                if (lowestChangeRequest <= 1)
                    m_secondIndirectBlockAddress = GetBlockIndexValue(m_bufferPool.FirstIndirect, m_firstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
                if (lowestChangeRequest <= 2)
                    m_thirdIndirectBlockAddress = GetBlockIndexValue(m_bufferPool.SecondIndirect, m_secondIndirectBlockAddress, m_mapping.SecondIndirectOffset, 2, m_mapping.SecondIndirectBaseIndex);
                m_forthIndirectBlockAddress = 0;
                if (lowestChangeRequest <= 3)
                    m_dataClusterAddress = GetBlockIndexValue(m_bufferPool.ThirdIndirect, m_thirdIndirectBlockAddress, m_mapping.ThirdIndirectOffset, 3, m_mapping.ThirdIndirectBaseIndex);
            }
            else if (IndirectNumber == 4) //Quadruple Indirect
            {
                m_firstIndirectBlockAddress = m_file.QuadrupleIndirectCluster;
                if (lowestChangeRequest <= 1)
                    m_secondIndirectBlockAddress = GetBlockIndexValue(m_bufferPool.FirstIndirect, m_firstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
                if (lowestChangeRequest <= 2)
                    m_thirdIndirectBlockAddress = GetBlockIndexValue(m_bufferPool.SecondIndirect, m_secondIndirectBlockAddress, m_mapping.SecondIndirectOffset, 2, m_mapping.SecondIndirectBaseIndex);
                if (lowestChangeRequest <= 3)
                    m_forthIndirectBlockAddress = GetBlockIndexValue(m_bufferPool.ThirdIndirect, m_thirdIndirectBlockAddress, m_mapping.ThirdIndirectOffset, 3, m_mapping.ThirdIndirectBaseIndex);
                if (lowestChangeRequest <= 4)
                    m_dataClusterAddress = GetBlockIndexValue(m_bufferPool.ForthIndirect, m_forthIndirectBlockAddress, m_mapping.ForthIndirectOffset, 4, m_mapping.ForthIndirectBaseIndex);
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// This uses the (blockIndex,offset) values to determine what the next block index is.
        /// This also has consistency checks to determine if the file is inconsistent (potentially corruption)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="blockIndex">the index of the block to read</param>
        /// <param name="offset">the offset inside the block to use to determine the next index block</param>
        /// <param name="indexIndirectNumber">the value 1-4 which tell what indirect block this is</param>
        /// <param name="blockBaseIndex">the lowest virtual address that can be referenced from this indirect block</param>
        /// <returns></returns>
        private uint GetBlockIndexValue(IMemoryUnit buffer, uint blockIndex, int offset, byte indexIndirectNumber, uint blockBaseIndex)
        {
            if (blockIndex == 0)
                return 0;
            ReadBlockCheckForErrors(buffer, blockIndex, indexIndirectNumber, blockBaseIndex);
            return *(uint*)(buffer.Pointer + offset);
        }

        /// <summary>
        /// This function reads a block from the file and checks to make sure it is the block that was expected to be read.
        /// Errors are thrown if any exceptions are encountered.
        /// </summary>
        /// <param name="buffer">the buffer to use to read data</param>
        /// <param name="blockIndex">the index of the block to read</param>
        /// <param name="indexIndirectNumber">the indirect page number</param>
        /// <param name="indexBaseIndex">the base address of this block</param>
        private void ReadBlockCheckForErrors(IMemoryUnit buffer, uint blockIndex, byte indexIndirectNumber, uint indexBaseIndex)
        {
            if (!buffer.IsValid || buffer.BlockIndex != blockIndex)
            {
                IoReadState readState = m_dataReader.AquireBlockForRead(blockIndex, BlockType.IndexIndirect, indexBaseIndex, m_file.FileIdNumber, m_snapshotSequenceNumber, buffer);

                if (readState != IoReadState.Valid)
                    throw new Exception("Error Reading File " + readState.ToString());
            }
            if (buffer.Pointer[ArchiveConstants.BlockSize - 22] != indexIndirectNumber)
                throw new Exception("The redirect value of this page is incorrect");
        }
        #endregion

    }
}
