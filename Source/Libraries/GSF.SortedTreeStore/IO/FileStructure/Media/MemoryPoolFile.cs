//******************************************************************************************************
//  MemoryFile.cs - Gbtc
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
//  2/1/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    internal partial class MemoryPoolFile
        : MemoryPoolStreamCore, IDiskMediumCoreFunctions
    {
        #region [ Members ]


        /// <summary>
        /// A Reusable I/O session for all BinaryStreams
        /// </summary>
        private readonly IoSession m_ioSession;

        private bool m_isReadOnly;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Create a new <see cref="MemoryPoolFile"/>
        /// </summary>
        public MemoryPoolFile(MemoryPool pool)
            : base(pool)
        {
            m_ioSession = new IoSession(this);
            m_isReadOnly = false;
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates a <see cref="BinaryStreamIoSessionBase"/> that can be used to read from this disk medium.
        /// </summary>
        /// <returns></returns>
        public BinaryStreamIoSessionBase CreateIoSession()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("MemoryStream");
            return m_ioSession;
        }

        public string FileName => string.Empty;

        /// <summary>
        /// Executes a commit of data. This will flush the data to the disk use the provided header data to properly
        /// execute this function.
        /// </summary>
        /// <param name="headerBlock"></param>
        public void CommitChanges(FileHeaderBlock headerBlock)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("MemoryStream");
        }

        /// <summary>
        /// Rolls back all edits to the DiskMedium
        /// </summary>
        public void RollbackChanges()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("MemoryStream");
        }

        /// <summary>
        /// Changes the extension of the current file.
        /// </summary>
        /// <param name="extension">the new extension</param>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeExtension(string extension, bool isReadOnly, bool isSharingEnabled)
        {
            m_isReadOnly = isReadOnly;
        }

        /// <summary>
        /// Reopens the file with different permissions.
        /// </summary>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeShareMode(bool isReadOnly, bool isSharingEnabled)
        {
            m_isReadOnly = isReadOnly;
        }

        #endregion
    }
}