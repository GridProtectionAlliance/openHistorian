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
//******************************************************************************************************

using System;

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// This class is used to convert the position of a file into a set of directions 
    /// that <see cref="IndexParser"/> can use to lookup the data cluster.
    /// </summary>
    internal class IndexMapper
    {
        #region [ Members ]

        int m_blockDataLength;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a index mapper that is based on a given cluster size,
        /// </summary>
        public IndexMapper(int blockSize)
        {
            m_blockDataLength = blockSize - FileStructureConstants.BlockFooterLength;

            //initializes all of the values
            SetPosition(0);
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the number of indirects that must be parsed to get to the data cluster.
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple
        /// </summary>
        public int IndirectNumber { get; private set; }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the first indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (int))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int FirstIndirectOffset { get; private set; }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the second indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (int))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int SecondIndirectOffset { get; private set; }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the third indirect block. This address is an absolute offset and has already been multiplied by
        /// 4 (the size of (int))
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int ThirdIndirectOffset { get; private set; }

        /// <summary>
        /// Gets the index of the first cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public int FirstIndirectBaseIndex { get; private set; }

        /// <summary>
        /// Gets the index of the second cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public int SecondIndirectBaseIndex { get; private set; }

        /// <summary>
        /// Gets the index of the third cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        public int ThirdIndirectBaseIndex { get; private set; }

        /// <summary>
        /// Determines the block index value that will be stored in the footer of the data block.
        /// </summary>
        public int BaseVirtualAddressIndexValue { get; private set; }

        /// <summary>
        /// Returns the first address that can be referenced by this cluster.
        /// </summary>
        public long BaseVirtualAddress { get; private set; }

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
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple, 4=NoChange
        /// </returns>
        public int SetPosition(long position)
        {
            int addressesPerBlock = m_blockDataLength >> 2;
            const int firstSingleIndirectBlockIndex = 1;
            int firstDoubleIndirectBlockIndex = firstSingleIndirectBlockIndex + addressesPerBlock;
            int firstTripleIndirectIndex = (int)Math.Min(int.MaxValue, firstDoubleIndirectBlockIndex + (long)addressesPerBlock * (long)addressesPerBlock);
            int lastAddressableBlockIndex = (int)Math.Min(int.MaxValue, firstTripleIndirectIndex + (long)addressesPerBlock * (long)addressesPerBlock * (long)addressesPerBlock - 1);
            int lowestChange = 4;

            if (position < 0)
                throw new ArgumentException("Position cannot be negative", "position");

            //the index if the data block
            long indexNumberLng = position / m_blockDataLength;

            if (indexNumberLng >= int.MaxValue)
                throw new IndexOutOfRangeException("Reading outside the bounds of the feature is not supported");

            //Divide by and mod of an int is quite a bit faster.
            int indexNumber = (int)indexNumberLng;

            BaseVirtualAddress = (long)indexNumber * m_blockDataLength;
            BaseVirtualAddressIndexValue = indexNumber;

            if (indexNumber < firstSingleIndirectBlockIndex) //immediate
            {
                SetIndirectNumber(0, ref lowestChange);
                SetFirstIndirectOffset(-1, ref lowestChange);
                SetSecondIndirectOffset(-1, ref lowestChange);
                SetThirdIndirectOffset(-1, ref lowestChange);
            }
            else if (indexNumber < firstDoubleIndirectBlockIndex) //single redirect
            {
                SetIndirectNumber(1, ref lowestChange);
                indexNumber -= firstSingleIndirectBlockIndex;
                SetFirstIndirectOffset((indexNumber) << 2, ref lowestChange);
                SetSecondIndirectOffset(-1, ref lowestChange);
                SetThirdIndirectOffset(-1, ref lowestChange);
            }
            else if (indexNumber < firstTripleIndirectIndex) //double redirect
            {
                SetIndirectNumber(2, ref lowestChange);
                indexNumber -= firstDoubleIndirectBlockIndex;
                SetFirstIndirectOffset((indexNumber / addressesPerBlock) << 2, ref lowestChange);
                SetSecondIndirectOffset((indexNumber % addressesPerBlock) << 2, ref lowestChange);
                SetThirdIndirectOffset(-1, ref lowestChange);
            }
            else if (indexNumber <= lastAddressableBlockIndex) //triple
            {
                SetIndirectNumber(3, ref lowestChange);
                indexNumber -= firstTripleIndirectIndex;
                SetFirstIndirectOffset((indexNumber / (addressesPerBlock * addressesPerBlock)) << 2, ref lowestChange);
                SetSecondIndirectOffset((indexNumber / addressesPerBlock % addressesPerBlock) << 2, ref lowestChange);
                SetThirdIndirectOffset((indexNumber % addressesPerBlock) << 2, ref lowestChange);
            }
            else
            {
                throw new Exception("Position goes beyond the valid address space of the inode");
            }

            switch (IndirectNumber)
            {
                case 0:
                    FirstIndirectBaseIndex = 0;
                    SecondIndirectBaseIndex = 0;
                    ThirdIndirectBaseIndex = 0;
                    break;
                case 1:
                    FirstIndirectBaseIndex = firstSingleIndirectBlockIndex;
                    SecondIndirectBaseIndex = 0;
                    ThirdIndirectBaseIndex = 0;
                    break;
                case 2:
                    FirstIndirectBaseIndex = firstSingleIndirectBlockIndex;
                    SecondIndirectBaseIndex = (FirstIndirectBaseIndex + addressesPerBlock * (FirstIndirectOffset >> 2));
                    ThirdIndirectBaseIndex = 0;
                    break;
                case 3:
                    FirstIndirectBaseIndex = firstSingleIndirectBlockIndex;
                    SecondIndirectBaseIndex = (FirstIndirectBaseIndex + addressesPerBlock * addressesPerBlock * (FirstIndirectOffset >> 2));
                    ThirdIndirectBaseIndex = (SecondIndirectBaseIndex + addressesPerBlock * (SecondIndirectOffset >> 2));
                    break;
                default:
                    throw new Exception();
            }

            return lowestChange;
        }

        void SetIndirectNumber(int value, ref int lowestChange)
        {
            if (IndirectNumber != value)
            {
                IndirectNumber = value;
                lowestChange = Math.Min(0, lowestChange);
            }
        }
        void SetFirstIndirectOffset(int value, ref int lowestChange)
        {
            if (FirstIndirectOffset != value)
            {
                FirstIndirectOffset = value;
                lowestChange = Math.Min(1, lowestChange);
            }
        }
        void SetSecondIndirectOffset(int value, ref int lowestChange)
        {
            if (SecondIndirectOffset != value)
            {
                SecondIndirectOffset = value;
                lowestChange = Math.Min(2, lowestChange);
            }
        }
        void SetThirdIndirectOffset(int value, ref int lowestChange)
        {
            if (ThirdIndirectOffset != value)
            {
                ThirdIndirectOffset = value;
                lowestChange = Math.Min(3, lowestChange);
            }
        }

        #endregion

    }
}
