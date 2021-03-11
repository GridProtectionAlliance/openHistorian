//******************************************************************************************************
//  ServerSettings.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/01/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Data;
using GSF.IO;
using System.IO;
using GSF.Immutable;
using GSF.Snap.Services.Net;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Settings for <see cref="SnapServer"/>
    /// </summary>
    public class ServerSettings
        : SettingsBase<ServerSettings>, IToServerSettings
    {
        /// <summary>
        /// Lists all of the databases that are part of the server
        /// </summary>
        private readonly ImmutableList<ServerDatabaseSettings> m_databases;

        /// <summary>
        /// All of the socket based listeners for the database.
        /// </summary>
        private readonly ImmutableList<SnapSocketListenerSettings> m_listeners;

        /// <summary>
        /// Creates a new instance of <see cref="ServerSettings"/>
        /// </summary>
        public ServerSettings()
        {
            m_databases = new ImmutableList<ServerDatabaseSettings>(x =>
            {
                if (x is null)
                    throw new ArgumentNullException("value");
                return x;
            });
            m_listeners = new ImmutableList<SnapSocketListenerSettings>(x =>
            {
                if (x is null)
                    throw new ArgumentNullException("value");
                return x;
            });
        }

        /// <summary>
        /// Lists all of the databases that are part of the server
        /// </summary>
        public ImmutableList<ServerDatabaseSettings> Databases => m_databases;

        /// <summary>
        /// All of the socket based listeners for the database.
        /// </summary>
        public ImmutableList<SnapSocketListenerSettings> Listeners => m_listeners;

        /// <summary>
        /// Creates a <see cref="ServerSettings"/> configuration that can be used for <see cref="SnapServer"/>
        /// </summary>
        /// <returns></returns>
        ServerSettings IToServerSettings.ToServerSettings()
        {
            return this;
        }

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(m_databases.Count);
            foreach (ServerDatabaseSettings databaseSettings in m_databases)
            {
                databaseSettings.Save(stream);
            }
            stream.Write(m_listeners.Count);
            foreach (SnapSocketListenerSettings listenerSettings in m_listeners)
            {
                listenerSettings.Save(stream);
            }
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    int cnt = stream.ReadInt32();
                    m_databases.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        ServerDatabaseSettings database = new ServerDatabaseSettings();
                        database.Load(stream);
                        m_databases.Add(database);
                    }
                    cnt = stream.ReadInt32();
                    m_listeners.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        SnapSocketListenerSettings listener = new SnapSocketListenerSettings();
                        listener.Load(stream);
                        m_listeners.Add(listener);
                    }
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);

            }
        }

        public override void Validate()
        {
            foreach (ServerDatabaseSettings db in m_databases)
            {
                db.Validate();
            }
            foreach (SnapSocketListenerSettings lst in m_listeners)
            {
                lst.Validate();
            }
        }
    }
}
