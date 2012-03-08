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

namespace openHistorian.Core.StorageSystem.File
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
        internal const int SizeInBytes = 52;
        bool m_isReadOnly;
        uint m_fileIdNumber;
        Guid m_fileExtension;
        uint m_fileFlags;
        uint m_lastAllocatedCluster;
        uint m_directCluster;
        uint m_singleIndirectCluster;
        uint m_doubleIndirectCluster;
        uint m_tripleIndirectCluster;
        uint m_quadrupleIndirectCluster;

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
            m_lastAllocatedCluster = origionalFileMetaData.LastAllocatedCluster;
            m_directCluster = origionalFileMetaData.DirectCluster;
            m_singleIndirectCluster = origionalFileMetaData.SingleIndirectCluster;
            m_doubleIndirectCluster = origionalFileMetaData.DoubleIndirectCluster;
            m_tripleIndirectCluster = origionalFileMetaData.TripleIndirectCluster;
            m_quadrupleIndirectCluster = origionalFileMetaData.QuadrupleIndirectCluster;
        }

        /// <summary>
        /// Creates a new inode that can be edited
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="featureType"></param>
        FileMetaData(uint fileId, Guid featureType)
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
        public uint FileIdNumber
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
        public uint FileFlags
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
        /// The last cluster index value that is assigned.
        /// </summary>
        internal uint LastAllocatedCluster
        {
            get
            {
                return m_lastAllocatedCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_lastAllocatedCluster = value;
            }
        }

        /// <summary>
        /// Gets the block address for the first direct cluster of this file.
        /// </summary>
        internal uint DirectCluster
        {
            get
            {
                return m_directCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_directCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the single indirect cluster.
        /// </summary>
        internal uint SingleIndirectCluster
        {
            get
            {
                return m_singleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_singleIndirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the double indirect cluster.
        /// </summary>
        internal uint DoubleIndirectCluster
        {
            get
            {
                return m_doubleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_doubleIndirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the tripple indirect cluster.
        /// </summary>
        internal uint TripleIndirectCluster
        {
            get
            {
                return m_tripleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_tripleIndirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the quadruple indirect cluster.
        /// </summary>
        internal uint QuadrupleIndirectCluster
        {
            get
            {
                return m_quadrupleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_quadrupleIndirectCluster = value;
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
            dataWriter.Write(m_lastAllocatedCluster);
            dataWriter.Write(m_directCluster);
            dataWriter.Write(m_singleIndirectCluster);
            dataWriter.Write(m_doubleIndirectCluster);
            dataWriter.Write(m_tripleIndirectCluster);
            dataWriter.Write(m_quadrupleIndirectCluster);
        }

        /// <summary>
        /// Loads this class with data form the binary stream.
        /// </summary>
        /// <param name="dataReader"></param>
        void Load(BinaryReader dataReader)
        {
            m_fileIdNumber = dataReader.ReadUInt32();
            m_fileExtension = new Guid(dataReader.ReadBytes(16));
            m_fileFlags = dataReader.ReadUInt32();
            m_lastAllocatedCluster = dataReader.ReadUInt32();
            m_directCluster = dataReader.ReadUInt32();
            m_singleIndirectCluster = dataReader.ReadUInt32();
            m_doubleIndirectCluster = dataReader.ReadUInt32();
            m_tripleIndirectCluster = dataReader.ReadUInt32();
            m_quadrupleIndirectCluster = dataReader.ReadUInt32();
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
            if (LastAllocatedCluster != a.LastAllocatedCluster) return false;
            if (DirectCluster != a.DirectCluster) return false;
            if (SingleIndirectCluster != a.SingleIndirectCluster) return false;
            if (DoubleIndirectCluster != a.DoubleIndirectCluster) return false;
            if (TripleIndirectCluster != a.TripleIndirectCluster) return false;
            if (QuadrupleIndirectCluster != a.QuadrupleIndirectCluster) return false;
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
        public static FileMetaData CreateFileMetaData(uint fileIdNumber, Guid fileExtension)
        {
            FileMetaData file = new FileMetaData(fileIdNumber, fileExtension);
            if (file.IsReadOnly)
                throw new Exception();
            return file;
        }

        #endregion

    }
}
