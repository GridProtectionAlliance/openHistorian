//******************************************************************************************************
//  ResourceEngine.cs - Gbtc
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
//  5/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Manages the complete list of archive resources and the 
    /// associated reading and writing that goes along with it.
    /// </summary>
    partial class ResourceEngine
    {
        public event Action<int, long> CommitComplete;
        public const int GenerationCount = 3;
        object m_syncRoot = new object();
        PartitionSummary[] m_activePartitions;
        PartitionSummary[] m_processingPartitions;
        PartitionInitializer m_partitionInitializer;
        object[] m_editLocks = new object[GenerationCount];

        /// <summary>
        /// Contais the archives that have successfully been rolled over and are pending
        /// deletion from the system.
        /// </summary>
        List<PartitionSummary> m_partitionsPendingDeletions;

        /// <summary>
        /// Contains the list of archives that are perminent to the system and 
        /// cannot be written to.
        /// </summary>
        List<PartitionSummary> m_finalPartitions;

        List<TransactionResources> m_readLockedResourceses;

        public ResourceEngine()
        {
            for (int x = 0; x < GenerationCount; x++)
            {
                m_editLocks[x] = new object();
            }
            m_partitionInitializer = new PartitionInitializer();
            m_activePartitions = new PartitionSummary[GenerationCount];
            m_processingPartitions = new PartitionSummary[GenerationCount];
            m_partitionsPendingDeletions = new List<PartitionSummary>();
            m_finalPartitions = new List<PartitionSummary>();
            m_readLockedResourceses = new List<TransactionResources>();
        }

        public TransactionResources CreateNewClientResources()
        {
            lock (m_syncRoot)
            {
                var resources = new TransactionResources();
                m_readLockedResourceses.Add(resources);
                return resources;
            }
        }

        public void ReleaseClientResources(TransactionResources resources)
        {
            lock (m_syncRoot)
            {
                m_readLockedResourceses.Remove(resources);
            }
        }

        public void AquireSnapshot(TransactionResources transaction)
        {
            lock (m_syncRoot)
            {
                int count = m_finalPartitions.Count + 6;
                PartitionSummary[] partitions = new PartitionSummary[count];

                m_activePartitions.CopyTo(partitions, 0);
                m_processingPartitions.CopyTo(partitions, GenerationCount);
                m_finalPartitions.CopyTo(GenerationCount * 2, partitions, 0, m_finalPartitions.Count);
            }
        }

        #region [ Partition Insert Mode ]

        /// <summary>
        /// Returns the current active partition for the provided <see cref="generation"/>.
        /// </summary>
        /// <param name="generation">the generation number</param>
        /// <returns></returns>
        /// <remarks>
        /// Since only one thread will be locking the active partition, this function should return very quickly.
        /// A partition will always be returned from this class.  If it is not available, it will be created.
        /// </remarks>
        public PartitionInsertMode StartPartitionInsertMode(int generation)
        {
            if (generation < 0 || (generation >= GenerationCount))
                throw new ArgumentOutOfRangeException("generation");
            lock (m_syncRoot)
            {
                //Since only 1 thread is responsible for inserts, this lock will always be uncontested.
                PartitionSummary activePartition = GetAndLockActivePartition(generation);
                return new PartitionInsertMode(activePartition, this, generation);
            }
        }

        void CommitPartitionInsertMode(PartitionInsertMode insertMode)
        {
            PartitionSummary newPartition = new PartitionSummary(insertMode.Partition.PartitionFileFile);

            lock (m_syncRoot)
            {
                //Since the commit can happen after a rollover is pending, determine where the initial 
                //partition is to assign the new one.
                if (m_activePartitions[insertMode.Generation] == insertMode.Partition)
                {
                    m_activePartitions[insertMode.Generation] = newPartition;
                }
                else if (m_processingPartitions[insertMode.Generation] == insertMode.Partition)
                {
                    m_processingPartitions[insertMode.Generation] = newPartition;
                }
                else
                {
                    throw new Exception();
                }
                Monitor.Exit(m_editLocks[insertMode.Generation]);
            }
            CommitComplete(insertMode.Generation, 1);
        }

        void RollbackPartitionInsertMode(PartitionInsertMode insertMode)
        {
            Monitor.Exit(m_editLocks[insertMode.Generation]);
        }

        #endregion

        #region [ Partition Rollover Mode ]

        public PartitionRolloverMode StartPartitionRolloverMode(int generation)
        {
            if (generation < 0 || (generation > GenerationCount))
                throw new ArgumentOutOfRangeException("generation");
            PartitionSummary processingPartition;
            lock (m_syncRoot)
            {
                processingPartition = m_activePartitions[generation];
                m_processingPartitions[generation] = processingPartition;
                m_activePartitions[generation] = null;
            }
            lock (m_editLocks[generation])//Wait for lock release
            {
            }
            PartitionSummary activePartition = GetAndLockActivePartition(generation + 1);

            return new PartitionRolloverMode(processingPartition, activePartition, this, generation);
        }

        void CommitPartitionRolloverMode(PartitionRolloverMode rolloverMode)
        {
            PartitionSummary newPartition = new PartitionSummary(rolloverMode.DestinationPartition.PartitionFileFile);

            lock (m_syncRoot)
            {
                //Since the commit can happen after a rollover is pending, determine where the initial 
                //partition is to assign the new one.
                if (m_activePartitions[rolloverMode.DestinationGeneration] == rolloverMode.DestinationPartition)
                {
                    m_activePartitions[rolloverMode.DestinationGeneration] = newPartition;
                }
                else if (m_processingPartitions[rolloverMode.DestinationGeneration] == rolloverMode.DestinationPartition)
                {
                    m_processingPartitions[rolloverMode.DestinationGeneration] = newPartition;
                }
                else
                {
                    throw new Exception();
                }
                Monitor.Exit(m_editLocks[rolloverMode.DestinationGeneration]);

                m_partitionsPendingDeletions.Add(rolloverMode.SourcePartition);
                m_processingPartitions[rolloverMode.SourceGeneration] = null;
            }

            CommitComplete(rolloverMode.DestinationGeneration, 1);
        }

        void RollbackPartitionRolloverMode(PartitionRolloverMode rolloverMode)
        {
            Monitor.Exit(m_editLocks[rolloverMode.DestinationGeneration]);
        }

        #endregion


        /// <summary>
        /// Returns the active partition for the given generation. This object will also be edit locked.
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        /// <remarks>If the current active partition does not exist, one will be created.</remarks>
        PartitionSummary GetAndLockActivePartition(int generation)
        {
            PartitionSummary activePartition;
            lock (m_syncRoot)
            {
                activePartition = m_activePartitions[generation];
                if (activePartition != null)
                {
                    Monitor.Enter(m_editLocks[generation]);
                    return activePartition;
                }
            }
            activePartition = m_partitionInitializer.CreatePartition(generation);
            Monitor.Enter(m_editLocks[generation]);
            lock (m_syncRoot)
            {
                m_activePartitions[generation] = activePartition;
            }
            return activePartition;
        }
    }
}
