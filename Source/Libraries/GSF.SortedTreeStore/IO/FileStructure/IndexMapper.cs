//******************************************************************************************************
//  IndexMapper.cs - Gbtc
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
//******************************************************************************************************

using System;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// This class is used to convert the position of a file into a set of directions 
    /// that <see cref="IndexParser"/> can use to lookup the data cluster.
    /// </summary>
    internal class IndexMapper
    {
        #region [ Members ]

        private readonly uint m_lastAddressableBlockIndex;
        private readonly uint m_addressPerBlock3;
        private readonly uint m_addressPerBlock2;
        private readonly uint m_addressPerBlock;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="IndexMapper"/> that is based on a given <see cref="blockSize"/>.
        /// </summary>
        /// <param name="blockSize">the number of bytes per block. Cannot be less than 64, greater than 1048576</param>
        public IndexMapper(int blockSize)
        {
            if (blockSize < 64)
                throw new ArgumentOutOfRangeException("blockSize", "Cannot be less than 64");
            if (blockSize > 1024 * 1024)
                throw new ArgumentOutOfRangeException("blockSize", "Cannot be greater than 1048576");
            if (!BitMath.IsPowerOfTwo(blockSize))
                throw new Exception("block size must be a power of 2");

            m_addressPerBlock = (uint)(blockSize - FileStructureConstants.BlockFooterLength) >> 2;
            m_addressPerBlock = (uint)Math.Min(uint.MaxValue, (ulong)m_addressPerBlock);
            m_addressPerBlock2 = (uint)Math.Min(uint.MaxValue, m_addressPerBlock * (ulong)m_addressPerBlock);
            m_addressPerBlock3 = (uint)Math.Min(uint.MaxValue, m_addressPerBlock * (ulong)m_addressPerBlock * m_addressPerBlock);
            m_lastAddressableBlockIndex = (uint)Math.Min(uint.MaxValue - 1, m_addressPerBlock * (ulong)m_addressPerBlock * m_addressPerBlock * m_addressPerBlock - 1);
            //initializes all of the values
            MapPosition(0);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines the block index value that will be stored in the footer of the data block.
        /// </summary>
        public uint BaseVirtualAddressIndexValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the first indirect block. 
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int FirstIndirectOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the second indirect block. 
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int SecondIndirectOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the third indirect block. 
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        public int ThirdIndirectOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset position for the address that must be read within the indirect block
        /// at the forth indirect block. 
        /// </summary>
        /// <remarks>Returns a -1 of invalid.  -1 was chosen since it will likely generate an error if not handled properly.</remarks>
        protected int FourthIndirectOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the index of the first cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        protected const uint FirstIndirectBaseIndex = 0;

        /// <summary>
        /// Gets the index of the second cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        protected uint SecondIndirectBaseIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the index of the third cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        protected uint ThirdIndirectBaseIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the index of the third cluster that can be accessed by this indirect block.  This value is useful because 
        /// the footer of the indirect page will have this address.
        /// </summary>
        protected uint FourthIndirectBaseIndex
        {
            get;
            private set;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this class to reflect the path that must be taken to reach the cluster that contains this virtual point
        /// </summary>
        /// <param name="positionIndex">The address that is being translated</param>
        /// <returns>
        /// This determines what has changed in the most recent update request.
        /// The calling classes can use this to determine what lookup information needs to be 
        /// scrapped, and what can be kept.
        /// 0=Immediate, 1=Single, 2=Double, 3=Triple, 4=NoChange
        /// </returns>
        public void MapPosition(uint positionIndex)
        {
            if (positionIndex > m_lastAddressableBlockIndex)
                throw new ArgumentOutOfRangeException("positionIndex", "position is not indexable with the current page size.");

            //uint addressesPerBlock = m_addressesPerBlock;
            uint addressPerBlock3U = m_addressPerBlock3;
            uint addressPerBlock2U = m_addressPerBlock2;
            uint addressPerBlockU = m_addressPerBlock;

            BaseVirtualAddressIndexValue = positionIndex;

            uint firstIndirect;
            uint secondIndirect;
            uint thirdIndirect;

            // Comments work through the provided examples utilizing 3187
            // FirstIndirect = 3
            // SecondIndirect = 1
            // ThirdIndirect = 8
            // FourthIndirect = 7

            if (positionIndex >= addressPerBlock3U)
            {
                //Note, if the position is greater than addressesPerBlock^3 then addressesPerBlock^3 is a 32 bit number.
                firstIndirect = positionIndex / addressPerBlock3U; // 3187/1000, returns 3
                positionIndex -= firstIndirect * addressPerBlock3U;

                secondIndirect = positionIndex / addressPerBlock2U; // 187/100, returns 1
                positionIndex -= secondIndirect * addressPerBlock2U;

                thirdIndirect = positionIndex / addressPerBlockU; // 87/10, returns 8
                positionIndex -= thirdIndirect * addressPerBlockU;

                FirstIndirectOffset = (int)firstIndirect;
                SecondIndirectOffset = (int)secondIndirect;
                ThirdIndirectOffset = (int)thirdIndirect;
                FourthIndirectOffset = (int)positionIndex;

                SecondIndirectBaseIndex = firstIndirect * addressPerBlock3U; //Contains 3000
                ThirdIndirectBaseIndex = SecondIndirectBaseIndex + secondIndirect * addressPerBlock2U; //Contains 3000 + 100
                FourthIndirectBaseIndex = ThirdIndirectBaseIndex + thirdIndirect * addressPerBlockU; //Contains 3000 + 100 + 80
            }
            //Value is now 187
            else if (positionIndex >= addressPerBlock2U)
            {
                secondIndirect = positionIndex / addressPerBlock2U; // 187/100, returns 1
                positionIndex -= secondIndirect * addressPerBlock2U;

                thirdIndirect = positionIndex / addressPerBlockU; // 87/10, returns 8
                positionIndex -= thirdIndirect * addressPerBlockU;

                FirstIndirectOffset = 0;
                SecondIndirectOffset = (int)secondIndirect;
                ThirdIndirectOffset = (int)thirdIndirect;
                FourthIndirectOffset = (int)positionIndex;

                SecondIndirectBaseIndex = 0; //Contains 3000
                ThirdIndirectBaseIndex = secondIndirect * addressPerBlock2U; //Contains 3000 + 100
                FourthIndirectBaseIndex = ThirdIndirectBaseIndex + thirdIndirect * addressPerBlockU; //Contains 3000 + 100 + 80
            }
            //Value is now 87
            else if (positionIndex >= addressPerBlockU)
            {
                thirdIndirect = positionIndex / addressPerBlockU; // 87/10, returns 8
                positionIndex -= thirdIndirect * addressPerBlockU;

                FirstIndirectOffset = 0;
                SecondIndirectOffset = 0;
                ThirdIndirectOffset = (int)thirdIndirect;
                FourthIndirectOffset = (int)positionIndex;

                SecondIndirectBaseIndex = 0;
                ThirdIndirectBaseIndex = 0;
                FourthIndirectBaseIndex = thirdIndirect * addressPerBlockU;
            }
            //Value is now 7
            else if (positionIndex > 0)
            {
                FirstIndirectOffset = 0;
                SecondIndirectOffset = 0;
                ThirdIndirectOffset = 0;
                FourthIndirectOffset = (int)positionIndex;

                SecondIndirectBaseIndex = 0;
                ThirdIndirectBaseIndex = 0;
                FourthIndirectBaseIndex = 0;
            }
            else
            {
                FirstIndirectOffset = 0;
                SecondIndirectOffset = 0;
                ThirdIndirectOffset = 0;
                FourthIndirectOffset = 0;

                SecondIndirectBaseIndex = 0;
                ThirdIndirectBaseIndex = 0;
                FourthIndirectBaseIndex = 0;
            }
        }

        #endregion
    }
}