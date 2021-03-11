//******************************************************************************************************
//  SubFileHeader.cs - Gbtc
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
//  11/23/2011 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;
using GSF.Immutable;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// This contains the meta data of the file along with index information to map all of the blocks of the file.
    /// </summary>
    public class SubFileHeader
        : ImmutableObjectAutoBase<SubFileHeader>
    {
        #region [ Members ]

        private readonly bool m_isSimplified;
        private readonly SubFileName m_fileName;
        private readonly ushort m_fileIdNumber;
        private uint m_dataBlockCount;
        private uint m_totalBlocksCount;
        private uint m_directBlock;
        private uint m_singleIndirectBlock;
        private uint m_doubleIndirectBlock;
        private uint m_tripleIndirectBlock;
        private uint m_quadrupleIndirectBlock;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="SubFileHeader"/> from the data stream.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="isImmutable">Determines if this class will be immutable upon creation</param>
        /// <param name="isSimplified">gets if the file structure is the simplified type</param>
        public SubFileHeader(BinaryReader dataReader, bool isImmutable, bool isSimplified)
        {
            m_isSimplified = isSimplified;
            m_fileName = SubFileName.Load(dataReader);
            m_fileIdNumber = dataReader.ReadUInt16();
            m_dataBlockCount = dataReader.ReadUInt32();
            if (!isSimplified)
            {
                m_totalBlocksCount = dataReader.ReadUInt32();
            }
            m_directBlock = dataReader.ReadUInt32();
            if (!isSimplified)
            {
                m_singleIndirectBlock = dataReader.ReadUInt32();
                m_doubleIndirectBlock = dataReader.ReadUInt32();
                m_tripleIndirectBlock = dataReader.ReadUInt32();
                m_quadrupleIndirectBlock = dataReader.ReadUInt32();
            }
            IsReadOnly = isImmutable;
        }

        /// <summary>
        /// Creates a new <see cref="SubFileHeader"/>.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        /// <param name="isImmutable">Determines if this class will be immutable upon creation</param>
        /// <param name="isSimplified">if this header is a simplified header.</param>
        public SubFileHeader(ushort fileId, SubFileName fileName, bool isImmutable, bool isSimplified)
        {
            if (fileName is null)
                throw new ArgumentException("The feature type cannot be an empty GUID value", "fileName");
            m_isSimplified = isSimplified;
            IsReadOnly = isImmutable;
            m_fileIdNumber = fileId;
            m_fileName = fileName;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the unique file identifier for this file.
        /// </summary>
        public ushort FileIdNumber => m_fileIdNumber;

        /// <summary>
        /// Gets the <see cref="SubFileName"/> that represents what type of data is contained in this file.
        /// </summary>
        public SubFileName FileName => m_fileName;

        /// <summary>
        /// Gets the number of blocks the data portion of this file contains
        /// </summary>
        public uint DataBlockCount
        {
            get => m_dataBlockCount;
            set
            {
                TestForEditable();
                m_dataBlockCount = value;
            }
        }

        /// <summary>
        /// Gets the total number of blocks that has been used by this file. 
        /// This includes meta data blocks and previous version blocks
        /// </summary>
        public uint TotalBlockCount
        {
            get
            {
                if (m_isSimplified)
                    return m_dataBlockCount;
                return m_totalBlocksCount;
            }
            set
            {
                TestSimplifiedFile();
                TestForEditable();
                m_totalBlocksCount = value;
            }
        }


        /// <summary>
        /// Gets the block address for the first direct block of this file.
        /// </summary>
        public uint DirectBlock
        {
            get => m_directBlock;
            set
            {
                TestForEditable();
                m_directBlock = value;
            }
        }

        /// <summary>
        /// Gets the block address for the single indirect block.
        /// </summary>
        public uint SingleIndirectBlock
        {
            get
            {
                TestSimplifiedFile();
                return m_singleIndirectBlock;
            }
            set
            {
                TestForEditable();
                TestSimplifiedFile();
                m_singleIndirectBlock = value;
            }
        }

        /// <summary>
        /// Gets the block address for the double indirect block.
        /// </summary>
        public uint DoubleIndirectBlock
        {
            get
            {
                TestSimplifiedFile();
                return m_doubleIndirectBlock;
            }
            set
            {
                TestForEditable();
                TestSimplifiedFile();
                m_doubleIndirectBlock = value;
            }
        }

        /// <summary>
        /// Gets the block address for the tripple indirect block.
        /// </summary>
        public uint TripleIndirectBlock
        {
            get
            {
                TestSimplifiedFile();
                return m_tripleIndirectBlock;
            }
            set
            {
                TestForEditable();
                TestSimplifiedFile();
                m_tripleIndirectBlock = value;
            }
        }

        /// <summary>
        /// Gets the block address for the quadruple indirect block.
        /// </summary>
        public uint QuadrupleIndirectBlock
        {
            get
            {
                TestSimplifiedFile();
                return m_quadrupleIndirectBlock;
            }
            set
            {
                TestForEditable();
                TestSimplifiedFile();
                m_quadrupleIndirectBlock = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Writes the data contained in <see cref="SubFileHeader"/> to the data stream.
        /// </summary>
        /// <param name="dataWriter">The stream to write to.</param>
        public void Save(BinaryWriter dataWriter)
        {
            m_fileName.Save(dataWriter);
            dataWriter.Write(m_fileIdNumber);
            dataWriter.Write(m_dataBlockCount);
            if (!m_isSimplified)
            {
                dataWriter.Write(m_totalBlocksCount);
            }
            dataWriter.Write(m_directBlock);
            if (!m_isSimplified)
            {
                dataWriter.Write(m_singleIndirectBlock);
                dataWriter.Write(m_doubleIndirectBlock);
                dataWriter.Write(m_tripleIndirectBlock);
                dataWriter.Write(m_quadrupleIndirectBlock);
            }
        }


        /// <summary>
        /// Test if the class has been marked as readonly. Throws an exception if editing cannot occur.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TestSimplifiedFile()
        {
            if (m_isSimplified)
                ThrowSimplified();
        }

        private void ThrowSimplified()
        {
            throw new Exception("Value is not valid for a Simplified File Header");
        }

        #endregion
    }
}