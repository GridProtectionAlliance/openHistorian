//******************************************************************************************************
//  PartitionRolloverMode.cs - Gbtc
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
        /// Governs the rollover process.
        /// </summary>
        public class PartitionRolloverMode : IDisposable
        {
            int m_sourceGeneration;
            bool m_disposed;
            PartitionSummary m_sourcePartition;
            PartitionSummary m_destinationPartition;
            ResourceEngine m_resourceEngine;

            public PartitionRolloverMode(PartitionSummary sourcePartition, PartitionSummary destinationPartition, ResourceEngine resourceEngine, int sourceGeneration)
            {
                m_sourceGeneration = sourceGeneration;
                m_resourceEngine = resourceEngine;
                m_sourcePartition = sourcePartition;
                m_destinationPartition = destinationPartition;
            }

            /// <summary>
            /// Gets the generation of the source partition.
            /// </summary>
            public int SourceGeneration
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_sourceGeneration;
                }
            }

            /// <summary>
            /// Gets the generation of the destination partition.
            /// </summary>
            public int DestinationGeneration
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_sourceGeneration + 1;
                }
            }

            /// <summary>
            /// Gets the source partition.
            /// </summary>
            public PartitionSummary SourcePartition
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_sourcePartition;
                }
            }

            /// <summary>
            /// Gets the destination partition.
            /// </summary>
            public PartitionSummary DestinationPartition
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_destinationPartition;
                }
            }

            /// <summary>
            /// Commits and Disposes the changes made by this class.
            /// </summary>
            public void Commit()
            {
                if (m_disposed)
                    throw new Exception("Commit/Rollback has already been called");
                m_resourceEngine.CommitPartitionRolloverMode(this);
                m_disposed = true;
                m_sourcePartition = null;
                m_destinationPartition = null;
                m_resourceEngine = null;
            }

            /// <summary>
            /// Rolls back and disposes the changes made by this class.
            /// </summary>
            public void Rollback()
            {
                if (m_disposed)
                    throw new Exception("Commit/Rollback has already been called");
                m_resourceEngine.RollbackPartitionRolloverMode(this);
                m_disposed = true;
                m_sourcePartition = null;
                m_destinationPartition = null;
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
