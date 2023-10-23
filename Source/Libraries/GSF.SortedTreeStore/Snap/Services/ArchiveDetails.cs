//******************************************************************************************************
//  ArchiveDetails.cs - Gbtc
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
//  10/03/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Linq;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Gets basic archive details that can be returned to the client.
    /// </summary>
    public class ArchiveDetails
    {
        /// <summary>
        /// The ID for the file
        /// </summary>
        public Guid Id
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets if the file contains anything.
        /// </summary>
        public bool IsEmpty
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public long FileSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the first key as a string.
        /// </summary>
        public string FirstKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last key as a string.
        /// </summary>
        public string LastKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the start time of the archive, if applicable to Key type.
        /// </summary>
        /// <remarks>
        /// If Key type does not expose a TimestampAsDate property, value will be <see cref="DateTime.MinValue"/>.
        /// </remarks>
        public DateTime StartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the end time of the archive, if applicable to Key type.
        /// </summary>
        /// <remarks>
        /// If Key type does not expose a TimestampAsDate property, value will be <see cref="DateTime.MaxValue"/>.
        /// </remarks>
        public DateTime EndTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the flags for the archive file.
        /// </summary>
        public Guid[] Flags
        {
            get;
            private set;
        }

        private ArchiveDetails()
        {
        }

        /// <summary>
        /// Creates a <see cref="ArchiveDetails"/> from a specific <see cref="ArchiveTableSummary{TKey,TValue}"/>
        /// </summary>
        public static ArchiveDetails Create<TKey, TValue>(ArchiveTableSummary<TKey, TValue> table)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            ArchiveDetails details = new ArchiveDetails
            {
                Id = table.FileId,
                FileName = table.SortedTreeTable.BaseFile.FilePath,
                IsEmpty = table.IsEmpty,
                FileSize = table.SortedTreeTable.BaseFile.ArchiveSize,
                FirstKey = table.FirstKey.ToString(),
                LastKey = table.LastKey.ToString(),
                Flags = table.SortedTreeTable.BaseFile.Snapshot.Header.Flags.ToArray()
            };

#if SQLCLR
            details.StartTime = DateTime.MinValue;
            details.EndTime = DateTime.MaxValue;
#else
            try
            {
                // Attempt to get timestamp range for archive file
                dynamic firstKey = table.FirstKey;
                dynamic lastKey = table.LastKey;
                details.StartTime = firstKey.TimestampAsDate;
                details.EndTime = lastKey.TimestampAsDate;
            }
            catch
            {
                // TKey implementation does not contain a TimestampAsDate property
                details.StartTime = DateTime.MinValue;
                details.EndTime = DateTime.MaxValue;
            }
#endif
            return details;
        }
    }
}
