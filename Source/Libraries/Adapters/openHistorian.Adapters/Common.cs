////******************************************************************************************************
////  HistorianInstance.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
////  07/19/2013 - Ritchie
////       Generated original version of source code.
////
////******************************************************************************************************

//namespace openHistorian.Adapters
//{
//    /// <summary>
//    /// Represents to the static singleton historian server API instance used by all adapters as initialized by the service host.
//    /// </summary>
//    /// <remarks>
//    /// For better performance, locking coordination between the archive and server components and shared memory utilization
//    /// only a single server historian API object is created, even for multiple historian "instances" hosted by this process.
//    /// </remarks>
//    public static class Common
//    {
//        private static readonly HistorianServer s_historianServer;

//        static Common()
//        {
//            s_historianServer = new HistorianServer(38402);
//        }

//        /// <summary>
//        /// Shared instance of historian server API used by time-series adapters.
//        /// </summary>
//        public static HistorianServer HistorianServer
//        {
//            get
//            {
//                return s_historianServer;
//            }
//        }
//    }
//}