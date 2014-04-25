//******************************************************************************************************
//  HistorianDatabaseInstance.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  07/25/2013 - J. Ritchie Carroll
//       Derived code from original HistorianServerOptions.
//
//******************************************************************************************************

using System.Collections.Generic;
using GSF;

namespace openHistorian
{
    /// <summary>
    /// Represents a database instance of a historian to be hosted by a <see cref="HistorianServer"/>.
    /// </summary>
    public class HistorianDatabaseInstance
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Defines the default network port for a <see cref="HistorianDatabaseInstance"/>.
        /// </summary>
        public const int DefaultNetworkPort = 38402;

        // Fields

        /// <summary>
        /// Data paths used by this <see cref="HistorianDatabaseInstance"/>.
        /// </summary>
        public string[] Paths;

        /// <summary>
        /// Determines if this this <see cref="HistorianDatabaseInstance"/> is archived to disk or memory.
        /// </summary>
        public bool InMemoryArchive = true;

        /// <summary>
        /// The socket connection parameters for this <see cref="HistorianDatabaseInstance"/>.
        /// </summary>
        public string ConnectionString;

        /// <summary>
        /// The database instance name for this <see cref="HistorianDatabaseInstance"/>.
        /// </summary>
        /// <remarks>
        /// Unless overridden, name is "default".
        /// </remarks>
        public string DatabaseName = "default";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets flag that determines if this <see cref="HistorianDatabaseInstance"/> is exposed via network socket.
        /// </summary>
        /// <remarks>
        /// Property value is inferred based on existence of <see cref="ConnectionString"/>. Empty or <c>null</c> connection
        /// strings are determined to mean that <see cref="HistorianDatabaseInstance"/> will not be exposed via a network
        /// socket. Setting value to <c>true</c> when no <see cref="ConnectionString"/> exists will create a connection
        /// string established with a default port number.
        /// </remarks>
        public bool IsNetworkHosted
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ConnectionString);
            }
            set
            {
                if (value)
                {
                    // Define a default connection string when none exists
                    if (string.IsNullOrWhiteSpace(ConnectionString))
                        ConnectionString = "port=" + DefaultNetworkPort;
                }
                else
                {
                    ConnectionString = null;
                }
            }
        }

        /// <summary>
        /// Gets port number parsed from <see cref="ConnectionString"/>, or <c>-1</c> if no connection string is defined.
        /// </summary>
        public int PortNumber
        {
            get
            {
                return GetPortNumber(ConnectionString);
            }
        }

        #endregion

        #region [ Static ]

        // Static Methods

        /// <summary>
        /// Gets the network server port number parsed from the <paramref name="connectionString"/>, or <c>-1</c> if no connection string is defined.
        /// </summary>
        /// <param name="connectionString">The key/value pair connection string to parse.</param>
        /// <returns>Network server port number parsed from the <paramref name="connectionString"/>, or <c>-1</c> if no connection string is defined.</returns>
        public static int GetPortNumber(string connectionString)
        {
            int port = -1;

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
                string value;

                if (!settings.TryGetValue("port", out value) || !int.TryParse(value, out port))
                    port = DefaultNetworkPort;
            }

            return port;
        }

        #endregion
    }
}
