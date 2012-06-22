//******************************************************************************************************
//  FileMetaData.cs - Gbtc
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

namespace openHistorian.V2.FileSystem
{
    /// <summary>
    /// This contains the meta data of the file along with index information to map all of the blocks of the file.
    /// </summary>
    public class FileMetaData
    {
        #region [ Members ]
        /// <summary>
        /// The number of bytes that are required to save this class.
        /// </summary>
        internal const int SizeInBytes = 40;
        bool m_isReadOnly;
        
        int m_fileIdNumber;
        Guid m_fileExtension;
        int m_fileFlags;
        int m_directBlock;
        int m_singleIndirectBlock;
        int m_doubleIndirectBlock;
        int m_tripleIndirectBlock;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an immutable class from the data stream.
        /// </summary>
        /// <param name="dataReader"></param>
        FileMetaData(BinaryReader dataReader)
        {
            m_isReadOnly = true;
            Load(dataReader);
        }

        /// <summary>
        /// Creates an editable version of the archive header.
        /// </summary>
        /// <param name="origionalFileMetaData"></param>
        FileMetaData(FileMetaData origionalFileMetaData)
        {
            m_isReadOnly = false;
            m_fileIdNumber = origionalFileMetaData.FileIdNumber;
            m_fileExtension = origionalFileMetaData.FileExtension;
            m_fileFlags = origionalFileMetaData.FileFlags;
            m_directBlock = origionalFileMetaData.DirectBlock;
            m_singleIndirectBlock = origionalFileMetaData.SingleIndirectBlock;
            m_doubleIndirectBlock = origionalFileMetaData.DoubleIndirectBlock;
            m_tripleIndirectBlock = origionalFileMetaData.TripleIndirectBlock;
        }

        /// <summary>
        /// Creates a new inode that can be edited
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="featureType"></param>
        FileMetaData(int fileId, Guid featureType)
        {
            m_isReadOnly = false;
            if (featureType == Guid.Empty)
                throw new ArgumentException("The feature type cannot be an empty GUID value", "featureType");
            m_fileIdNumber = fileId;
            m_fileExtension = featureType;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the class is immutable
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
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
        /// Gets the block address for the first direct cluster of this file.
        /// </summary>
        internal int DirectBlock
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
        /// Gets the block address for the single indirect cluster.
        /// </summary>
        internal int SingleIndirectBlock
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
        /// Gets the block address for the double indirect cluster.
        /// </summary>
        internal int DoubleIndirectBlock
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
        /// Gets the block address for the tripple indirect cluster.
        /// </summary>
        internal int TripleIndirectBlock
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
        internal FileMetaData CreateEditableCopy()
        {
            return new FileMetaData(this);
        }

        /// <summary>
        /// Writes the FileMetaData to the data stream.
        /// </summary>
        /// <param name="dataWriter"></param>
        internal void Save(BinaryWriter dataWriter)
        {
            dataWriter.Write(m_fileIdNumber);
            dataWriter.Write(m_fileExtension.ToByteArray());
            dataWriter.Write(m_fileFlags);
            dataWriter.Write(m_directBlock);
            dataWriter.Write(m_singleIndirectBlock);
            dataWriter.Write(m_doubleIndirectBlock);
            dataWriter.Write(m_tripleIndirectBlock);
        }

        /// <summary>
        /// Loads this class with data form the binary stream.
        /// </summary>
        /// <param name="dataReader"></param>
        void Load(BinaryReader dataReader)
        {
            m_fileIdNumber = dataReader.ReadInt32();
            m_fileExtension = new Guid(dataReader.ReadBytes(16));
            m_fileFlags = dataReader.ReadInt32();
            m_directBlock = dataReader.ReadInt32();
            m_singleIndirectBlock = dataReader.ReadInt32();
            m_doubleIndirectBlock = dataReader.ReadInt32();
            m_tripleIndirectBlock = dataReader.ReadInt32();
        }
        /// <summary>
        /// Determines if the two objects are equal in value.
        /// </summary>
        /// <param name="a">the object to compare this class to</param>
        /// <returns></returns>
        public bool AreEqual(FileMetaData a)
        {
            if (a == null)
                return false;

            if (FileIdNumber != a.FileIdNumber) return false;
            if (FileExtension != a.FileExtension) return false;
            if (FileFlags != a.FileFlags) return false;
            if (DirectBlock != a.DirectBlock) return false;
            if (SingleIndirectBlock != a.SingleIndirectBlock) return false;
            if (DoubleIndirectBlock != a.DoubleIndirectBlock) return false;
            if (TripleIndirectBlock != a.TripleIndirectBlock) return false;
            return true;
        }

        #endregion

        #region [ Static ]

        /// <summary>
        /// Creates an immutable class from the data stream.
        /// </summary>
        /// <param name="dataReader"></param>
        public static FileMetaData OpenFileMetaData(BinaryReader dataReader)
        {
            FileMetaData file = new FileMetaData(dataReader);
            if (!file.IsReadOnly)
                throw new Exception();
            return file;
        }

        /// <summary>
        /// Creates a new file that can be edited
        /// </summary>
        /// <param name="fileIdNumber">A unique file ID number in the file system</param>
        /// <param name="fileExtension">A <see cref="Guid"/> that represents what type of data is contained in this file.</param>
        public static FileMetaData CreateFileMetaData(int fileIdNumber, Guid fileExtension)
        {
            FileMetaData file = new FileMetaData(fileIdNumber, fileExtension);
            if (file.IsReadOnly)
                throw new Exception();
            return file;
        }

        #endregion

    }
}
