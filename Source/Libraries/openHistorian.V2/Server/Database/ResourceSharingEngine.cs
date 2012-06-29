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
    class ResourceSharingEngine
    {
        bool m_writeLockGeneration0Active;
        bool m_writeLockGeneration0Processing;
        bool m_writeLockGeneration1Active;
        bool m_writeLockGeneration1Processing;
        bool m_writeLockGeneration2Active;
        bool m_writeLockGeneration2Processing;

        PartitionSummary m_archiveGeneration0Active;
        PartitionSummary m_archiveGeneration0Processing;

        PartitionSummary m_archiveGeneration1Active;
        PartitionSummary m_archiveGeneration1Processing;

        PartitionSummary m_archiveGeneration2Active;
        PartitionSummary m_archiveGeneration2Processing;

        /// <summary>
        /// Contais the archives that have successfully been rolled over and are pending
        /// deletion from the system.
        /// </summary>
        List<PartitionSummary> m_pendingDeletes;

        /// <summary>
        /// Contains the list of archives that are perminent to the system and 
        /// cannot be written to.
        /// </summary>
        List<PartitionSummary> m_perminentArchives;


        public void AquireSnapshot(TransactionResources transaction)
        {
            lock (this)
            {
                int count = m_perminentArchives.Count + 6;
                PartitionSummary[] partitions = new PartitionSummary[count];

                partitions[0] = m_archiveGeneration0Active;
                partitions[1] = m_archiveGeneration0Processing;
                partitions[2] = m_archiveGeneration1Active;
                partitions[3] = m_archiveGeneration1Processing;
                partitions[4] = m_archiveGeneration2Active;
                partitions[5] = m_archiveGeneration2Processing;

                for (int x = 0; x < m_perminentArchives.Count; x++)
                {
                    partitions[x + 6] = m_perminentArchives[x];
                }
                transaction.Tables = partitions;
            }
        }

        public void RequestInsertIntoGeneration0(Action<PartitionFile> callback)
        {
            while (true)
            {
                lock (this)
                {
                    if (!m_writeLockGeneration0Active)
                    {
                        m_writeLockGeneration0Active = true;
                        break;
                    }
                }
                Thread.Sleep(100);
            }

            callback.Invoke(m_archiveGeneration0Active.PartitionFileFile);

            lock (this)
            {
                m_writeLockGeneration0Active = false;
                PartitionSummary newPartition = m_archiveGeneration0Active.CloneEditableCopy();
                newPartition.ActiveSnapshot = newPartition.PartitionFileFile.CreateSnapshot();
                newPartition.FirstKeyValue = newPartition.PartitionFileFile.GetFirstKey1;
                newPartition.LastKeyValue = newPartition.PartitionFileFile.GetLastKey2;
                newPartition.KeyMatchMode = PartitionSummary.MatchMode.Bounded;
                m_archiveGeneration0Active = newPartition;
            }
        }

        public void RequestRolloverGeneration1(Action<PartitionSummary, PartitionSummary> callback) { }
        public void RequestRolloverGeneration2(Action<PartitionSummary, PartitionSummary> callback) { }

        public void RequestRolloverGeneration0(Action<PartitionSummary, PartitionSummary> callback)
        {
            while (true)
            {
                lock (this)
                {
                    if (m_archiveGeneration0Processing == null)
                    {
                        m_archiveGeneration0Processing = m_archiveGeneration0Active;
                        PartitionFile newPartitionFile = new PartitionFile();
                        PartitionSummary newPartition = new PartitionSummary();
                        newPartition.PartitionFileFile = newPartitionFile;
                        newPartition.ActiveSnapshot = newPartitionFile.CreateSnapshot();
                        newPartition.KeyMatchMode = PartitionSummary.MatchMode.EmptyEntry;
                        newPartition.IsReadOnly = true;
                        m_archiveGeneration0Active = newPartition;
                        break;
                    }
                }
                Thread.Sleep(100);
            }

            callback.Invoke(m_archiveGeneration0Processing, m_archiveGeneration1Active);

            lock (this)
            {
                PartitionSummary newPartition = m_archiveGeneration0Processing.CloneEditableCopy();

            }
        }

        public void RequestRolloverGeneration0(PartitionSummary sourceArchive, PartitionSummary destinationArchive)
        {
            //bool readyToRollOver = false;
            //lock (this)
            //{
            //    if (!m_activeArchives.Contains(sourceArchive))
            //        throw new Exception("sourceArchiveMissing");

            //    if (!m_activeArchives.Contains(destinationArchive))
            //        throw new Exception("destinationArchiveMissing");

            //    if (m_activeArchivesWriteLocks.Contains(destinationArchive))
            //        throw new Exception("destinationArchive is write locked");

            //    m_activeArchives.Remove(sourceArchive);
            //    m_pendingRollovers.Add(sourceArchive);
            //    m_activeArchivesWriteLocks.Add(destinationArchive);

            //    readyToRollOver = CanRollOver(sourceArchive);

            //}

            //while (!readyToRollOver)
            //{
            //    Thread.Sleep(1000);
            //    lock (this)
            //    {
            //        readyToRollOver = CanRollOver(sourceArchive);
            //    }
            //}

            //ProcessRollover(sourceArchive, destinationArchive);

            //lock (this)
            //{
            //    m_activeArchivesWriteLocks.Remove(destinationArchive);
            //    m_pendingRollovers.Remove(sourceArchive);
            //    m_pendingDeletes.Add(sourceArchive);
            //    m_activeArchives.Remove(destinationArchive);
            //    var newDestination = destinationArchive.CloneEditableCopy();
            //    newDestination.ActiveSnapshot = newDestination.ArchiveFile.CreateSnapshot();
            //    m_activeArchives.Add(newDestination);
            //}

        }

        bool ProcessRollover(PartitionSummary sourceArchive, PartitionSummary destinationArchive)
        {
            var source = sourceArchive.ActiveSnapshot.OpenInstance();
            var dest = destinationArchive.PartitionFileFile;

            dest.BeginEdit();

            var reader = source.GetDataRange();
            reader.SeekToKey(0, 0);

            ulong value1, value2, key1, key2;
            while (reader.GetNextKey(out key1, out key2, out value1, out value2))
            {
                dest.AddPoint(key1, key2, value1, value2);
            }

            dest.CommitEdit();
            return true;
        }
    }
}
