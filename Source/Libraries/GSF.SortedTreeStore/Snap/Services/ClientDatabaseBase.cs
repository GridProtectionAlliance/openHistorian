//******************************************************************************************************
//  SortedTreeClientBase.cs - Gbtc
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
//  12/08/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Represents a single historian database.
    /// </summary>
    public abstract class ClientDatabaseBase : IDisposable
    {
        /// <summary>
        /// Loads the provided files from all of the specified paths.
        /// </summary>
        /// <param name="paths">all of the paths of archive files to attach. These can either be a path, or an individual file name.</param>
        public abstract void AttachFilesOrPaths(IEnumerable<string> paths);

        /// <summary>
        /// Enumerates all of the files attached to the database.
        /// </summary>
        /// <returns></returns>
        public abstract List<ArchiveDetails> GetAllAttachedFiles();

        /// <summary>
        /// Detaches the list of files from the database.
        /// </summary>
        /// <param name="files">the file ids that need to be detatched.</param>
        public abstract void DetatchFiles(List<Guid> files);

        /// <summary>
        /// Deletes the list of files from the database.
        /// </summary>
        /// <param name="files">the files that need to be deleted</param>
        public abstract void DeleteFiles(List<Guid> files);

        /// <summary>
        /// Gets if has been disposed.
        /// </summary>
        public abstract bool IsDisposed { get; }

        /// <summary>
        /// Gets basic information about the current Database.
        /// </summary>
        public abstract DatabaseInfo Info { get; }

        /// <summary>
        /// Forces a soft commit on the database. A soft commit 
        /// only commits data to memory. This allows other clients to read the data.
        /// While soft committed, this data could be lost during an unexpected shutdown.
        /// Soft commits usually occur within microseconds. 
        /// </summary>
        public abstract void SoftCommit();

        /// <summary>
        /// Forces a commit to the disk subsystem. Once this returns, the data will not
        /// be lost due to an application crash or unexpected shutdown.
        /// Hard commits can take 100ms or longer depending on how much data has to be committed. 
        /// This requires two consecutive hardware cache flushes.
        /// </summary>
        public abstract void HardCommit();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}