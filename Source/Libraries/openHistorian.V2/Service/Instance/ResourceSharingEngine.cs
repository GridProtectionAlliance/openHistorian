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
using openHistorian.V2.Service.Instance.File;

namespace openHistorian.V2.Service.Instance
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

        TableSummaryInfo m_archiveGeneration0Active;
        TableSummaryInfo m_archiveGeneration0Processing;

        TableSummaryInfo m_archiveGeneration1Active;
        TableSummaryInfo m_archiveGeneration1Processing;

        TableSummaryInfo m_archiveGeneration2Active;
        TableSummaryInfo m_archiveGeneration2Processing;

        /// <summary>
        /// Contais the archives that have successfully been rolled over and are pending
        /// deletion from the system.
        /// </summary>
        List<TableSummaryInfo> m_pendingDeletes;
        /// <summary>
        /// Contains the list of archives that are perminent to the system and 
        /// cannot be written to.
        /// </summary>
        List<TableSummaryInfo> m_perminentArchives;

        /// <summary>
        /// Contains the complete list of system snapshots.
        /// </summary>
        List<TableSnapshot> m_activeSnapshots;


        public TableSnapshot AquireSnapshot()
        {
            lock (this)
            {
                TableSnapshot snapshot = new TableSnapshot(RemoveSnapshot);

                if (m_archiveGeneration0Active != null)
                    snapshot.Tables.Add(m_archiveGeneration0Active);

                if (m_archiveGeneration0Processing != null)
                    snapshot.Tables.Add(m_archiveGeneration0Processing);

                if (m_archiveGeneration1Active != null)
                    snapshot.Tables.Add(m_archiveGeneration1Active);

                if (m_archiveGeneration1Processing != null)
                    snapshot.Tables.Add(m_archiveGeneration1Processing);

                if (m_archiveGeneration2Active != null)
                    snapshot.Tables.Add(m_archiveGeneration2Active);

                if (m_archiveGeneration2Processing != null)
                    snapshot.Tables.Add(m_archiveGeneration2Processing);

                foreach (var table in m_perminentArchives)
                {
                    snapshot.Tables.Add(table);
                }
                m_activeSnapshots.Add(snapshot);
                return snapshot;
            }
        }

        void RemoveSnapshot(TableSnapshot snapshot)
        {
            lock (this)
            {
                m_activeSnapshots.Remove(snapshot);
            }
        }
        public void RequestInsertIntoGeneration0(Action<Archive> callback)
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

            callback.Invoke(m_archiveGeneration0Active.ArchiveFile);

            lock (this)
            {
                m_writeLockGeneration0Active = false;
                TableSummaryInfo newTableInfo = m_archiveGeneration0Active.CloneEditableCopy();
                newTableInfo.ActiveSnapshot = newTableInfo.ArchiveFile.CreateSnapshot();
                newTableInfo.FirstTime = newTableInfo.ArchiveFile.GetFirstTimeStamp;
                newTableInfo.LastTime = newTableInfo.ArchiveFile.GetLastTimeStamp;
                newTableInfo.TimeMatchMode = TableSummaryInfo.MatchMode.Bounded;
                m_archiveGeneration0Active = newTableInfo;
            }
        }

        public void RequestRolloverGeneration1(Action<TableSummaryInfo, TableSummaryInfo> callback) { }
        public void RequestRolloverGeneration2(Action<TableSummaryInfo, TableSummaryInfo> callback) { }
        public void RequestRolloverGeneration0(Action<TableSummaryInfo, TableSummaryInfo> callback)
        {
            while (true)
            {
                lock (this)
                {
                    if (m_archiveGeneration0Processing == null)
                    {
                        m_archiveGeneration0Processing = m_archiveGeneration0Active;
                        Archive newArchive = new Archive();
                        TableSummaryInfo newTableInfo = new TableSummaryInfo();
                        newTableInfo.ArchiveFile = newArchive;
                        newTableInfo.ActiveSnapshot = newArchive.CreateSnapshot();
                        newTableInfo.TimeMatchMode = TableSummaryInfo.MatchMode.EmptyEntry;
                        newTableInfo.IsReadOnly = true;
                        m_archiveGeneration0Active = newTableInfo;
                        break;
                    }
                }
                Thread.Sleep(100);
            }

            callback.Invoke(m_archiveGeneration0Processing, m_archiveGeneration1Active);

            lock (this)
            {
                TableSummaryInfo newTableInfo = m_archiveGeneration0Processing.CloneEditableCopy();

            }
        }

        public void RequestRolloverGeneration0(TableSummaryInfo sourceArchive, TableSummaryInfo destinationArchive)
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

        bool ProcessRollover(TableSummaryInfo sourceArchive, TableSummaryInfo destinationArchive)
        {
            var source = sourceArchive.ActiveSnapshot.OpenInstance();
            var dest = destinationArchive.ArchiveFile;

            dest.BeginEdit();

            var reader = source.GetDataRange();
            reader.SeekToKey(0,0);

            ulong value1, value2, key1, key2;
            while (reader.GetNextKey(out key1, out key2, out value1, out value2))
            {
                dest.AddPoint(key1,key2,value1,value2);
            }

            dest.CommitEdit();
            return true;
        }
    }
}
