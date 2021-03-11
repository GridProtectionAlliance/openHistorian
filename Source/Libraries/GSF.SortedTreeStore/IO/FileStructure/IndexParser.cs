//******************************************************************************************************
//  IndexParser.cs - Gbtc
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
//  1/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.IO.FileStructure.Media;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// This class provides passthrough properties for the <see cref="IndexMapper"/> class as well follows the directions
    /// of the Index Mapper to find the data cluster that contains the point in question.
    /// </summary>
    internal unsafe class IndexParser
        : IndexMapper
    {
        #region [ Members ]

        /// <summary>
        /// The file that is being read by this parser.
        /// </summary>
        private readonly SubFileHeader m_subFile;

        private readonly SubFileDiskIoSessionPool m_ioSessions;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="ioSessions">IoSessions to use to read from this disk</param>
        public IndexParser(SubFileDiskIoSessionPool ioSessions)
            : base(ioSessions.Header.BlockSize)
        {
            m_subFile = ioSessions.File;
            m_ioSessions = ioSessions;
            m_oldFirstOffset = -1;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The address of the first indirect block
        /// </summary>
        protected uint FirstIndirectBlockAddress;

        /// <summary>
        /// The address of the second indirect block
        /// </summary>
        protected uint SecondIndirectBlockAddress;

        /// <summary>
        /// The address of the third indirect block
        /// </summary>
        protected uint ThirdIndirectBlockAddress;

        /// <summary>
        /// The address of the third indirect block
        /// </summary>
        protected uint FourthIndirectBlockAddress;

        /// <summary>
        /// The address of the first block of the data cluster.
        /// </summary>
        protected uint DataClusterAddress;

        /// <summary>
        /// The address of the first indirect block
        /// </summary>
        private int m_oldFirstOffset;

        /// <summary>
        /// The address of the second indirect block
        /// </summary>
        private int m_oldSecondOffset;

        /// <summary>
        /// The address of the third indirect block
        /// </summary>
        private int m_oldThirdOffset;

        /// <summary>
        /// The address of the fourth indirect block
        /// </summary>
        private int m_oldFourthOffset;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// This function will also call <see cref="IndexMapper.MapPosition"/> so after it returns, the current block data will be updated.
        /// </summary>
        /// <param name="positionIndex">The virtual index address.</param>
        /// <returns>the physical position index for the provided virtual position</returns>
        public uint VirtualToPhysical(uint positionIndex)
        {
            MapPosition(positionIndex);
            UpdateBlockInformation();
            return DataClusterAddress;
        }

        /// <summary>
        /// Determines if the current sector contains the position passed. If not, it updates the current sector to the one that contains the passed position.
        /// </summary>
        /// <param name="positionIndex">The position to navigate to indexed to the block data block size.</param>
        public void SetPositionAndLookup(uint positionIndex)
        {
            MapPosition(positionIndex);
            UpdateBlockInformation();
        }
        /// <summary>
        /// Resets the index cache with the information from the supplied <see cref="mostRecentParser"/>
        /// </summary>
        /// <param name="mostRecentParser"></param>
        public void ClearIndexCache(IndexParser mostRecentParser)
        {
            FirstIndirectBlockAddress = mostRecentParser.FirstIndirectBlockAddress;
            SecondIndirectBlockAddress = mostRecentParser.SecondIndirectBlockAddress;
            ThirdIndirectBlockAddress = mostRecentParser.ThirdIndirectBlockAddress;
            FourthIndirectBlockAddress = mostRecentParser.FourthIndirectBlockAddress;
            DataClusterAddress = mostRecentParser.DataClusterAddress;
            MapPosition(mostRecentParser.BaseVirtualAddressIndexValue);
            m_oldFirstOffset = -1;
        }

        /// <summary>
        /// Looks up the physical/virtual block positions for the address given.
        /// </summary>
        /// <returns></returns>
        private void UpdateBlockInformation()
        {
            int lowestChange;
            if (m_oldFirstOffset != FirstIndirectOffset)
                lowestChange = 1;
            else if (m_oldSecondOffset != SecondIndirectOffset)
                lowestChange = 2;
            else if (m_oldThirdOffset != ThirdIndirectOffset)
                lowestChange = 3;
            else if (m_oldFourthOffset != FourthIndirectOffset)
                lowestChange = 4;
            else
            {
                lowestChange = 5;
                //DataClusterAddress = m_subFile.DirectBlock;
                return;
            }

            m_oldFirstOffset = FirstIndirectOffset;
            m_oldSecondOffset = SecondIndirectOffset;
            m_oldThirdOffset = ThirdIndirectOffset;
            m_oldFourthOffset = FourthIndirectOffset;

            if (FirstIndirectOffset != 0) //Quadruple Indirect
            {
                FirstIndirectBlockAddress = m_subFile.QuadrupleIndirectBlock;
                if (lowestChange <= 1)
                    SecondIndirectBlockAddress = GetBlockIndexValue(FirstIndirectBlockAddress, FirstIndirectOffset, BlockType.IndexIndirect1, FirstIndirectBaseIndex);
                if (lowestChange <= 2)
                    ThirdIndirectBlockAddress = GetBlockIndexValue(SecondIndirectBlockAddress, SecondIndirectOffset, BlockType.IndexIndirect2, SecondIndirectBaseIndex);
                if (lowestChange <= 3)
                    FourthIndirectBlockAddress = GetBlockIndexValue(ThirdIndirectBlockAddress, ThirdIndirectOffset, BlockType.IndexIndirect3, ThirdIndirectBaseIndex);
                if (lowestChange <= 4)
                    DataClusterAddress = GetBlockIndexValue(FourthIndirectBlockAddress, FourthIndirectOffset, BlockType.IndexIndirect4, FourthIndirectBaseIndex);
            }
            else if (SecondIndirectOffset != 0) //Triple Indirect
            {
                FirstIndirectBlockAddress = 0;
                SecondIndirectBlockAddress = m_subFile.TripleIndirectBlock;
                if (lowestChange <= 2)
                    ThirdIndirectBlockAddress = GetBlockIndexValue(SecondIndirectBlockAddress, SecondIndirectOffset, BlockType.IndexIndirect2, SecondIndirectBaseIndex);
                if (lowestChange <= 3)
                    FourthIndirectBlockAddress = GetBlockIndexValue(ThirdIndirectBlockAddress, ThirdIndirectOffset, BlockType.IndexIndirect3, ThirdIndirectBaseIndex);
                if (lowestChange <= 4)
                    DataClusterAddress = GetBlockIndexValue(FourthIndirectBlockAddress, FourthIndirectOffset, BlockType.IndexIndirect4, FourthIndirectBaseIndex);
            }
            else if (ThirdIndirectOffset != 0) //Double Indirect
            {
                FirstIndirectBlockAddress = 0;
                SecondIndirectBlockAddress = 0;
                ThirdIndirectBlockAddress = m_subFile.DoubleIndirectBlock;
                if (lowestChange <= 3)
                    FourthIndirectBlockAddress = GetBlockIndexValue(ThirdIndirectBlockAddress, ThirdIndirectOffset, BlockType.IndexIndirect3, ThirdIndirectBaseIndex);
                if (lowestChange <= 4)
                    DataClusterAddress = GetBlockIndexValue(FourthIndirectBlockAddress, FourthIndirectOffset, BlockType.IndexIndirect4, FourthIndirectBaseIndex);
            }
            else if (FourthIndirectOffset != 0) //Single Indirect
            {
                FirstIndirectBlockAddress = 0;
                SecondIndirectBlockAddress = 0;
                ThirdIndirectBlockAddress = 0;
                FourthIndirectBlockAddress = m_subFile.SingleIndirectBlock;
                if (lowestChange <= 4)
                    DataClusterAddress = GetBlockIndexValue(FourthIndirectBlockAddress, FourthIndirectOffset, BlockType.IndexIndirect4, FourthIndirectBaseIndex);
            }
            else //Immediate
            {
                FirstIndirectBlockAddress = 0;
                SecondIndirectBlockAddress = 0;
                ThirdIndirectBlockAddress = 0;
                FourthIndirectBlockAddress = 0;
                DataClusterAddress = m_subFile.DirectBlock;
            }
        }

        /// <summary>
        /// This uses the (blockIndex,offset) values to determine what the next block index is.
        /// This also has consistency checks to determine if the file is inconsistent (potentially corruption)
        /// </summary>
        /// <param name="blockIndex">the index of the block to read</param>
        /// <param name="offset">the offset inside the block to use to determine the next index block</param>
        /// <param name="blockType">the value 1-4 which tell what indirect block this is</param>
        /// <param name="blockBaseIndex">the lowest virtual address that can be referenced from this indirect block</param>
        /// <returns></returns>
        private uint GetBlockIndexValue(uint blockIndex, int offset, BlockType blockType, uint blockBaseIndex)
        {
            DiskIoSession buffer = m_ioSessions.SourceIndex;
            if (blockIndex == 0)
                return 0;

            //Skip the redundant read if this block is still cached.
            if (!buffer.IsValid || buffer.BlockIndex != blockIndex)
            {
                buffer.Read(blockIndex, blockType, blockBaseIndex);
            }
            return *(uint*)(buffer.Pointer + (offset << 2));
        }

        #endregion
    }
}