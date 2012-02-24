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

namespace Historian.StorageSystem.File
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
        bool m_IsReadOnly;
        uint m_FileIDNumber;
        Guid m_FileExtension;
        uint m_FileFlags;
        uint m_LastAllocatedCluster;
        uint m_BlocksPerCluster;
        uint m_DirectCluster;
        uint m_SingleIndirectCluster;
        uint m_DoubleIndirectCluster;
        uint m_TripleIndirectCluster;
        uint m_QuadrupleIndirectCluster;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an immutable class from the data stream.
        /// </summary>
        /// <param name="dataReader"></param>
        FileMetaData(BinaryReader dataReader)
        {
            m_IsReadOnly = true;
            Load(dataReader);
        }

        /// <summary>
        /// Creates an editable version of the archive header.
        /// </summary>
        /// <param name="Orig"></param>
        FileMetaData(FileMetaData Orig)
        {
            m_IsReadOnly = false;
            m_FileIDNumber = Orig.FileIDNumber;
            m_FileExtension = Orig.FileExtension;
            m_FileFlags = Orig.FileFlags;
            m_LastAllocatedCluster = Orig.LastAllocatedCluster;
            m_BlocksPerCluster = Orig.BlocksPerCluster;
            m_DirectCluster = Orig.DirectCluster;
            m_SingleIndirectCluster = Orig.SingleIndirectCluster;
            m_DoubleIndirectCluster = Orig.DoubleIndirectCluster;
            m_TripleIndirectCluster = Orig.TripleIndirectCluster;
            m_QuadrupleIndirectCluster = Orig.QuadrupleIndirectCluster;
        }

        /// <summary>
        /// Creates a new inode that can be edited
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="featureType"></param>
        /// <param name="pagesPerBlock"></param>
        FileMetaData(uint fileID, Guid featureType, uint pagesPerBlock)
        {
            m_IsReadOnly = false;
            if (featureType == Guid.Empty)
                throw new ArgumentException("featureType", "The feature type cannot be an empty GUID value");
            if (pagesPerBlock == 0)
                throw new ArgumentException("pagesPerBlock", "Value cannot be zero");
            m_FileIDNumber = fileID;
            m_FileExtension = featureType;
            m_BlocksPerCluster = pagesPerBlock;
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
                return m_IsReadOnly;
            }
        }

        /// <summary>
        /// Gets the unique file identifier for this file.
        /// </summary>
        public uint FileIDNumber
        {
            get
            {
                return m_FileIDNumber;
            }
        }

        /// <summary>
        /// Gets a <see cref="Guid"/> that represents what type of data is contained in this file.
        /// </summary>
        public Guid FileExtension
        {
            get
            {
                return m_FileExtension;
            }
        }

        /// <summary>
        /// Gets a flag which can be used to help distinguish different files with the same <see cref="FileExtension"/>.
        /// </summary>
        public uint FileFlags
        {
            get
            {
                return m_FileFlags;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_FileFlags = value;
            }
        }

        /// <summary>
        /// The last cluster index value that is assigned.
        /// </summary>
        internal uint LastAllocatedCluster
        {
            get
            {
                return m_LastAllocatedCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_LastAllocatedCluster = value;
            }
        }

        /// <summary>
        /// The number of blocks that make up a cluster.
        /// </summary>
        public uint BlocksPerCluster
        {
            get
            {
                return m_BlocksPerCluster;
            }
        }

        /// <summary>
        /// Gets the block address for the first direct cluster of this file.
        /// </summary>
        internal uint DirectCluster
        {
            get
            {
                return m_DirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_DirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the single indirect cluster.
        /// </summary>
        internal uint SingleIndirectCluster
        {
            get
            {
                return m_SingleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_SingleIndirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the double indirect cluster.
        /// </summary>
        internal uint DoubleIndirectCluster
        {
            get
            {
                return m_DoubleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_DoubleIndirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the tripple indirect cluster.
        /// </summary>
        internal uint TripleIndirectCluster
        {
            get
            {
                return m_TripleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_TripleIndirectCluster = value;
            }
        }
        /// <summary>
        /// Gets the block address for the quadruple indirect cluster.
        /// </summary>
        internal uint QuadrupleIndirectCluster
        {
            get
            {
                return m_QuadrupleIndirectCluster;
            }
            set
            {
                if (IsReadOnly)
                    throw new Exception("Class is read only");
                m_QuadrupleIndirectCluster = value;
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
            dataWriter.Write(m_FileIDNumber);
            dataWriter.Write(m_FileExtension.ToByteArray());
            dataWriter.Write(m_FileFlags);
            dataWriter.Write(m_LastAllocatedCluster);
            dataWriter.Write(m_BlocksPerCluster);
            dataWriter.Write(m_DirectCluster);
            dataWriter.Write(m_SingleIndirectCluster);
            dataWriter.Write(m_DoubleIndirectCluster);
            dataWriter.Write(m_TripleIndirectCluster);
            dataWriter.Write(m_QuadrupleIndirectCluster);
        }

        /// <summary>
        /// Loads this class with data form the binary stream.
        /// </summary>
        /// <param name="dataReader"></param>
        void Load(BinaryReader dataReader)
        {
            m_FileIDNumber = dataReader.ReadUInt32();
            m_FileExtension = new Guid(dataReader.ReadBytes(16));
            m_FileFlags = dataReader.ReadUInt32();
            m_LastAllocatedCluster = dataReader.ReadUInt32();
            m_BlocksPerCluster = dataReader.ReadUInt32();
            m_DirectCluster = dataReader.ReadUInt32();
            m_SingleIndirectCluster = dataReader.ReadUInt32();
            m_DoubleIndirectCluster = dataReader.ReadUInt32();
            m_TripleIndirectCluster = dataReader.ReadUInt32();
            m_QuadrupleIndirectCluster = dataReader.ReadUInt32();
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

            if (FileIDNumber != a.FileIDNumber) return false;
            if (FileExtension != a.FileExtension) return false;
            if (FileFlags != a.FileFlags) return false;
            if (LastAllocatedCluster != a.LastAllocatedCluster) return false;
            if (BlocksPerCluster != a.BlocksPerCluster) return false;
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
        /// <param name="fileIDNumber">A unique file ID number in the file system</param>
        /// <param name="fileExtension">A <see cref="Guid"/> that represents what type of data is contained in this file.</param>
        /// <param name="blocksPerCluster">The number of blocks that are contained in each file cluster.</param>
        public static FileMetaData CreateFileMetaData(uint fileIDNumber, Guid fileExtension, uint blocksPerCluster)
        {
            FileMetaData file =  new FileMetaData(fileIDNumber, fileExtension, blocksPerCluster);
            if (file.IsReadOnly)
                throw new Exception();
            return file;
        }

        #endregion

    }
}
