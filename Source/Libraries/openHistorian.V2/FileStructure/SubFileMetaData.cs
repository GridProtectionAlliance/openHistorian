//******************************************************************************************************
//  SubFileMetaData.cs - Gbtc
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
//  11/23/2011 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// This contains the meta data of the file along with index information to map all of the blocks of the file.
    /// </summary>
    public class SubFileMetaData
    {
        #region [ Members ]

        /// <summary>
        /// The number of bytes that are required to save this class.
        /// </summary>
        internal const int SizeInBytes = 40;

        Guid m_fileExtension;
        bool m_isReadOnly;
        int m_fileIdNumber;
        int m_fileFlags;
        int m_directBlock;
        int m_singleIndirectBlock;
        int m_doubleIndirectBlock;
        int m_tripleIndirectBlock;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="SubFileMetaData"/> from the data stream.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="mode"></param>
        public SubFileMetaData(BinaryReader dataReader, AccessMode mode)
        {
            m_isReadOnly = (mode == AccessMode.ReadOnly);
            m_fileIdNumber = dataReader.ReadInt32();
            m_fileExtension = new Guid(dataReader.ReadBytes(16));
            m_fileFlags = dataReader.ReadInt32();
            m_directBlock = dataReader.ReadInt32();
            m_singleIndirectBlock = dataReader.ReadInt32();
            m_doubleIndirectBlock = dataReader.ReadInt32();
            m_tripleIndirectBlock = dataReader.ReadInt32();
        }

        /// <summary>
        /// Clones a <see cref="SubFileMetaData"/> allowing the caller to change the readonly flag.
        /// </summary>
        /// <param name="origionalSubFileMetaData"></param>
        /// <param name="mode"></param>
        public SubFileMetaData(SubFileMetaData origionalSubFileMetaData, AccessMode mode)
        {
            m_isReadOnly = (mode == AccessMode.ReadOnly);
            m_fileIdNumber = origionalSubFileMetaData.FileIdNumber;
            m_fileExtension = origionalSubFileMetaData.FileExtension;
            m_fileFlags = origionalSubFileMetaData.FileFlags;
            m_directBlock = origionalSubFileMetaData.DirectBlock;
            m_singleIndirectBlock = origionalSubFileMetaData.SingleIndirectBlock;
            m_doubleIndirectBlock = origionalSubFileMetaData.DoubleIndirectBlock;
            m_tripleIndirectBlock = origionalSubFileMetaData.TripleIndirectBlock;
        }

        /// <summary>
        /// Creates a new <see cref="SubFileMetaData"/>.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="featureType"></param>
        /// <param name="mode"></param>
        public SubFileMetaData(int fileId, Guid featureType, AccessMode mode)
        {
            if (featureType == Guid.Empty)
                throw new ArgumentException("The feature type cannot be an empty GUID value", "featureType");
            m_isReadOnly = (mode == AccessMode.ReadOnly);
            m_fileIdNumber = fileId;
            m_fileExtension = featureType;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets/Sets if the class is immutable
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
            set
            {
                if (m_isReadOnly && !value)
                    throw new Exception("Class is read only");
                m_isReadOnly = value;
            }
        }

        /// <summary>
        /// Gets the unique file identifier for this file.
        /// </summary>
        public int FileIdNumber
        {
            get
            {
                return m_fileIdNumber;
            }
        }

        /// <summary>
        /// Gets a <see cref="Guid"/> that represents what type of data is contained in this file.
        /// </summary>
        public Guid FileExtension
        {
            get
            {
                return m_fileExtension;
            }
        }

        /// <summary>
        /// Gets a flag which can be used to help distinguish different files with the same <see cref="FileExtension"/>.
        /// </summary>
        public int FileFlags
        {
            get
            {
                return m_fileFlags;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_fileFlags = value;
            }
        }

        /// <summary>
        /// Gets the block address for the first direct block of this file.
        /// </summary>
        public int DirectBlock
        {
            get
            {
                return m_directBlock;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_directBlock = value;
            }
        }

        /// <summary>
        /// Gets the block address for the single indirect block.
        /// </summary>
        public int SingleIndirectBlock
        {
            get
            {
                return m_singleIndirectBlock;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_singleIndirectBlock = value;
            }
        }

        /// <summary>
        /// Gets the block address for the double indirect block.
        /// </summary>
        public int DoubleIndirectBlock
        {
            get
            {
                return m_doubleIndirectBlock;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_doubleIndirectBlock = value;
            }
        }
        /// <summary>
        /// Gets the block address for the tripple indirect block.
        /// </summary>
        public int TripleIndirectBlock
        {
            get
            {
                return m_tripleIndirectBlock;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_tripleIndirectBlock = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates an editable copy of this class.
        /// </summary>
        /// <returns></returns>
        public SubFileMetaData CloneEditableCopy()
        {
            return new SubFileMetaData(this, AccessMode.ReadWrite);
        }

        /// <summary>
        /// Writes the data contained in <see cref="SubFileMetaData"/> to the data stream.
        /// </summary>
        /// <param name="dataWriter">The stream to write to.</param>
        public void Save(BinaryWriter dataWriter)
        {
            dataWriter.Write(m_fileIdNumber);
            dataWriter.Write(m_fileExtension.ToByteArray());
            dataWriter.Write(m_fileFlags);
            dataWriter.Write(m_directBlock);
            dataWriter.Write(m_singleIndirectBlock);
            dataWriter.Write(m_doubleIndirectBlock);
            dataWriter.Write(m_tripleIndirectBlock);
        }

        #endregion

    }
}
