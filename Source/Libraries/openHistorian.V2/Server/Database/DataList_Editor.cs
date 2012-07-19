//******************************************************************************************************
//  DataList_Editor.cs - Gbtc
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
using System.Collections.ObjectModel;
using System.Threading;
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    partial class DataList
    {
        public class Editor : IDisposable
        {
            bool m_disposed;
            DataList m_collection;
            ReadOnlyCollection<PartitionStateInformation> m_partitions;

            public Editor(DataList collection)
            {
                m_collection = collection;
                m_partitions = new ReadOnlyCollection<PartitionStateInformation>(collection.m_partitions);
                Monitor.Enter(m_collection.m_syncRoot);
            }

            /// <summary>
            /// Represents a readonly list of all of the partitions 
            /// </summary>
            public ReadOnlyCollection<PartitionStateInformation> Partitions
            {
                get
                {
                    if (m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    return m_partitions;
                }
            }

            public void ReleaseEditLock(PartitionFile partition)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var partitions = m_collection.m_partitions;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].Summary.PartitionFileFile == partition)
                    {
                        partitions[x].IsEditLocked = false;
                        return;
                    }
                }
            }

            /// <summary>
            /// Renews the snapshot of the partition file. This will acquire the latest 
            /// read transaction so all new snapshots will use this later version.
            /// </summary>
            /// <param name="partition">the file to update the snapshot on.</param>
            /// <returns></returns>
            public bool RenewSnapshot(PartitionFile partition)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var partitions = m_collection.m_partitions;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].Summary.PartitionFileFile == partition)
                    {
                        partitions[x].Summary = new PartitionSummary(partition);
                        return true;
                    }
                }
                return false;
            }

            public void Add(PartitionFile partition, PartitionStateInformation stateInformation)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                stateInformation.Summary = new PartitionSummary(partition);
                m_collection.m_partitions.Add(stateInformation);
            }

            /// <summary>
            /// Removes the first occurnace of <see cref="partition"/> from <see cref="DataList"/>.
            /// </summary>
            /// <param name="partition">The partition to remove</param>
            /// <param name="listRemovalStatus">A <see cref="DataListRemovalStatus"/> that can be used to determine
            /// when this resource is no longer being used and can be closed as a result.  
            /// Closing prematurely can cause erratic behaviour which may result in 
            /// data coruption and the application crashing.  Value is null if no item can be found.</param>
            /// <returns>True if the item was removed, False otherwise.</returns>
            /// <exception cref="Exception">Thrown if <see cref="partition"/> is not in this list.</exception>
            public bool Remove(PartitionFile partition, out DataListRemovalStatus listRemovalStatus)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                var partitions = m_collection.m_partitions;
                for (int x = 0; x < partitions.Count; x++)
                {
                    if (partitions[x].Summary.PartitionFileFile == partition)
                    {
                        listRemovalStatus = new DataListRemovalStatus(partitions[x].Summary.PartitionFileFile, m_collection);
                        partitions.RemoveAt(x);
                        return true;
                    }
                }
                listRemovalStatus = null;
                return false;
            }

            /// <summary>
            /// Releases the lock on the <see cref="DataList"/>.
            /// </summary>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    Monitor.Exit(m_collection.m_syncRoot);
                    m_partitions = null;
                    m_collection = null;
                    m_disposed = true;
                }
            }
        }
    }
}
