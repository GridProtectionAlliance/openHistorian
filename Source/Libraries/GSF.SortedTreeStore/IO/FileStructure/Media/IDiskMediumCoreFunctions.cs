//******************************************************************************************************
//  IDiskMediumCoreFunctions.cs - Gbtc
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
//  2/22/2013 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    /// <summary>
    /// The interface that is required construct a <see cref="DiskMedium"/> class.
    /// </summary>
    internal interface IDiskMediumCoreFunctions 
        : IDisposable
    {
        /// <summary>
        /// Creates a <see cref="BinaryStreamIoSessionBase"/> that can be used to read from this disk medium.
        /// </summary>
        /// <returns></returns>
        BinaryStreamIoSessionBase CreateIoSession();

        /// <summary>
        /// Gets the current number of bytes used by the file system. 
        /// This is only intended to be an approximate figure. 
        /// </summary>
        long Length
        {
            get;
        }

        /// <summary>
        /// Gets the file name associated with the medium. Returns an empty string if a memory file.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Executes a commit of data. This will flush the data to the disk use the provided header data to properly
        /// execute this function.
        /// </summary>
        /// <param name="header"></param>
        void CommitChanges(FileHeaderBlock header);

        /// <summary>
        /// Rolls back all edits to the DiskMedium
        /// </summary>
        void RollbackChanges();

        /// <summary>
        /// Changes the extension of the current file.
        /// </summary>
        /// <param name="extension">the new extension</param>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        void ChangeExtension(string extension, bool isReadOnly, bool isSharingEnabled);

        /// <summary>
        /// Reopens the file with different permissions.
        /// </summary>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        void ChangeShareMode(bool isReadOnly, bool isSharingEnabled);

    }
}