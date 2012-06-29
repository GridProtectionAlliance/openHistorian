//******************************************************************************************************
//  ResourceSharingEngine.cs - Gbtc
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
    class ResourceEngine
    {
        public const int GenerationCount = 3;
        object[] m_editLocks;
        PartitionSummary[] m_activePartitions;
        PartitionSummary[] m_processingPartitions;

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

        public ResourceEngine()
        {
            m_editLocks = new object[GenerationCount];
            m_activePartitions = new PartitionSummary[GenerationCount];
            m_processingPartitions = new PartitionSummary[GenerationCount];
            for (int x = 0; x < GenerationCount; x++)
            {
                m_editLocks[x] = new object();
            }
        }

        public void AquireSnapshot(TransactionResources transaction)
        {
            lock (this)
            {
                int count = m_finalPartitions.Count + 6;
                PartitionSummary[] partitions = new PartitionSummary[count];

                m_activePartitions.CopyTo(partitions, 0);
                m_processingPartitions.CopyTo(partitions, GenerationCount);
                m_finalPartitions.CopyTo(GenerationCount * 2, partitions, 0, m_finalPartitions.Count);
            }
        }

        /// <summary>
        /// Acquires a edit lock on a generation's active partition.
        /// Must call corresponding release lock or a deadlock can occur.
        /// </summary>
        /// <remarks>
        /// A generation 0 lock is aquired to synchronize edits to this class.
        /// </remarks>
        public void AcquireEditLock(int generation)
        {
            if (generation < 0 || (generation >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generation");
            Monitor.Enter(m_editLocks[generation]);
        }

        public void ReleaseEditLock(int generation)
        {
            if (generation < 0 || (generation >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generation");
            Monitor.Exit(m_editLocks[generation]);
        }

        public PartitionSummary GetActivePartition(int generation)
        {
            if (generation < 0 || (generation >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generation");
            lock (this)
            {
                return m_activePartitions[generation];
            }
        }

        public PartitionSummary GetProcessingPartition(int generation)
        {
            if (generation < 0 || (generation >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generation");
            lock (this)
            {
                return m_processingPartitions[generation];
            }
        }

        public void SetActivePartition(int generation, PartitionSummary partition)
        {
            if (generation < 0 || (generation >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generation");
            PartitionSummary newPartition = partition.CloneEditableCopy();
            newPartition.ActiveSnapshot = newPartition.PartitionFileFile.CreateSnapshot();
            newPartition.FirstKeyValue = newPartition.PartitionFileFile.GetFirstKey1;
            newPartition.LastKeyValue = newPartition.PartitionFileFile.GetLastKey2;
            newPartition.KeyMatchMode = PartitionSummary.MatchMode.Bounded;
            lock (this)
            {
                m_activePartitions[generation] = newPartition;
            }
        }

        public void ReplaceActivePartition(int generation, PartitionSummary newActivePartition)
        {
            if (generation < 0 || (generation >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generation");
            AcquireEditLock(generation);
            lock (this)
            {
                m_processingPartitions[generation] = m_activePartitions[generation];
                m_activePartitions[generation] = newActivePartition;
            }
            ReleaseEditLock(generation);
        }

        public void CommittActivePartitionAndRemoveProcessing(int generationCommit, int generationRemove)
        {
            if (generationCommit < 0 || (generationCommit >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generationCommit");
                     if (generationRemove < 0 || (generationRemove >= m_editLocks.Length))
                throw new ArgumentOutOfRangeException("generationRemove");

            PartitionSummary newPartition = m_activePartitions[generationCommit];
            newPartition.ActiveSnapshot = newPartition.PartitionFileFile.CreateSnapshot();
            newPartition.FirstKeyValue = newPartition.PartitionFileFile.GetFirstKey1;
            newPartition.LastKeyValue = newPartition.PartitionFileFile.GetLastKey2;
            newPartition.KeyMatchMode = PartitionSummary.MatchMode.Bounded;
            lock (this)
            {
                m_activePartitions[generationCommit] = newPartition;
                var generation = m_processingPartitions[generationRemove];
                m_processingPartitions[generationRemove] = null;
                m_partitionsPendingDeletions.Add(generation);
            }

        }

      
    }
}
