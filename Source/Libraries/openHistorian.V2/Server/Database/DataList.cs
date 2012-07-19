//******************************************************************************************************
//  DataList.cs - Gbtc
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
//  7/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Manages the complete list of archive resources and the 
    /// associated reading and writing that goes along with it.
    /// </summary>
    partial class DataList
    {
        object m_syncRoot = new object();

        /// <summary>
        /// Contains the list of archives that are perminent to the system and 
        /// cannot be written to.
        /// </summary>
        List<PartitionStateInformation> m_partitions;

        List<DataListSnapshot> m_resources;

        public DataList()
        {
            m_partitions = new List<PartitionStateInformation>();
            m_resources = new List<DataListSnapshot>();
        }

        #region [ Resource Locks ]

        public DataListSnapshot CreateNewClientResources()
        {
            lock (m_syncRoot)
            {
                var resources = new DataListSnapshot(ReleaseClientResources, AcquireSnapshot);
                m_resources.Add(resources);
                return resources;
            }
        }

        void ReleaseClientResources(DataListSnapshot dataLists)
        {
            lock (m_syncRoot)
            {
                m_resources.Remove(dataLists);
            }
        }

        void AcquireSnapshot(DataListSnapshot transaction)
        {
            lock (m_syncRoot)
            {
                int requiredSize = m_partitions.Count;
                if (transaction.Tables == null || transaction.Tables.Length < requiredSize)
                    transaction.Tables = new PartitionSummary[requiredSize];
                int actualSize = transaction.Tables.Length;

                for (int x = 0; x < m_partitions.Count; x++)
                {
                    transaction.Tables[x] = m_partitions[x].Summary;
                }

                if (requiredSize > actualSize)
                    Array.Clear(transaction.Tables, requiredSize, actualSize - requiredSize);
            }
        }

        #endregion

        /// <summary>
        /// Returns an <see cref="IDisposable"/> class that can be used to edit the contents of this resource.
        /// WARNING: Make changes quickly and dispose the returned class.  All calls to this class are blocked while
        /// editing this class.
        /// </summary>
        /// <returns></returns>
        public Editor AcquireEditLock()
        {
            return new Editor(this);
        }

        /// <summary>
        /// Determines if the provided partition file is currently in use
        /// by any resource. 
        /// </summary>
        /// <param name="partition">the partition to search for.</param>
        /// <returns></returns>
        public bool IsPartitionBeingUsed(PartitionFile partition)
        {
            lock (m_syncRoot)
            {
                foreach (var resource in m_resources)
                {
                    var tables = resource.Tables;
                    if (tables != null)
                    {
                        for (int x = 0; x < tables.Length; x++)
                        {
                            var summary = tables[x];
                            if (summary != null && summary.PartitionFileFile == partition)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

    }
}
