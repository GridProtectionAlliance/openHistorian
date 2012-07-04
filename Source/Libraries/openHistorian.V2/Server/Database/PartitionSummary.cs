//******************************************************************************************************
//  PartitionSummary.cs - Gbtc
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
using System.Threading;
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Contains an immutable class of the current partition
    /// along with its most recent snapshot.
    /// </summary>
    class PartitionSummary
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
        object m_editLock = new object();
        bool m_isReadOnly;
        ulong m_firstKeyValue;
        ulong m_lastKeyValue;
        MatchMode m_keyMatchMode;
        int m_rolloverGeneration;
        PartitionFile m_partitionFileFile;
        PartitionSnapshot m_activeSnapshot;

        /// <summary>
        /// Creates an editable <see cref="PartitionSummary"/> 
        /// until it <see cref="IsReadOnly"/> has been set to true.
        /// </summary>
        public PartitionSummary()
        {
            m_isReadOnly = false;
        }

        /// <summary>
        /// Clones an existing <see cref="PartitionSummary"/> class and makes it editable
        /// until <see cref="IsReadOnly"/> is set to true.
        /// </summary>
        /// <param name="partition">the <see cref="PartitionSummary"/> to clone and make editable.</param>
        public PartitionSummary(PartitionSummary partition)
        {
            m_firstKeyValue = partition.m_firstKeyValue;
            m_lastKeyValue = partition.m_lastKeyValue;
            m_keyMatchMode = partition.m_keyMatchMode;
            m_rolloverGeneration = partition.m_rolloverGeneration;
            m_partitionFileFile = partition.m_partitionFileFile;
            m_activeSnapshot = partition.m_activeSnapshot;
            m_isReadOnly = false;
        }

        /// <summary>
        /// Clones this class and creates on that can be edited.
        /// </summary>
        /// <returns></returns>
        public PartitionSummary CloneEditableCopy()
        {
            return new PartitionSummary(this);
        }

        /// <summary>
        /// Gets/Sets if this class is in an immutable state.
        /// The state of the variable can only be changed from False to True.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
            set
            {
                if (m_isReadOnly && !value)
                    throw new ReadOnlyException("Object is read only");
                m_isReadOnly = value;
            }
        }

        /// <summary>
        /// When synchronizing edits with this partition, lock on this object.
        /// </summary>
        public object EditLockObject
        {
            get
            {
                return m_editLock;
            }
        }

        /// <summary>
        /// Gets/Sets the <see cref="PartitionFile"/> that this class represents.
        /// Can only set if <see cref="IsReadOnly"/> is false.
        /// </summary>
        public PartitionFile PartitionFileFile
        {
            get
            {
                return m_partitionFileFile;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_partitionFileFile = value;
            }
        }

        /// <summary>
        /// Gets/Sets the first key contained in this partition.
        /// </summary>
        public ulong FirstKeyValue
        {
            get
            {
                return m_firstKeyValue;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_firstKeyValue = value;
            }
        }

        /// <summary>
        /// Gets/Sets the last key contained in this partition.
        /// </summary>
        public ulong LastKeyValue
        {
            get
            {
                return m_lastKeyValue;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_lastKeyValue = value;
            }
        }

        /// <summary>
        /// Gets/Sets a <see cref="MatchMode"/> for matching the key.
        /// </summary>
        public MatchMode KeyMatchMode
        {
            get
            {
                return m_keyMatchMode;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_keyMatchMode = value;
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

        /// <summary>
        /// Gets/Sets the most recent <see cref="PartitionSnapshot"/> of this class when it was instanced.
        /// </summary>
        public PartitionSnapshot ActiveSnapshot
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

        /// <summary>
        /// Determines if this partition might contain data for the keys provided.
        /// </summary>
        /// <param name="startKey">the first key searching for</param>
        /// <param name="stopKey">the last key searching for</param>
        /// <returns></returns>
        public bool Contains(ulong startKey, ulong stopKey)
        {
            //ToDo: Don't be lazy by always returning true
            return true;
        }

        public void WaitForEditLockRelease()
        {
            lock(EditLockObject)
            {
            }
        }

    

    }
}
