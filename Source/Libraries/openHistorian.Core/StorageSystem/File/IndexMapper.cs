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

namespace openHistorian.V2.StorageSystem.File
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
        int m_lowestChange;
        /// <summary>
        /// The number of data bytes that are contained per address in the index indirect pages.
        /// </summary>
        long m_dataPerCluster;
        /// <summary>
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple
        /// </summary>
        int m_indirectNumber;
        /// <summary>
        /// The offset inside the first indirect block
        /// that contains the address for the second indirect block or data cluster if IndirectNumber = 1
        /// </summary>
        int m_firstIndirectOffset;
        /// <summary>
        /// The offset inside the second indirect block
        /// that contains the address for the third indirect block or data cluster if IndirectNumber = 2
        /// </summary>
        int m_secondIndirectOffset;
        /// <summary>
        /// The offset inside the third indirect block
        /// that contains the address for the forth indirect block or data cluster if IndirectNumber = 3
        /// </summary>
        int m_thirdIndirectOffset;
        /// <summary>
        /// The offset inside the forth indirect block
        /// that contains the address for the data cluster if IndirectNumber = 4
        /// </summary>
        int m_forthIndirectOffset;
        /// <summary>
        /// The value of the first data block index that can be referenced using this indirect block.
        /// </summary>
        uint m_firstIndirectBaseIndex;
        /// <summary>
        /// The value of the first data cluster index that can be referenced using this indirect block.
        /// </summary>
        uint m_secondIndirectBaseIndex;
        /// <summary>
        /// The value of the first data cluster index that can be referenced using this indirect block.
        /// </summary>
        uint m_thirdIndirectBaseIndex;
        /// <summary>
        /// The value of the first data cluster index that can be referenced using this indirect block.
        /// </summary>
        uint m_forthIndirectBaseIndex;
        /// <summary>
        /// The first address that can be referenced in this cluster
        /// </summary>
        long m_baseVirtualAddress;
        /// <summary>
        /// The index value of the base virtual address.
        /// </summary>
        uint m_baseVirtualAddressIndexValue;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a index mapper that is based on a given cluster size,
        /// </summary>
        public IndexMapper()
        {
            m_dataPerCluster = ArchiveConstants.DataBlockDataLength;
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
                return m_indirectNumber;
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
                return m_firstIndirectOffset;
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
                return m_secondIndirectOffset;
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
                return m_thirdIndirectOffset;
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
                return m_forthIndirectOffset;
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
                return m_firstIndirectBaseIndex;
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
                return m_secondIndirectBaseIndex;
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
                return m_thirdIndirectBaseIndex;
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
                return m_forthIndirectBaseIndex;
            }
        }
        /// <summary>
        /// Returns the length of the virtual base address for which this lookup map is valid.
        /// </summary>
        public long Length
        {
            get
            {
                return m_dataPerCluster;
            }
        }
        /// <summary>
        /// Determines the block index value that will be stored in the footer of the data block.
        /// </summary>
        public uint BaseVirtualAddressIndexValue
        {
            get
            {
                return m_baseVirtualAddressIndexValue;
            }
        }
        /// <summary>
        /// Returns the first address that can be referenced by this cluster.
        /// </summary>
        public long BaseVirtualAddress
        {
            get
            {
                return m_baseVirtualAddress;
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
            m_lowestChange = 5;

            if (position < 0)
                throw new ArgumentException("Position cannot be negative", "position");

            //the index if the data block
            long indexNumber = position / m_dataPerCluster;

            if (indexNumber >= uint.MaxValue)
                throw new IndexOutOfRangeException("Reading outside the bounds of the feature is not supported");

            m_baseVirtualAddress = indexNumber * m_dataPerCluster;
            m_baseVirtualAddressIndexValue = (uint)indexNumber;

            if (indexNumber < ArchiveConstants.FirstSingleIndirectBlockIndex) //immediate
            {
                SetIndirectNumber(0);
                SetFirstIndirectOffset(-1);
                SetSecondIndirectOffset(-1);
                SetThirdIndirectOffset(-1);
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.FirstDoubleIndirectBlockIndex) //single redirect
            {
                SetIndirectNumber(1);
                indexNumber -= ArchiveConstants.FirstSingleIndirectBlockIndex;
                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber));
                SetSecondIndirectOffset(-1);
                SetThirdIndirectOffset(-1);
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.FirstTripleIndirectIndex) //double redirect
            {
                SetIndirectNumber(2);
                indexNumber -= ArchiveConstants.FirstDoubleIndirectBlockIndex;

                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlock));
                SetSecondIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber % ArchiveConstants.AddressesPerBlock));
                SetThirdIndirectOffset(-1);
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.FirstQuadrupleIndirectBlockIndex) //triple
            {
                SetIndirectNumber(3);
                indexNumber -= ArchiveConstants.FirstTripleIndirectIndex;

                SetFirstIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlockSquare));
                SetSecondIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber / ArchiveConstants.AddressesPerBlock % ArchiveConstants.AddressesPerBlock));
                SetThirdIndirectOffset(CalculateRelativeIndexForIndexPosition(indexNumber % ArchiveConstants.AddressesPerBlock));
                SetForthIndirectOffset(-1);
            }
            else if (indexNumber < ArchiveConstants.LastValidQuadrupleIndirectBlock)
            {
                SetIndirectNumber(4);
                indexNumber -= ArchiveConstants.FirstQuadrupleIndirectBlockIndex;

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
            return m_lowestChange;
        }

        private void SetIndirectNumber(int value)
        {
            if (m_indirectNumber != value)
            {
                m_indirectNumber = value;
                m_lowestChange = Math.Min(0, m_lowestChange);
            }
        }
        private void SetFirstIndirectOffset(int value)
        {
            if (m_firstIndirectOffset != value)
            {
                m_firstIndirectOffset = value;
                m_lowestChange = Math.Min(1, m_lowestChange);
            }
        }
        private void SetSecondIndirectOffset(int value)
        {
            if (m_secondIndirectOffset != value)
            {
                m_secondIndirectOffset = value;
                m_lowestChange = Math.Min(2, m_lowestChange);
            }
        }
        private void SetThirdIndirectOffset(int value)
        {
            if (m_thirdIndirectOffset != value)
            {
                m_thirdIndirectOffset = value;
                m_lowestChange = Math.Min(3, m_lowestChange);
            }
        }
        private void SetForthIndirectOffset(int value)
        {
            if (m_forthIndirectOffset != value)
            {
                m_forthIndirectOffset = value;
                m_lowestChange = Math.Min(4, m_lowestChange);
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
                    m_firstIndirectBaseIndex = 0;
                    m_secondIndirectBaseIndex = 0;
                    m_thirdIndirectBaseIndex = 0;
                    m_forthIndirectBaseIndex = 0;
                    break;
                case 1:
                    m_firstIndirectBaseIndex = ArchiveConstants.FirstSingleIndirectBlockIndex;
                    m_secondIndirectBaseIndex = 0;
                    m_thirdIndirectBaseIndex = 0;
                    m_forthIndirectBaseIndex = 0;
                    break;
                case 2:
                    m_firstIndirectBaseIndex = ArchiveConstants.FirstSingleIndirectBlockIndex;
                    m_secondIndirectBaseIndex = (uint)(m_firstIndirectBaseIndex + ArchiveConstants.AddressesPerBlock * (m_firstIndirectOffset >> 2));
                    m_thirdIndirectBaseIndex = 0;
                    m_forthIndirectBaseIndex = 0;
                    break;
                case 3:
                    m_firstIndirectBaseIndex = ArchiveConstants.FirstSingleIndirectBlockIndex;
                    m_secondIndirectBaseIndex = (uint)(m_firstIndirectBaseIndex + ArchiveConstants.AddressesPerBlockSquare * (m_firstIndirectOffset >> 2));
                    m_thirdIndirectBaseIndex = (uint)(m_secondIndirectBaseIndex + ArchiveConstants.AddressesPerBlock * (m_secondIndirectOffset >> 2));
                    m_forthIndirectBaseIndex = 0;
                    break;
                case 4:
                    m_firstIndirectBaseIndex = ArchiveConstants.FirstSingleIndirectBlockIndex;
                    m_secondIndirectBaseIndex = (uint)(m_firstIndirectBaseIndex + ArchiveConstants.AddressesPerBlockCube * (m_firstIndirectOffset >> 2));
                    m_thirdIndirectBaseIndex = (uint)(m_secondIndirectBaseIndex + ArchiveConstants.AddressesPerBlockSquare * (m_secondIndirectOffset >> 2));
                    m_forthIndirectBaseIndex = (uint)(m_thirdIndirectBaseIndex + ArchiveConstants.AddressesPerBlock * (m_thirdIndirectOffset >> 2));
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
            return (virtualPos >= m_baseVirtualAddress) && (virtualPos < m_baseVirtualAddress + m_dataPerCluster);
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
                throw new ArgumentException("The index position must be less than the number of indexes per page", "index");
            //value = [number of indexes at this level * sizeof(uint)] 
            return (int)index << 2;
        }
        #endregion
    }
}
