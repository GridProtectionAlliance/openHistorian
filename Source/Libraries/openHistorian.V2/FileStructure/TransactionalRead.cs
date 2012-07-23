//******************************************************************************************************
//  TransactionalRead.cs - Gbtc
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
//  12/2/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.ObjectModel;

namespace openHistorian.V2.FileStructure
{
    /// <summary>
    /// Aquires a snapshot of the file system to browse in an isolated mannor.  
    /// This is read only and will also block the main file from being deleted. 
    /// Therefore it is important to release this lock so the file can be deleted after a rollover.
    /// </summary>
    public sealed class TransactionalRead
    {
        #region [ Members ]

        /// <summary>
        /// The readonly snapshot of the archive file.
        /// </summary>
        FileHeaderBlock m_fileHeaderBlock;

        /// <summary>
        /// The underlying diskIO to do the reads against.
        /// </summary>
        DiskIo m_dataReader;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a readonly copy of a transaction.
        /// </summary>
        /// <param name="dataReader"> </param>
        /// <param name="fileHeaderBlock">This parameter must be in a read only mode.
        ///  This is to ensure that the value is not modified after it has been passed to this class.</param>
        internal TransactionalRead(DiskIo dataReader, FileHeaderBlock fileHeaderBlock)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            if (fileHeaderBlock == null)
                throw new ArgumentNullException("fileHeaderBlock");
            if (!fileHeaderBlock.IsReadOnly)
                throw new ArgumentException("The file passed to this procedure must be read only.", "fileHeaderBlock");
            m_fileHeaderBlock = fileHeaderBlock;
            m_dataReader = dataReader;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A list of all of the files in this collection.
        /// </summary>
        public ReadOnlyCollection<SubFileMetaData> Files
        {
            get
            {
                return m_fileHeaderBlock.Files;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read the file passed to this function.
        /// </summary>
        /// <param name="fileIndex">The index of the file to open.</param>
        /// <returns></returns>
        public SubFileStream OpenFile(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= m_fileHeaderBlock.Files.Count)
                throw new ArgumentOutOfRangeException("fileIndex", "The file index provided could not be found in the header.");

            return new SubFileStream(m_dataReader, m_fileHeaderBlock.Files[fileIndex], m_fileHeaderBlock, AccessMode.ReadOnly);
        }

        /// <summary>
        /// Opens a ArchiveFileStream that can be used to read/write to the file passed to this function.
        /// </summary>
        /// <returns></returns>
        public SubFileStream OpenFile(Guid fileExtension, int fileFlags)
        {
            for (int x = 0; x < Files.Count; x++)
            {
                var file = Files[x];
                if (file.FileExtension == fileExtension && file.FileFlags == fileFlags)
                {
                    return OpenFile(x);
                }
            }
            throw new Exception("File does not exist");
        }

        #endregion
    }
}
