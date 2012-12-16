////******************************************************************************************************
////  Historian.cs - Gbtc
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
////  9/14/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using openHistorian.Collections.KeyValue;
//using openHistorian.IO.Unmanaged;

//namespace openHistorian.Local
//{
//    public partial class Historian : IHistorianDatabase
//    {
//        SortedTree256 m_tree;
//        BinaryStream m_stream;
//        bool m_disposed;

//        public Historian()
//        {
//            m_stream = new BinaryStream();
//            m_tree = new SortedTree256(m_stream, 4096);
//        }

//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        /// <filterpriority>2</filterpriority>
//        public void Dispose()
//        {
//            if (!m_disposed)
//            {
//                m_disposed = true;
//                m_stream.Dispose();
//            }
//        }

//        /// <summary>
//        /// Determines if this database is currently online.
//        /// </summary>
//        public bool IsOnline
//        {
//            get
//            {
//                return true;
//            }
//        }

//        /// <summary>
//        /// Opens a stream connection that can be used to read 
//        /// and write data to the current historian database.
//        /// </summary>
//        /// <returns></returns>
//        public IHistorianDataReader OpenDataReader()
//        {
//            return new HistorianReadWrite(this);

//        }

//        /// <summary>
//        /// Gets the current configuration of the database.
//        /// </summary>
//        /// <returns></returns>
//        public IDatabaseConfig GetConfig()
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Overwrites the config of the database.
//        /// </summary>
//        /// <param name="config"></param>
//        public void SetConfig(IDatabaseConfig config)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Talks the historian database offline
//        /// </summary>
//        /// <param name="waitTimeSeconds">the maximum number of seconds to wait before terminating all client connections.</param>
//        public void TakeOffline(float waitTimeSeconds = 0)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Brings this database online.
//        /// </summary>
//        public void BringOnline()
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Shuts down this database.
//        /// </summary>
//        /// <param name="waitTimeSeconds"></param>
//        public void Shutdown(float waitTimeSeconds)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
