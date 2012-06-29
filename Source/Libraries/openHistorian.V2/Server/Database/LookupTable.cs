//******************************************************************************************************
//  LookupTable.cs - Gbtc
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
using System.Collections.Generic;
using System.Data;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Provides an imutable object that acts as a snapshot isolation layer.
    /// To modify this object, call the <see cref="CloneEditableCopy"/> method.
    /// </summary>
    class LookupTable
    {
        List<PartitionSummary> m_generations;
        List<PartitionSummary> m_tables;
        bool m_isReadOnly;

        public LookupTable()
        {
            m_generations = new List<PartitionSummary>();
            m_tables = new List<PartitionSummary>();
            m_isReadOnly = false;
        }
        public LookupTable(LookupTable clone)
        {
            m_isReadOnly = false;
            m_generations = new List<PartitionSummary>(clone.m_generations);
            m_tables = new List<PartitionSummary>(clone.m_tables);
        }

        /// <summary>
        /// Gets if the object can be modified.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
        }

        public PartitionSummary GetGeneration(int index)
        {
            return m_generations[index];
        }

        public PartitionSummary GetTables(int index)
        {
            return m_tables[index];
        }

        public void SetGeneration(int index, PartitionSummary partition)
        {
            if (m_isReadOnly) throw new ReadOnlyException();
            m_generations[index] = partition;
        }

        public void SetTable(int index, PartitionSummary partition)
        {
            if (m_isReadOnly) throw new ReadOnlyException();
            m_tables[index] = partition;
        }

        public void AddTable(PartitionSummary partition)
        {
            if (m_isReadOnly) throw new ReadOnlyException();
            m_tables.Add(partition);
        }

        public void SetReadOnly()
        {
            m_isReadOnly = true;
        }

        public LookupTable CloneEditableCopy()
        {
            return new LookupTable(this);
        }


    }
}
