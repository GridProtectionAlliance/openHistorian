////******************************************************************************************************
////  HistorianDataStream.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  10/25/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System.Collections.Generic;
//using openHistorian.Server.Database;

//namespace openHistorian.Local
//{
//    public class HistorianDataReader : IHistorianDataReader
//    {
//        ArchiveDatabaseEngine m_database;
//        ArchiveReader m_reader;
//        public HistorianDataReader(ArchiveDatabaseEngine database)
//        {
//            m_database = database;
//        }

//        public IPointStream Read(ulong key)
//        {
//            if (m_reader == null)
//                m_reader = m_database.CreateReader();
//            return m_reader.Read(key);
//        }

//        public IPointStream Read(ulong startKey, ulong endKey)
//        {
//            if (m_reader == null)
//                m_reader = m_database.CreateReader();
//            return m_reader.Read(startKey, endKey);
//        }

//        public IPointStream Read(ulong startKey, ulong endKey, IEnumerable<ulong> points)
//        {
//            if (m_reader == null)
//                m_reader = m_database.CreateReader();
//            return m_reader.Read(startKey, endKey, points);
//        }

//        public void Write(IPointStream points)
//        {
//            ulong key1, key2, value1, value2;
//            while (points.Read(out key1, out key2, out value1, out value2))
//                m_database.WriteData(key1, key2, value1, value2);
//        }

//        public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
//        {
//            m_database.WriteData(key1, key2, value1, value2);
//        }

//        public long WriteBulk(IPointStream points)
//        {
//            ulong key1, key2, value1, value2;
//            while (points.Read(out key1, out key2, out value1, out value2))
//                m_database.WriteData(key1, key2, value1, value2);
//            return -1;
//        }

//        public bool IsCommitted(long transactionId)
//        {
//            return m_database.IsCommitted(transactionId);
//        }

//        public bool IsDiskCommitted(long transactionId)
//        {
//            return m_database.IsDiskCommitted(transactionId);
//        }

//        public bool WaitForCommitted(long transactionId)
//        {
//            return m_database.WaitForCommitted(transactionId);
//        }

//        public bool WaitForDiskCommitted(long transactionId)
//        {
//            return m_database.WaitForDiskCommitted(transactionId);
//        }

//        public void Commit()
//        {
//            m_database.Commit();
//        }

//        public void CommitToDisk()
//        {
//            m_database.CommitToDisk();
//        }

//        public long LastCommittedTransactionId
//        {
//            get
//            {
//                return m_database.LastCommittedTransactionId;
//            }
//        }
//        public long LastDiskCommittedTransactionId
//        {
//            get
//            {
//                return m_database.LastDiskCommittedTransactionId;
//            }
//        }
//        public long CurrentTransactionId
//        {
//            get
//            {
//                return m_database.CurrentTransactionId;
//            }
//        }

//        public void Disconnect()
//        {
//            if (m_reader != null)
//            {
//                m_reader.Dispose();
//                m_reader = null;
//            }
//        }

//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        /// <filterpriority>2</filterpriority>
//        public void Dispose()
//        {
//            Disconnect();
//        }
//    }
//}
