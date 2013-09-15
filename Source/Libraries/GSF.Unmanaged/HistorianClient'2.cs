//******************************************************************************************************
//  HistorianClient.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
using System.Net;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.Communications;

namespace openHistorian
{
    public class HistorianClientOptions
    {
        public bool IsReadOnly = true;
        public int NetworkPort = 38402;
        public string ServerNameOrIp = "localhost";
        public string DefaultDatabase = "default";
    }

    public class HistorianClient<TKey, TValue>
        : HistorianDatabaseBase<TKey, TValue>,
          IHistorianDatabaseCollection<TKey, TValue>, IDisposable
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {
        private readonly RemoteHistorian<TKey, TValue> m_historian;
        private HistorianDatabaseBase<TKey, TValue> m_currentDatabase;
        private readonly string m_defaultDatabase;

        public HistorianClient(HistorianClientOptions options)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(options.ServerNameOrIp, out ip))
            {
                ip = Dns.GetHostAddresses(options.ServerNameOrIp)[0];
            }
            m_historian = new RemoteHistorian<TKey, TValue>(new IPEndPoint(ip, options.NetworkPort));
            m_defaultDatabase = options.DefaultDatabase;
        }

        public HistorianDatabaseBase<TKey, TValue> GetDatabase()
        {
            return this;
        }

        public IHistorianDatabaseCollection<TKey, TValue> GetDatabaseCollection()
        {
            return this;
        }



        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        public override HistorianDataReaderBase<TKey, TValue> OpenDataReader()
        {
            ConnectIfNotConnected();
            return m_currentDatabase.OpenDataReader();
        }

        public override void Write(KeyValueStream<TKey, TValue> points)
        {
            ConnectIfNotConnected();
            m_currentDatabase.Write(points);
        }

        public override void Write(TKey key, TValue value)
        {
            ConnectIfNotConnected();
            m_currentDatabase.Write(key, value);
        }

        public override void SoftCommit()
        {
            throw new NotImplementedException();
        }

        public override void HardCommit()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnects from the current database. 
        /// </summary>
        public override void Disconnect()
        {
            if (m_currentDatabase != null)
                m_currentDatabase.Disconnect();
            m_currentDatabase = null;
        }

        HistorianDatabaseBase<TKey, TValue> IHistorianDatabaseCollection<TKey, TValue>.this[string databaseName]
        {
            get
            {
                if (m_currentDatabase != null)
                    m_currentDatabase.Disconnect();

                m_currentDatabase = m_historian[databaseName];

                return m_currentDatabase;
            }
        }

        private void ConnectIfNotConnected()
        {
            if (m_currentDatabase == null)
                m_currentDatabase = ((IHistorianDatabaseCollection<TKey, TValue>)this)[m_defaultDatabase];
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            m_historian.Disconnect();
        }
    }
}