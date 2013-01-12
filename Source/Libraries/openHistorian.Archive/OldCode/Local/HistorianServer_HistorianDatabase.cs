////******************************************************************************************************
////  HistorianServer_HistorianManage.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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

//using System;
//using System.Collections.Generic;
//using System.IO;
//using openHistorian.Server.Database;

//namespace openHistorian.Local
//{
//    public partial class HistorianServer
//    {
//        private class HistorianDatabase : IHistorianDatabase
//        {
//            ArchiveDatabaseEngine m_database;

//            public HistorianDatabase(ArchiveDatabaseEngine database)
//            {
//                m_database = database;
//            }

//            /// <summary>
//            /// Determines if this database is currently online.
//            /// </summary>
//            public bool IsOnline
//            {
//                get
//                {
//                    return true;
//                }
//            }

//            /// <summary>
//            /// Opens a stream connection that can be used to read 
//            /// and write data to the current historian database.
//            /// </summary>
//            /// <returns></returns>
//            public IHistorianDataReader OpenDataReader()
//            {
//                return new HistorianDataReader(m_database);
//            }

//            /// <summary>
//            /// Gets the current configuration of the database.
//            /// </summary>
//            /// <returns></returns>
//            public IDatabaseConfig GetConfig()
//            {
//                throw new NotImplementedException();
//            }

//            /// <summary>
//            /// Overwrites the config of the database.
//            /// </summary>
//            /// <param name="config"></param>
//            public void SetConfig(IDatabaseConfig config)
//            {
//                throw new NotImplementedException();
//            }

//            /// <summary>
//            /// Talks the historian database offline
//            /// </summary>
//            /// <param name="waitTimeSeconds">the maximum number of seconds to wait before terminating all client connections.</param>
//            public void TakeOffline(float waitTimeSeconds = 0)
//            {
//                throw new NotImplementedException();
//            }

//            /// <summary>
//            /// Brings this database online.
//            /// </summary>
//            public void BringOnline()
//            {
//                throw new NotImplementedException();
//            }

//            public void Shutdown(float waitTimeSeconds)
//            {
//                //m_server.Dispose();
//            }

//            /// <summary>
//            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//            /// </summary>
//            /// <filterpriority>2</filterpriority>
//            public void Dispose()
//            {
                
//            }
//        }
//    }
//}
