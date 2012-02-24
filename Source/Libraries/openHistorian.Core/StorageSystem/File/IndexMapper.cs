//******************************************************************************************************
//  IndexMapper.cs - Gbtc
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

namespace Historian.StorageSystem.File
{
    /// <summary>
    /// This class is used to convert the position of a file into a set of directions that <see cref="IndexParser"/> can use to 
    /// lookup the data cluster.
    /// </summary>
    internal class IndexMapper
    {
        #region [ Members ]
        /// <summary>
        /// Internal variable used by SetPosition:
        /// This determines what has changed in the most recent update request.
        /// The calling classes can use this to determine what lookup information needs to be 
        /// scrapped, and what can be kept.
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple, 4=Quadruple, 5=NoChange
        /// </summary>
        int m_LowestChange;
        /// <summary>
        /// The number of data bytes that are contained per address in the index indirect pages.
        /// </summary>
        long m_DataPerCluster;
        /// <summary>
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple
        /// </summary>
        int m_IndirectNumber;
        /// <summary>
        /// The offset inside the first indirect block
        /// that contains the address for the second indirect block or data cluster if IndirectNumber = 1
        /// </summary>
        int m_FirstIndirectOffset;
        /// <summary>
        /// The offset inside the second indirect block
        /// that contains the address for the third indirect block or data cluster if IndirectNumber = 2
        /// </summary>
        int m_SecondIndirectOffset;
        /// <summary>
        /// The offset inside the third indirect block
        /// that contains the address for the forth indirect block or data cluster if IndirectNumber = 3
        /// </summary>
        int m_ThirdIndirectOffset;
        /// <summary>
        /// The offset inside the forth indirect block
        /// that contains the address for the data cluster if IndirectNumber = 4
        /// </summary>
        int m_ForthIndirectOffset;
        /// <summary>
        /// The value of the first data block index that can be referenced using this indirect block.
        /// </summary>
        uint m_FirstIndirectBaseIndex;
        /// <summary>
        /// The value of the first data cluster index that can be referenced using this indirect block.
        /// </summary>
        uint m_SecondIndirectBaseIndex;
        /// <summary>
        /// The value of the first data cluster index that can be referenced using this indirect block.
        /// </summary>
        uint m_ThirdIndirectBaseIndex;
        /// <summary>
        /// The value of the first data cluster index that can be referenced using this indirect block.
        /// </summary>
        uint m_ForthIndirectBaseIndex;
        /// <summary>
        /// The first address that can be referenced in this cluster
        /// </summary>
        long m_BaseVirtualAddress;
        /// <summary>
        /// The index value of the base virtual address.
        /// </summary>
        uint m_BaseVirtualAddressIndexValue;

        #endregion

        #region [ Constructors ]
        /// <summary>
        /// Creates a index mapper that is based on a given cluster size,
        /// </summary>
        /// <param name="blocksPerCluster">An integer value greater than 0  that can be used to compute the cluster size.</param>
        public IndexMapper(uint blocksPerCluster)
        {
            if (blocksPerCluster == 0)
                throw new ArgumentException("blocksPerCluster", "Value cannot be 0");
            m_DataPerCluster = blocksPerCluster * ArchiveConstants.DataBlockDataLength;
            //initializes all of the values
            SetPosition(0);
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the number of indirects that must be parsed to get to the data cluster.
        /// </summary>
        public int IndirectNumber
        {
            get
            {
                return m_IndirectNumber;
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
                return m_FirstIndirectOffset;
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
                return m_SecondIndirectOffset;
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
                return m_ThirdIndirectOffset;
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
                return m_ForthIndirectOffset;
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
                return m_FirstIndirectBaseIndex;
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
                return m_SecondIndirectBaseIndex;
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
                return m_ThirdIndirectBaseIndex;
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
                return m_ForthIndirectBaseIndex;
            }
        }
        /// <summary>
        /// Returns the length of the virtual base address for which this lookup map is valid.
        /// </summary>
        public long Length
        {
            get
            {
                return m_DataPerCluster;
            }
        }
        /// <summary>
        /// Determines the block index value that will be stored in the footer of the data block.
        /// </summary>
        public uint BaseVirtualAddressIndexValue
        {
            get
            {
                return m_BaseVirtualAddressIndexValue;
            }
        }
        /// <summary>
        /// Returns the first address that can be referenced by this cluster.
        /// </summary>
        public long BaseVirtualAddress
        {
            get
            {
                return m_BaseVirtualAddress;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this class to reflect the path that must be taken to reach the cluster that contains this virtual point
        /// </summary>
        /// <param name="position">The address that is being translated</param>
        /// <returns>
        /// This determines what has changed in the most recent update request.
        /// The calling classes can use this to determine what lookup information needs to be 
        /// scrapped, and what can be kept.
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple, 4=Quadruple, 5=NoChange
        /// </returns>
        public int SetPosition(long position)
        {
            m_LowestChange = 5;

            if (position < 0)
                throw new ArgumentException("position", "Position cannot be negative");

            //the index if the data block
            long indexNumber = position / m_DataPerCluster;

            if (indexNumber >= uint.MaxValue)
                throw new IndexOutOfRangeException("Reading outside the bounds of the feature is not supported");

            m_BaseVirtualAddress = indexNumber * m_DataPerCluster;
            m_BaseVirtualAddressIndexValue = (uint)indexNumber;

            if (indexNumber < ArchiveConstants.LastDirectBlockIndex) //immediate
            {
                SetIndirectNumber(0);
                SetFirstIndirectOffset(-1);
                SetSecondIndirectOffset(-1);
                SetThirdIndirectOffset(-1);
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.LastSingleIndirectBlockIndex) //single redirect
            {
                SetIndirectNumber(1);
                indexNumber -= ArchiveConstants.LastDirectBlockIndex;
                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber));
                SetSecondIndirectOffset(-1);
                SetThirdIndirectOffset(-1);
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.LastDoubleIndirectBlockIndex) //double redirect
            {
                SetIndirectNumber(2);
                indexNumber -= ArchiveConstants.LastSingleIndirectBlockIndex;

                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlock));
                SetSecondIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber % ArchiveConstants.AddressesPerBlock));
                SetThirdIndirectOffset(-1);
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.LastTripleIndirectBlockIndex) //triple
            {
                SetIndirectNumber(3);
                indexNumber -= ArchiveConstants.LastDoubleIndirectBlockIndex;

                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlockSquare));
                SetSecondIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlock % ArchiveConstants.AddressesPerBlock));
                SetThirdIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber % ArchiveConstants.AddressesPerBlock));
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.LastQuadrupleIndirectBlockIndex)
            {
                SetIndirectNumber(4);
                indexNumber -= ArchiveConstants.LastTripleIndirectBlockIndex;

                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlockCube));
                SetSecondIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlockSquare % ArchiveConstants.AddressesPerBlock));
                SetThirdIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlock % ArchiveConstants.AddressesPerBlock));
                SetForthIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber % ArchiveConstants.AddressesPerBlock));
            }
            else
            {
                throw new Exception("Position goes beyond the valid address space of the inode");
            }
            ComputeBaseIndexValues();
            return m_LowestChange;
        }

        private void SetIndirectNumber(int value)
        {
            if (m_IndirectNumber != value)
            {
                m_IndirectNumber = value;
                m_LowestChange = Math.Min(0, m_LowestChange);
            }
        }
        private void SetFirstIndirectOffset(int value)
        {
            if (m_FirstIndirectOffset != value)
            {
                m_FirstIndirectOffset = value;
                m_LowestChange = Math.Min(1, m_LowestChange);
            }
        }
        private void SetSecondIndirectOffset(int value)
        {
            if (m_SecondIndirectOffset != value)
            {
                m_SecondIndirectOffset = value;
                m_LowestChange = Math.Min(2, m_LowestChange);
            }
        }
        private void SetThirdIndirectOffset(int value)
        {
            if (m_ThirdIndirectOffset != value)
            {
                m_ThirdIndirectOffset = value;
                m_LowestChange = Math.Min(3, m_LowestChange);
            }
        }
        private void SetForthIndirectOffset(int value)
        {
            if (m_ForthIndirectOffset != value)
            {
                m_ForthIndirectOffset = value;
                m_LowestChange = Math.Min(4, m_LowestChange);
            }
        }

        /// <summary>
        /// Computes the base index value of every redirect index.
        /// </summary>
        private void ComputeBaseIndexValues()
        {
            switch (IndirectNumber)
            {
                case 0:
                    m_FirstIndirectBaseIndex = 0;
                    m_SecondIndirectBaseIndex = 0;
                    m_ThirdIndirectBaseIndex = 0;
                    m_ForthIndirectBaseIndex = 0;
                    break;
                case 1:
                    m_FirstIndirectBaseIndex = ArchiveConstants.LastDirectBlockIndex;
                    m_SecondIndirectBaseIndex = 0;
                    m_ThirdIndirectBaseIndex = 0;
                    m_ForthIndirectBaseIndex = 0;
                    break;
                case 2:
                    m_FirstIndirectBaseIndex = ArchiveConstants.LastDirectBlockIndex;
                    m_SecondIndirectBaseIndex = (uint)(m_FirstIndirectBaseIndex + ArchiveConstants.AddressesPerBlock * (m_FirstIndirectOffset >> 2));
                    m_ThirdIndirectBaseIndex = 0;
                    m_ForthIndirectBaseIndex = 0;
                    break;
                case 3:
                    m_FirstIndirectBaseIndex = ArchiveConstants.LastDirectBlockIndex;
                    m_SecondIndirectBaseIndex = (uint)(m_FirstIndirectBaseIndex + ArchiveConstants.AddressesPerBlockSquare * (m_FirstIndirectOffset >> 2));
                    m_ThirdIndirectBaseIndex = (uint)(m_SecondIndirectBaseIndex + ArchiveConstants.AddressesPerBlock * (m_SecondIndirectOffset >> 2));
                    m_ForthIndirectBaseIndex = 0;
                    break;
                case 4:
                    m_FirstIndirectBaseIndex = ArchiveConstants.LastDirectBlockIndex;
                    m_SecondIndirectBaseIndex = (uint)(m_FirstIndirectBaseIndex + ArchiveConstants.AddressesPerBlockCube * (m_FirstIndirectOffset >> 2));
                    m_ThirdIndirectBaseIndex = (uint)(m_SecondIndirectBaseIndex + ArchiveConstants.AddressesPerBlockSquare * (m_SecondIndirectOffset >> 2));
                    m_ForthIndirectBaseIndex = (uint)(m_ThirdIndirectBaseIndex + ArchiveConstants.AddressesPerBlock * (m_ThirdIndirectOffset >> 2));
                    break;
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Determines if the current cluster contains the virtual address.
        /// </summary>
        /// <param name="virtualPos">The virtual address to check for.</param>
        /// <returns></returns>
        public bool Containts(long virtualPos)
        {
            return (virtualPos >= m_BaseVirtualAddress) && (virtualPos < m_BaseVirtualAddress + m_DataPerCluster);
        }

        #endregion

        #region [ Static ]

        /// <summary>
        /// Determines the byte position of the page for the index that needs to be referenced.
        /// i.e. Multiplies by 4.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int CalculateRelativeIndexForIndexPosition(long index)
        {
            if (index > ArchiveConstants.AddressesPerBlock)
                throw new ArgumentException("index", "The index position must be less than the number of indexes per page");
            //value = [number of indexes at this level * sizeof(uint)] 
            return (int)index << 2;
        }
        #endregion
    }
}
