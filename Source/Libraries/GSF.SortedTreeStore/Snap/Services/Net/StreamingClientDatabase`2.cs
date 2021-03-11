//******************************************************************************************************
//  StreamingClientDatabase`2.cs - Gbtc
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
//  12/08/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Net;
using GSF.Snap.Filters;
using GSF.Snap.Services.Reader;
using GSF.Snap.Streaming;

namespace GSF.Snap.Services.Net
{
    /// <summary>
    /// A socket based client that extends connecting to a database.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class StreamingClientDatabase<TKey, TValue>
        : ClientDatabaseBase<TKey, TValue>

        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private BulkWriting m_writer;
        private readonly TKey m_tmpKey;
        private readonly TValue m_tmpValue;
        private PointReader m_reader;
        private bool m_disposed;
        private readonly RemoteBinaryStream m_stream;
        private readonly Action m_onDispose;
        private StreamEncodingBase<TKey, TValue> m_encodingMode;
        private readonly DatabaseInfo m_info;

        /// <summary>
        /// Creates a streaming wrapper around a database.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="onDispose"></param>
        /// <param name="info"></param>
        public StreamingClientDatabase(RemoteBinaryStream stream, Action onDispose, DatabaseInfo info)
        {
            m_info = info;
            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
            m_onDispose = onDispose;
            m_stream = stream;
        }

        /// <summary>
        /// Defines the encoding method to use for the server.
        /// </summary>
        /// <param name="encoding"></param>
        public void SetEncodingMode(EncodingDefinition encoding)
        {
            m_encodingMode = Library.CreateStreamEncoding<TKey, TValue>(encoding);
            m_stream.Write((byte)ServerCommand.SetEncodingMethod);
            encoding.Save(m_stream);
            m_stream.Flush();

            ServerResponse command = (ServerResponse)m_stream.ReadUInt8();
            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception("Server UnhandledExcetion: \n" + exception);
                case ServerResponse.UnknownEncodingMethod:
                    throw new Exception("Server does not recgonize encoding method");
                case ServerResponse.EncodingMethodAccepted:
                    break;
                default:
                    throw new Exception("Unknown server response: " + command.ToString());
            }
        }


        /// <summary>
        /// Reads data from the SortedTreeEngine with the provided read options and server side filters.
        /// </summary>
        /// <param name="readerOptions">read options supplied to the reader. Can be null.</param>
        /// <param name="keySeekFilter">a seek based filter to follow. Can be null.</param>
        /// <param name="keyMatchFilter">a match based filer to follow. Can be null.</param>
        /// <returns>A stream that will read the specified data.</returns>
        public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
        {
            if (m_reader != null)
                throw new Exception("Sockets do not support concurrent readers. Dispose of old reader.");

            m_stream.Write((byte)ServerCommand.Read);
            if (keySeekFilter is null)
            {
                m_stream.Write(false);
            }
            else
            {
                m_stream.Write(true);
                m_stream.Write(keySeekFilter.FilterType);
                keySeekFilter.Save(m_stream);
            }

            if (keyMatchFilter is null)
            {
                m_stream.Write(false);
            }
            else
            {
                m_stream.Write(true);
                m_stream.Write(keyMatchFilter.FilterType);
                keyMatchFilter.Save(m_stream);
            }

            if (readerOptions is null)
            {
                m_stream.Write(false);
            }
            else
            {
                m_stream.Write(true);
                readerOptions.Save(m_stream);
            }
            m_stream.Flush();


            ServerResponse command = (ServerResponse)m_stream.ReadUInt8();
            switch (command)
            {
                case ServerResponse.UnhandledException:
                    string exception = m_stream.ReadString();
                    throw new Exception("Server UnhandledExcetion: \n" + exception);
                case ServerResponse.UnknownOrCorruptSeekFilter:
                    throw new Exception("Server does not recgonize the seek filter");
                case ServerResponse.UnknownOrCorruptMatchFilter:
                    throw new Exception("Server does not recgonize the match filter");
                case ServerResponse.UnknownOrCorruptReaderOptions:
                    throw new Exception("Server does not recgonize the reader options");
                case ServerResponse.SerializingPoints:
                    break;
                case ServerResponse.ErrorWhileReading:
                    exception = m_stream.ReadString();
                    throw new Exception("Server Error While Reading: \n" + exception);
                default:
                    throw new Exception("Unknown server response: " + command.ToString());
            }

            m_reader = new PointReader(m_encodingMode, m_stream, () => m_reader = null);
            return m_reader;
        }


        /// <summary>
        /// Writes the tree stream to the database. 
        /// </summary>
        /// <param name="stream">all of the key/value pairs to add to the database.</param>
        public override void Write(TreeStream<TKey, TValue> stream)
        {
            if (m_reader != null)
                throw new Exception("Sockets do not support writing while a reader is open. Dispose of reader.");

            m_stream.Write((byte)ServerCommand.Write);
            m_encodingMode.ResetEncoder();
            while (stream.Read(m_tmpKey, m_tmpValue))
            {
                m_encodingMode.Encode(m_stream, m_tmpKey, m_tmpValue);
            }
            m_encodingMode.WriteEndOfStream(m_stream);
            m_stream.Flush();
        }

        /// <summary>
        /// Writes an individual key/value to the sorted tree store.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Write(TKey key, TValue value)
        {
            if (m_reader != null)
                throw new Exception("Sockets do not support writing while a reader is open. Dispose of reader.");

            m_stream.Write((byte)ServerCommand.Write);
            m_encodingMode.ResetEncoder();
            m_encodingMode.Encode(m_stream, key, value);
            m_encodingMode.WriteEndOfStream(m_stream);
            m_stream.Flush();
        }

        /// <summary>
        /// Due to the blocking nature of streams, this helper class can substantially 
        /// improve the performance of writing streaming points to the historian.
        /// </summary>
        /// <returns></returns>
        public BulkWriting StartBulkWriting()
        {
            return new BulkWriting(this);
        }


        public override void AttachFilesOrPaths(IEnumerable<string> paths)
        {
            throw new NotImplementedException();
        }

        public override List<ArchiveDetails> GetAllAttachedFiles()
        {
            throw new NotImplementedException();
        }

        public override void DetatchFiles(List<Guid> files)
        {
            throw new NotImplementedException();
        }

        public override void DeleteFiles(List<Guid> files)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets if has been disposed.
        /// </summary>
        public override bool IsDisposed => m_disposed;

        /// <summary>
        /// Gets basic information about the current Database.
        /// </summary>
        public override DatabaseInfo Info => m_info;

        /// <summary>
        /// Forces a soft commit on the database. A soft commit 
        /// only commits data to memory. This allows other clients to read the data.
        /// While soft committed, this data could be lost during an unexpected shutdown.
        /// Soft commits usually occur within microseconds. 
        /// </summary>
        public override void SoftCommit()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Forces a commit to the disk subsystem. Once this returns, the data will not
        /// be lost due to an application crash or unexpected shutdown.
        /// Hard commits can take 100ms or longer depending on how much data has to be committed. 
        /// This requires two consecutive hardware cache flushes.
        /// </summary>
        public override void HardCommit()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;

                if (m_reader != null)
                    m_reader.Dispose();


                m_stream.Write((byte)ServerCommand.DisconnectDatabase);
                m_stream.Flush();
                m_onDispose();

                ServerResponse command = (ServerResponse)m_stream.ReadUInt8();
                switch (command)
                {
                    case ServerResponse.UnhandledException:
                        string exception = m_stream.ReadString();
                        throw new Exception("Server UnhandledExcetion: \n" + exception);
                    case ServerResponse.DatabaseDisconnected:
                        break;
                    default:
                        throw new Exception("Unknown server response: " + command.ToString());
                }
            }
        }

        /// <summary>
        /// Handles bulk writing to a streaming interface.
        /// </summary>
        public class BulkWriting
            : IDisposable
        {
            private bool m_disposed;
            private readonly StreamingClientDatabase<TKey, TValue> m_client;
            private readonly RemoteBinaryStream m_stream;
            private readonly StreamEncodingBase<TKey, TValue> m_encodingMode;

            internal BulkWriting(StreamingClientDatabase<TKey, TValue> client)
            {
                if (client.m_writer != null)
                    throw new Exception("Duplicate call to StartBulkWriting");

                m_client = client;
                m_client.m_writer = this;
                m_stream = m_client.m_stream;
                m_encodingMode = m_client.m_encodingMode;

                m_stream.Write((byte)ServerCommand.Write);
                m_encodingMode.ResetEncoder();
            }

            /// <summary>
            /// Writes to the encoded stream.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void Write(TKey key, TValue value)
            {
                m_encodingMode.Encode(m_stream, key, value);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    m_client.m_writer = null;
                    m_disposed = true;

                    m_encodingMode.WriteEndOfStream(m_stream);
                    m_stream.Flush();
                }
            }
        }

        private class PointReader
            : TreeStream<TKey, TValue>
        {
            private bool m_completed;
            private readonly Action m_onComplete;
            private readonly StreamEncodingBase<TKey, TValue> m_encodingMethod;
            private readonly RemoteBinaryStream m_stream;

            public PointReader(StreamEncodingBase<TKey, TValue> encodingMethod, RemoteBinaryStream stream, Action onComplete)
            {
                m_onComplete = onComplete;
                m_encodingMethod = encodingMethod;
                m_stream = stream;
                encodingMethod.ResetEncoder();
            }

            /// <summary>
            /// Advances the stream to the next value. 
            /// If before the beginning of the stream, advances to the first value
            /// </summary>
            /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
            protected override bool ReadNext(TKey key, TValue value)
            {
                if (!m_completed && m_encodingMethod.TryDecode(m_stream, key, value))
                {
                    return true;
                }
                else
                {
                    Complete();
                    return false;
                }
            }

            public void Cancel()
            {
                //ToDo: Actually cancel the stream.
                TKey key = new TKey();
                TValue value = new TValue();
                if (m_completed)
                    return;
                //flush the rest of the data off of the receive queue.
                while (m_encodingMethod.TryDecode(m_stream, key, value))
                {
                    //CurrentKey.ReadCompressed(m_client.m_stream, CurrentKey);
                    //CurrentValue.ReadCompressed(m_client.m_stream, CurrentValue);
                }
                Complete();
            }

            private void Complete()
            {
                if (!m_completed)
                {
                    m_completed = true;
                    m_onComplete();
                    string exception;
                    ServerResponse command = (ServerResponse)m_stream.ReadUInt8();
                    switch (command)
                    {
                        case ServerResponse.UnhandledException:
                            exception = m_stream.ReadString();
                            throw new Exception("Server UnhandledExcetion: \n" + exception);
                        case ServerResponse.ErrorWhileReading:
                            exception = m_stream.ReadString();
                            throw new Exception("Server Error While Reading: \n" + exception);

                        case ServerResponse.CanceledRead:
                            break;
                        case ServerResponse.ReadComplete:
                            break;
                        default:
                            throw new Exception("Unknown server response: " + command.ToString());
                    }

                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Cancel();
                }
                base.Dispose(disposing);
            }
        }
    }
}