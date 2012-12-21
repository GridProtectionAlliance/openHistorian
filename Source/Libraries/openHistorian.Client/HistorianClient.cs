//******************************************************************************************************
//  HistorianClient.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using openHistorian.Communications;
using System.Net;

namespace openHistorian
{
    public class HistorianClientOptions
    {
        public bool IsReadOnly = true;
        public int NetworkPort = 38402;
        public string ServerNameOrIp = "localhost";
        public string DefaultDatabase = "default";
    }

    public class HistorianClient : IHistorianDatabase, IHistorianDatabaseCollection, IDisposable
    {
        RemoteHistorian m_historian;
        IHistorianDatabase m_currentDatabase;
        string m_defaultDatabase;

        public HistorianClient(HistorianClientOptions options)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(options.ServerNameOrIp, out ip))
            {
                ip = Dns.GetHostAddresses(options.ServerNameOrIp)[0];
            }
            m_historian = new RemoteHistorian(new IPEndPoint(ip, options.NetworkPort));
            m_defaultDatabase = options.DefaultDatabase;

        }

        public IHistorianDatabase GetDatabase()
        {
            return this;
        }

        public IHistorianDatabaseCollection GetDatabaseCollection()
        {
            return this;
        }

        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        IHistorianDataReader IHistorianDatabase.OpenDataReader()
        {
            ConnectIfNotConnected();
            return m_currentDatabase.OpenDataReader();
        }

        void IHistorianDatabase.Write(IPointStream points)
        {
            ConnectIfNotConnected();
            m_currentDatabase.Write(points);
        }

        void IHistorianDatabase.Write(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            ConnectIfNotConnected();
            m_currentDatabase.Write(key1, key2, value1, value2);
        }

        void IHistorianDatabase.SoftCommit()
        {
            throw new NotImplementedException();
        }

        void IHistorianDatabase.HardCommit()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnects from the current database. 
        /// </summary>
        void IHistorianDatabase.Disconnect()
        {
            if (m_currentDatabase != null)
                m_currentDatabase.Disconnect();
            m_currentDatabase = null;
        }

        IHistorianDatabase IHistorianDatabaseCollection.ConnectToDatabase(string databaseName)
        {
            if (m_currentDatabase != null)
                m_currentDatabase.Disconnect();
            m_currentDatabase = m_historian.ConnectToDatabase(databaseName);
            return m_currentDatabase;
        }

        void ConnectIfNotConnected()
        {
            if (m_currentDatabase == null)
                m_currentDatabase = ((IHistorianDatabaseCollection)this).ConnectToDatabase(m_defaultDatabase);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            m_historian.Disconnect();
        }


    }
}
