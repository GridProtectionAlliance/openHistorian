//******************************************************************************************************
//  SortedTreeClient_Engine`2.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Net;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Engine.Reader;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Net
{
    public partial class SortedTreeClient
    {
        private class SortedTreeClientEngine<TKey, TValue>
            : SortedTreeEngineBase<TKey, TValue>
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {

            TKey m_tmpKey;
            TValue m_tmpValue;
            private PointReader m_reader;
            private bool m_disposed;
            private readonly SortedTreeClient m_client;
            NetworkBinaryStream m_stream;
            private readonly Action m_onDispose;
            StreamEncodingBase<TKey, TValue> m_encodingMode;

            public SortedTreeClientEngine(SortedTreeClient client, Action onDispose)
            {
                m_tmpKey = new TKey();
                m_tmpValue = new TValue();
                m_onDispose = onDispose;
                m_client = client;
                m_stream = client.m_stream;
            }

            public void SetEncodingMode(EncodingDefinition encoding)
            {
                m_encodingMode = StreamEncoding.CreateStreamEncoding<TKey, TValue>(encoding);
                m_stream.Write((byte)ServerCommand.SetEncodingMethod);
                encoding.Save(m_stream);
                m_stream.Flush();

                var command = (ServerResponse)m_stream.ReadUInt8();
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

            public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
            {
                if (m_reader != null)
                    throw new Exception("Sockets do not support concurrent readers. Dispose of old reader.");

                m_stream.Write((byte)ServerCommand.Read);
                if (keySeekFilter == null)
                {
                    m_stream.Write(false);
                }
                else
                {
                    m_stream.Write(true);
                    m_stream.Write(keySeekFilter.FilterType);
                    keySeekFilter.Save(m_stream);
                }

                if (keyMatchFilter == null)
                {
                    m_stream.Write(false);
                }
                else
                {
                    m_stream.Write(true);
                    m_stream.Write(keyMatchFilter.FilterType);
                    keyMatchFilter.Save(m_stream);
                }

                if (readerOptions == null)
                {
                    m_stream.Write(false);
                }
                else
                {
                    m_stream.Write(true);
                    readerOptions.Save(m_stream);
                }
                m_stream.Flush();


                var command = (ServerResponse)m_stream.ReadUInt8();
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
                    default:
                        throw new Exception("Unknown server response: " + command.ToString());
                }

                m_reader = new PointReader(m_encodingMode, m_stream, () => m_reader = null);
                return m_reader;
            }


            public override void Write(TreeStream<TKey, TValue> points)
            {
                if (m_reader != null)
                    throw new Exception("Sockets do not support writing while a reader is open. Dispose of reader.");

                m_stream.Write((byte)ServerCommand.Write);
                m_encodingMode.ResetEncoder();
                while (points.Read(m_tmpKey, m_tmpValue))
                {
                    m_encodingMode.Encode(m_stream, m_tmpKey, m_tmpValue);
                }
                m_encodingMode.WriteEndOfStream(m_stream);
                m_stream.Flush();
            }

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

            public override DatabaseInfo Info
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void SoftCommit()
            {
                //throw new NotImplementedException();
            }

            public override void HardCommit()
            {
                //throw new NotImplementedException();
            }

            public override void Disconnect()
            {
                Dispose();
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

                    var command = (ServerResponse)m_stream.ReadUInt8();
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

            private class PointReader
                : TreeStream<TKey, TValue>
            {
                private bool m_completed;
                private readonly Action m_onComplete;
                StreamEncodingBase<TKey, TValue> m_encodingMethod;
                private NetworkBinaryStream m_stream;

                public PointReader(StreamEncodingBase<TKey, TValue> encodingMethod, NetworkBinaryStream stream, Action onComplete)
                {
                    m_onComplete = onComplete;
                    m_encodingMethod = encodingMethod;
                    m_stream = stream;
                    encodingMethod.ResetEncoder();
                }

                public override bool Read(TKey key, TValue value)
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

                public override void Cancel()
                {
                    TKey key = new TKey();
                    TValue value = new TValue();
                    if (m_completed)
                        return;
                    m_stream.Write((byte)ServerCommand.CancelRead);
                    m_stream.Flush();
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
                        var command = (ServerResponse)m_stream.ReadUInt8();
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
}