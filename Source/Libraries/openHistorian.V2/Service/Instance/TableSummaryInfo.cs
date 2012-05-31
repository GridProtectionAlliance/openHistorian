//******************************************************************************************************
//  TableSummaryInfo.cs - Gbtc
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

using System;
using System.Data;
using openHistorian.V2.Service.Instance.File;

namespace openHistorian.V2.Service.Instance
{
   
    class TableSummaryInfo
    {
        public enum MatchMode : byte
        {
            /// <summary>
            /// Set if the cache is empty.
            /// Matches No Case
            /// </summary>
            EmptyEntry = 0,
            /// <summary>
            /// Set if there are both upper and lower bounds present.
            /// Matches [LowerBound,UpperBound]
            /// </summary>
            Bounded = 3,
            /// <summary>
            /// Set if there is only a lower bound.
            /// Matches [LowerBound, infinity)
            /// </summary>
            UpperIsMissing = 1,
            /// <summary>
            /// Set if there is only an upper bound.
            /// Matches (-infinity, UpperBound]
            /// </summary>
            LowerIsMissing = 2,
            /// <summary>
            /// Matches unconditionally.
            /// </summary>
            UniverseEntry = 4,

            LowerIsValidMask = 1,
            UpperIsValidMask = 2,
        }
        
        bool m_isReadOnly;
        DateTime m_firstTime;
        DateTime m_lastTime;
        MatchMode m_timeMatchMode;
        int m_rolloverGeneration;
        Archive m_archiveFile;
        ArchiveSnapshot m_activeSnapshot;

        public TableSummaryInfo()
        {
            m_isReadOnly = false;
        }

        public TableSummaryInfo(TableSummaryInfo table)
        {
            m_firstTime = table.m_firstTime;
            m_lastTime = table.m_lastTime;
            m_timeMatchMode = table.m_timeMatchMode;
            m_rolloverGeneration = table.m_rolloverGeneration;
            m_archiveFile = table.m_archiveFile;
            m_activeSnapshot = table.m_activeSnapshot;
            m_isReadOnly = false;
        }

        public TableSummaryInfo CloneEditableCopy()
        {
           return new TableSummaryInfo(this);
        }

        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_isReadOnly = value;
            }
        }

        public Archive ArchiveFile
        {
            get
            {
                return m_archiveFile;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_archiveFile = value;
            }
        }

        public DateTime FirstTime
        {
            get
            {
                return m_firstTime;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_firstTime = value;
            }
        }

        public DateTime LastTime
        {
            get
            {
                return m_lastTime;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_lastTime = value;
            }
        }

        public MatchMode TimeMatchMode
        {
            get
            {
                return m_timeMatchMode;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_timeMatchMode = value;
            }
        }

        /// <summary>
        /// Specifies the generation of the archive file. A generation is
        /// essentially the number of time that the archive file has been rolled over. 
        /// The perminent storage generation is -1
        /// </summary>
        public int RolloverGeneration
        {
            get
            {
                return m_rolloverGeneration;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_rolloverGeneration = value;
            }
        }

        public ArchiveSnapshot ActiveSnapshot
        {
            get
            {
                return m_activeSnapshot;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_activeSnapshot = value;
            }
        }

        public bool Contains(DateTime startTime, DateTime stopTime)
        {
            //ToDo: Don't be lazy and always return true
            return true;
        }
    }
}
