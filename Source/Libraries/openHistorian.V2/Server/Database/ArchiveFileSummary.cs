//******************************************************************************************************
//  ArchiveFileSummary.cs - Gbtc
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
//  5/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Contains an immutable class of the current partition
    /// along with its most recent snapshot.
    /// </summary>
    public class ArchiveFileSummary
    {
        #region [ Members ]

        ulong m_firstKeyValue;
        ulong m_lastKeyValue;
        ArchiveFile m_archiveFileFile;
        ArchiveFileSnapshot m_activeSnapshot;

        #endregion

        #region [ Constructors ]
       
        public ArchiveFileSummary(ArchiveFile file)
        {
            m_archiveFileFile = file;
            m_activeSnapshot = file.CreateSnapshot();
            m_firstKeyValue = file.FirstKey;
            m_lastKeyValue = file.LastKey;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="ArchiveFile"/> that this class represents.
        /// </summary>
        public ArchiveFile ArchiveFileFile
        {
            get
            {
                return m_archiveFileFile;
            }
        }

        /// <summary>
        /// Gets the first key contained in this partition.
        /// </summary>
        public ulong FirstKeyValue
        {
            get
            {
                return m_firstKeyValue;
            }
        }

        /// <summary>
        /// Gets the last key contained in this partition.
        /// </summary>
        public ulong LastKeyValue
        {
            get
            {
                return m_lastKeyValue;
            }
        }

        /// <summary>
        /// Gets the most recent <see cref="ArchiveFileSnapshot"/> of this class when it was instanced.
        /// </summary>
        public ArchiveFileSnapshot ActiveSnapshot
        {
            get
            {
                return m_activeSnapshot;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Determines if this partition might contain data for the keys provided.
        /// </summary>
        /// <param name="startKey">the first key searching for</param>
        /// <param name="stopKey">the last key searching for</param>
        /// <returns></returns>
        public bool Contains(ulong startKey, ulong stopKey)
        {
            //If the archive file is empty, it will always be searched.  
            //Since this will likely never happen and has little performance 
            //implications, I have decided not to include logic that would exclude this case.
            return !(startKey > LastKeyValue || stopKey < FirstKeyValue);
        }

        #endregion

    }
}
