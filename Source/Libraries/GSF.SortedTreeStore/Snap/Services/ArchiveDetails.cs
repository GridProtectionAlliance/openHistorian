//******************************************************************************************************
//  ArchiveDetails.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/03/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.Snap.Tree;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Gets basic archive details that can be returned to the client.
    /// </summary>
    public class ArchiveDetails
    {
        /// <summary>
        /// The ID fo the file
        /// </summary>
        public Guid Id;
        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName;
        /// <summary>
        /// Gets if the file contains anything.
        /// </summary>
        public bool IsEmpty;
        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public long FileSize;
        /// <summary>
        /// Gets the first key as a string.
        /// </summary>
        public string FirstKey;
        /// <summary>
        /// Gets the last key as a string.
        /// </summary>
        public string LastKey;

        private ArchiveDetails()
        {

        }
        /// <summary>
        /// Creates a <see cref="ArchiveDetails"/> from a specific <see cref="ArchiveTableSummary{TKey,TValue}"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static ArchiveDetails Create<TKey, TValue>(ArchiveTableSummary<TKey, TValue> table)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            return new ArchiveDetails()
                {
                    Id = table.FileId,
                    FileName = table.SortedTreeTable.BaseFile.FilePath,
                    IsEmpty = table.IsEmpty,
                    FileSize = table.SortedTreeTable.BaseFile.ArchiveSize,
                    FirstKey = table.FirstKey.ToString(),
                    LastKey = table.LastKey.ToString()
                };
        }
    }
}
