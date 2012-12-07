//******************************************************************************************************
//  HistorianClient.cs - Gbtc
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
//  12/07/2012 - Ritchie
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSF;
using GSF.Communication;

namespace openHistorian
{
    public class HistorianClient : IDisposable
    {
        #region [ Members ]

        // Nested Types

        // Constants

        // Delegates

        // Events
        /// <summary>
        /// Occurs when client connection to the data publication server is established.
        /// </summary>
        public event EventHandler ConnectionEstablished;

        /// <summary>
        /// Occurs when client connection to the data publication server is terminated.
        /// </summary>
        public event EventHandler ConnectionTerminated;

        /// <summary>
        /// Occurs when client connection to the data publication server has successfully authenticated.
        /// </summary>
        public event EventHandler ConnectionAuthenticated;

        /// <summary>
        /// Indicates that processing for an input adapter (via temporal session) has completed.
        /// </summary>
        /// <remarks>
        /// This event is expected to only be raised when an input adapter has been designed to process
        /// a finite amount of data, e.g., reading a historical range of data during temporal procesing.
        /// </remarks>
        public new event EventHandler<EventArgs<string>> ProcessingComplete;

        // Fields
        private TcpClient m_tcpClient;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public HistorianClient()
        {
            m_tcpClient = new TcpClient();
            m_tcpClient.AllowDualStackSocket = true;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="HistorianClient"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~HistorianClient()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        /// <summary>
        /// Releases all the resources used by the <see cref="HistorianClient"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion

        #region [ Operators ]

        #endregion

        #region [ Static ]

        // Static Fields

        // Static Constructor

        // Static Properties

        // Static Methods

        #endregion
        
    }
}
