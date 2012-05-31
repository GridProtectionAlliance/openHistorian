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

namespace openHistorian.V2.Service.Instance
{
    /// <summary>
    /// Provides an imutable object that acts as a snapshot isolation layer.
    /// To modify this object, call the <see cref="CloneEditableCopy"/> method.
    /// </summary>
    class LookupTable
    {
        List<TableSummaryInfo> m_generations;
        List<TableSummaryInfo> m_tables;
        bool m_isReadOnly;

        public LookupTable()
        {
            m_generations = new List<TableSummaryInfo>();
            m_tables = new List<TableSummaryInfo>();
            m_isReadOnly = false;
        }
        public LookupTable(LookupTable clone)
        {
            m_isReadOnly = false;
            m_generations = new List<TableSummaryInfo>(clone.m_generations);
            m_tables = new List<TableSummaryInfo>(clone.m_tables);
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

        public TableSummaryInfo GetGeneration(int index)
        {
            return m_generations[index];
        }

        public TableSummaryInfo GetTables(int index)
        {
            return m_tables[index];
        }

        public void SetGeneration(int index, TableSummaryInfo table)
        {
            if (m_isReadOnly) throw new ReadOnlyException();
            m_generations[index] = table;
        }

        public void SetTable(int index, TableSummaryInfo table)
        {
            if (m_isReadOnly) throw new ReadOnlyException();
            m_tables[index] = table;
        }

        public void AddTable(TableSummaryInfo table)
        {
            if (m_isReadOnly) throw new ReadOnlyException();
            m_tables.Add(table);
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
