//******************************************************************************************************
//  MetadataReceiver.cs - Gbtc
//
//  Copyright © 2019, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/11/2019 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF.TimeSeries.Transport;
using System;
using System.Data;
using System.Threading;
using ServerCommand = GSF.TimeSeries.Transport.ServerCommand;

namespace DataExtractor
{
    /// <summary>
    /// Retrieves current meta-data from openHistorian using GEP
    /// </summary>
    internal sealed class MetadataRetriever : IDisposable
    {
        #region [ Members ]

        // Fields
        private DataSubscriber m_subscriber;
        private DataSet m_metadata;
        private Exception m_processException;
        private ManualResetEventSlim m_waitHandle;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MetadataRetriever"/> instance with the specified <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">GEP connection string for openHistorian.</param>
        private MetadataRetriever(string connectionString)
        {
            m_subscriber = new DataSubscriber
            {
                ConnectionString = connectionString,
                ReceiveInternalMetadata = true,
                ReceiveExternalMetadata = true,
                OperationalModes = OperationalModes.UseCommonSerializationFormat | OperationalModes.CompressMetadata,
                CompressionModes = CompressionModes.GZip
            };

            // Attach to needed subscriber events
            m_subscriber.ProcessException += m_subscriber_ProcessException;
            m_subscriber.ConnectionEstablished += m_subscriber_ConnectionEstablished;
            m_subscriber.MetaDataReceived += m_subscriber_MetaDataReceived;

            // Initialize the subscriber
            m_subscriber.Initialize();

            // Create a wait handle to allow time to receive meta-data
            m_waitHandle = new ManualResetEventSlim();

            // Start subscriber connection cycle
            m_subscriber.Start();
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="MetadataRetriever"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~MetadataRetriever()
        {
            Dispose(false);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="MetadataRetriever"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MetadataRetriever"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        if ((object)m_subscriber != null)
                        {
                            // Detach from subscriber events
                            m_subscriber.ProcessException -= m_subscriber_ProcessException;
                            m_subscriber.ConnectionEstablished -= m_subscriber_ConnectionEstablished;
                            m_subscriber.MetaDataReceived -= m_subscriber_MetaDataReceived;

                            m_subscriber.Dispose();
                            m_subscriber = null;
                        }

                        if ((object)m_waitHandle != null)
                        {
                            m_waitHandle.Set(); // Release any waiting threads
                            m_waitHandle.Dispose();
                            m_waitHandle = null;
                        }
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Gets meta-data using GEP waiting no longer than specified <paramref name="timeout"/>.
        /// </summary>
        /// <param name="timeout">Specifies how long to wait, in milliseconds, for meta-data.</param>
        /// <returns>The meta-data received from the GEP publisher in <see cref="DataSet"/> format.</returns>
        private DataSet GetMetadata(int timeout)
        {
            // Wait for meta-data or an exception to occur for up to the specified maximum time, then time out with an exception
            if (!m_waitHandle.Wait(timeout))
                throw new TimeoutException($"Waited for {timeout / 1000.0D} seconds for meta-data, but none was received.");

            // If meta-data was received, return it
            if ((object)m_metadata != null)
                return m_metadata;

            // If a processing exception occurred, re-throw it
            if ((object)m_processException != null)
                throw new InvalidOperationException(m_processException.Message, m_processException);

            // Otherwise return null (unlikely to ever get to this return)
            return null;
        }

        private void m_subscriber_ConnectionEstablished(object sender, EventArgs e)
        {
            // Request meta-data upon successful connection
            m_subscriber.SendServerCommand(ServerCommand.MetaDataRefresh);
        }

        private void m_subscriber_MetaDataReceived(object sender, GSF.EventArgs<DataSet> e)
        {
            m_metadata = e.Argument;

            // Release waiting threads if meta-data received
            m_waitHandle.Set();
        }

        private void m_subscriber_ProcessException(object sender, GSF.EventArgs<Exception> e)
        {
            m_processException = e.Argument;

            // Release waiting threads if an error occurred
            m_waitHandle.Set();
        }

        #endregion

        #region [ Static ]

        /// <summary>
        /// Gets meta-data from the <paramref name="connectionString" /> waiting no longer than the specified <paramref name="timeout"/>.
        /// </summary>
        /// <param name="connectionString">GEP connection string.</param>
        /// <param name="timeout">Specifies how long to wait, in milliseconds, for meta-data.</param>
        /// <returns>The meta-data received from the GEP publisher in <see cref="DataSet"/> format.</returns>
        public static DataSet GetMetadata(string connectionString, int timeout = Timeout.Infinite)
        {
            using (MetadataRetriever receiver = new MetadataRetriever(connectionString))
            {
                return receiver.GetMetadata(timeout);
            }
        }

        #endregion
    }

}
