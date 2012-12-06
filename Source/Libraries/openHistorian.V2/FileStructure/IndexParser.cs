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

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// This class provides passthrough properties for the <see cref="IndexMapper"/> class as well follows the directions
    /// of the Index Mapper to find the data cluster that contains the point in question.
    /// </summary>
    unsafe internal class IndexParser : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// The sequence number that represents the version of the archive that is allowed to be read by this parser
        /// </summary>
        /// <remarks>Internal checks use the sequence number to verify the pages being read are not from a later snapshot.</remarks>
        int m_snapshotSequenceNumber;
        /// <summary>
        /// The internal index mapper that is encapsolated inside this class.
        /// </summary>
        IndexMapper m_mapping;
        /// <summary>
        /// The file that is being read by this parser.
        /// </summary>
        SubFileMetaData m_subFile;

        DiskIoSession m_diskIo;

        int m_blockSize;

        int m_blockDataLength;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="blockSize">The number of bytes in an individual block</param>
        /// <param name="snapshotSequenceNumber">Contains the sequence number of the snapshot that is
        /// being read or the one that is currently being written to.</param>
        /// <param name="dataReader">The disk that will be read to parse the cluster addresses from the file</param>
        /// <param name="subFile">The file that is to be read.</param>
        public IndexParser(int blockSize, int snapshotSequenceNumber, DiskIo dataReader, SubFileMetaData subFile)
        {
            m_blockSize = blockSize;
            m_blockDataLength = blockSize - FileStructureConstants.BlockFooterLength;
            m_snapshotSequenceNumber = snapshotSequenceNumber;
            m_mapping = new IndexMapper(blockSize);
            m_subFile = subFile;
            m_diskIo = dataReader.CreateDiskIoSession();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// The address of the first indirect block
        /// </summary>
        public int FirstIndirectBlockAddress { get; private set; }

        /// <summary>
        /// The address of the second indirect block
        /// </summary>
        public int SecondIndirectBlockAddress { get; private set; }

        /// <summary>
        /// The address of the third indirect block
        /// </summary>
        public int ThirdIndirectBlockAddress { get; private set; }

        /// <summary>
        /// The address of the first block of the data cluster.
        /// </summary>
        public int DataClusterAddress { get; private set; }

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
        /// 4 (the size of (int))
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
        /// 4 (the size of (int))
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
        /// 4 (the size of (int))
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
        /// Gets the index of the first cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public int FirstIndirectBaseIndex
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
        public int SecondIndirectBaseIndex
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
        public int ThirdIndirectBaseIndex
        {
            get
            {
                return m_mapping.ThirdIndirectBaseIndex;
            }
        }

        /// <summary>
        /// Determines the block index value that will be stored in the footer of the data block.
        /// </summary>
        public int BaseVirtualAddressIndexValue
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
            int pageIndex = (int)(offset / m_blockDataLength);
            positionData.VirtualPosition = BaseVirtualAddress + pageIndex * m_blockDataLength;
            if (DataClusterAddress == 0)
                positionData.PhysicalBlockIndex = 0;
            else
                positionData.PhysicalBlockIndex = DataClusterAddress + pageIndex;
            positionData.Length = m_blockDataLength;
            return positionData;
        }

        /// <summary>
        /// Determines if the current sector contains the position passed. If not, it updates the current sector to the one that contains the passed position.
        /// </summary>
        /// <param name="position">The position to navigate to.</param>
        public void SetPosition(long position)
        {
            int lowestChange = m_mapping.SetPosition(position);
            UpdateBlockInformation(lowestChange);
        }

        /// <summary>
        /// This is only to be called by <see cref="ShadowCopyAllocator"/> when it has made shadow copies of data and it needs to update their addresses.
        /// </summary>
        internal void UpdateAddressesFromShadowCopy(int dataClusterAddress, int firstIndirectBlockAddress, int secondIndirectBlockAddress, int thirdIndirectBlockAddress)
        {
            if (FirstIndirectBlockAddress != firstIndirectBlockAddress
                || SecondIndirectBlockAddress != secondIndirectBlockAddress
                || ThirdIndirectBlockAddress != thirdIndirectBlockAddress)
                m_diskIo.Clear();
            DataClusterAddress = dataClusterAddress;
            FirstIndirectBlockAddress = firstIndirectBlockAddress;
            SecondIndirectBlockAddress = secondIndirectBlockAddress;
            ThirdIndirectBlockAddress = thirdIndirectBlockAddress;
        }

        /// <summary>
        /// Looks up the physical/virtual block positions for the address given.
        /// </summary>
        /// <param name="lowestChangeRequest">
        ///  0=Immediate, 1=Single, 2=Double, 3=Triple, 4=NoChange
        /// </param>
        /// <returns></returns>
        void UpdateBlockInformation(int lowestChangeRequest)
        {
            if (IndirectNumber == 0) //Immediate
            {
                FirstIndirectBlockAddress = 0;
                SecondIndirectBlockAddress = 0;
                ThirdIndirectBlockAddress = 0;
                DataClusterAddress = m_subFile.DirectBlock;
            }
            else if (IndirectNumber == 1) //Single Indirect
            {
                FirstIndirectBlockAddress = m_subFile.SingleIndirectBlock;
                SecondIndirectBlockAddress = 0;
                ThirdIndirectBlockAddress = 0;

                if (lowestChangeRequest <= 1) //if the single indirect offset did not change, there is no need to relookup the address
                    DataClusterAddress = GetBlockIndexValue(FirstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
            }
            else if (IndirectNumber == 2) //Double Indirect
            {
                FirstIndirectBlockAddress = m_subFile.DoubleIndirectBlock;
                if (lowestChangeRequest <= 1)
                    SecondIndirectBlockAddress = GetBlockIndexValue(FirstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
                ThirdIndirectBlockAddress = 0;
                if (lowestChangeRequest <= 2)
                    DataClusterAddress = GetBlockIndexValue(SecondIndirectBlockAddress, m_mapping.SecondIndirectOffset, 2, m_mapping.SecondIndirectBaseIndex);
            }
            else if (IndirectNumber == 3) //Triple Indirect
            {
                FirstIndirectBlockAddress = m_subFile.TripleIndirectBlock;
                if (lowestChangeRequest <= 1)
                    SecondIndirectBlockAddress = GetBlockIndexValue(FirstIndirectBlockAddress, m_mapping.FirstIndirectOffset, 1, m_mapping.FirstIndirectBaseIndex);
                if (lowestChangeRequest <= 2)
                    ThirdIndirectBlockAddress = GetBlockIndexValue(SecondIndirectBlockAddress, m_mapping.SecondIndirectOffset, 2, m_mapping.SecondIndirectBaseIndex);
                if (lowestChangeRequest <= 3)
                    DataClusterAddress = GetBlockIndexValue(ThirdIndirectBlockAddress, m_mapping.ThirdIndirectOffset, 3, m_mapping.ThirdIndirectBaseIndex);
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
        /// <param name="blockIndex">the index of the block to read</param>
        /// <param name="offset">the offset inside the block to use to determine the next index block</param>
        /// <param name="indexIndirectNumber">the value 1-4 which tell what indirect block this is</param>
        /// <param name="blockBaseIndex">the lowest virtual address that can be referenced from this indirect block</param>
        /// <returns></returns>
        int GetBlockIndexValue(int blockIndex, int offset, byte indexIndirectNumber, int blockBaseIndex)
        {
            var buffer = m_diskIo;
            if (blockIndex == 0)
                return 0;

            //Skip the redundant read if this block is still cached.
            if (!buffer.IsValid || buffer.BlockIndex != blockIndex)
            {
                buffer.Read(blockIndex, BlockType.IndexIndirect, blockBaseIndex, m_subFile.FileIdNumber, m_snapshotSequenceNumber);
            }

            if (buffer.Pointer[m_blockSize - 31] != indexIndirectNumber)
                throw new Exception("The redirect value of this page is incorrect");

            int value = *(int*)(buffer.Pointer + offset);
            return value;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (m_diskIo != null)
            {
                m_diskIo.Dispose();
                m_diskIo = null;
            }
        }
    }
}
