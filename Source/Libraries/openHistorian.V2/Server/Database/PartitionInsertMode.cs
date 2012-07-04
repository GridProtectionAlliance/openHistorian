//******************************************************************************************************
//  PartitionInsertMode.cs - Gbtc
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
//  7/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.Server.Database
{
    partial class ResourceEngine
    {
        /// <summary>
        /// Governs the insertion process.
        /// </summary>
        public class PartitionInsertMode : IDisposable
        {
            int m_generation;
            bool m_disposed;
            PartitionSummary m_partition;
            ResourceEngine m_resourceEngine;

            /// <summary>
            /// Creates a <see cref="PartitionInsertMode"/> class.
            /// This class should only be constructed by <see cref="ResourceEngine"/>.
            /// </summary>
            /// <param name="partition">The partition that will be inserted into</param>
            /// <param name="resourceEngine">The caller class</param>
            /// <param name="generation">the generation of the partition</param>
            public PartitionInsertMode(PartitionSummary partition, ResourceEngine resourceEngine, int generation)
            {
                m_generation = generation;
                m_resourceEngine = resourceEngine;
                m_partition = partition;
            }

            /// <summary>
            /// Gets the partition that is being inserted into.
            /// </summary>
            public PartitionSummary Partition
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_partition;
                }
            }
            /// <summary>
            /// Gets the generation of the partition that is being inserted into.
            /// </summary>
            public int Generation
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_generation;
                }
            }
            /// <summary>
            /// Commits and Disposes the changes made by this class.
            /// </summary>
            public void Commit()
            {
                if(m_disposed)
                    throw new Exception("Commit/Rollback has already been called");
                m_resourceEngine.CommitPartitionInsertMode(this);
                m_disposed = true;
                m_partition = null;
                m_resourceEngine = null;
            }
            
            /// <summary>
            /// Rolls back and disposes the changes made by this class.
            /// </summary>
            public void Rollback()
            {
                if (m_disposed)
                    throw new Exception("Commit/Rollback has already been called");
                m_resourceEngine.RollbackPartitionInsertMode(this);
                m_disposed = true;
                m_partition = null;
                m_resourceEngine = null;
            }

            /// <summary>
            /// Disposes this class.  If the changes of this class have not been committed, they are rolled back.
            /// </summary>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    Rollback();
                }
            }
        }
    }

}
