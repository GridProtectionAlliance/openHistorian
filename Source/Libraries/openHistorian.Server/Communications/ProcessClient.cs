//******************************************************************************************************
//  ProcessClient.cs - Gbtc
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
using System.Collections.Generic;
using System.Diagnostics;
using GSF.Communications;
using GSF.IO;

namespace openHistorian.Communications
{
    internal class ProcessClient : IDisposable
    {
        public event SocketErrorEventHandler SocketError;

        public delegate void SocketErrorEventHandler(Exception ex);

        NetworkBinaryStream m_netStream;
        BinaryStreamWrapper m_stream;
        IHistorianDatabaseCollection m_historian;
        IHistorianDatabase m_historianDatabase;
        IHistorianDataReader m_historianReader;

        public ProcessClient(NetworkBinaryStream netStream, IHistorianDatabaseCollection historian)
        {
            m_netStream = netStream;
            m_stream = new BinaryStreamWrapper(m_netStream);
            m_historian = historian;
        }

        /// <summary>
        /// This function will verify the connection, create all necessary streams, set timeouts, and catch any exceptions and terminate the connection
        /// </summary>
        /// <remarks></remarks>

        public void Run()
        {
            m_netStream.Timeout = 5000;

            try
            {
                long code = m_stream.ReadInt64();
                if (code != 1122334455667788990L)
                {
                    //m_stream.Write((byte)ServerResponse.Error);
                    //m_stream.Write("Wrong Username Or Password");
                    //m_netStream.Flush();
                    return;
                }
                ProcessRequestsLevel1();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (SocketError != null)
                {
                    SocketError(ex);
                }
            }
            finally
            {
                try
                {
                    m_netStream.Disconnect();
                    m_netStream.Close();
                }
                catch (Exception ex)
                {
                }
                m_netStream = null;
            }
        }

        /// <summary>
        /// This function will process any of the packets that come in.  It will throw an error if anything happens.  
        /// This will cause the calling function to close the connection.
        /// </summary>
        /// <remarks></remarks>
        void ProcessRequestsLevel1()
        {
            while (true)
            {
                ServerCommand command = (ServerCommand)m_stream.ReadByte();
                switch (command)
                {
                    case ServerCommand.ConnectToDatabase:
                        if (m_historianDatabase != null)
                        {
                            //m_stream.Write((byte)ServerResponse.Error);
                            //m_stream.Write("Already connected to a database.");
                            //m_netStream.Flush();
                            return;
                        }
                        string databaseName = m_stream.ReadString();
                        m_historianDatabase = m_historian.ConnectToDatabase(databaseName);
                        ProcessRequestsLevel2();
                        break;
                    case ServerCommand.Disconnect:
                        return;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <remarks></remarks>
        void ProcessRequestsLevel2()
        {
            while (true)
            {
                switch ((ServerCommand)m_stream.ReadByte())
                {
                    case ServerCommand.OpenReader:
                        m_historianReader = m_historianDatabase.OpenDataReader();
                        ProcessRequestsLevel3();
                        break;
                    case ServerCommand.DisconnectDatabase:
                        m_historianDatabase.Disconnect();
                        m_historianDatabase = null;
                        return;
                        break;
                    case ServerCommand.Write:
                        ProcessWrite();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        void ProcessRequestsLevel3()
        {
            while (true)
            {
                switch ((ServerCommand)m_stream.ReadByte())
                {
                    case ServerCommand.DisconnectReader:
                        m_historianReader.Close();
                        m_historianReader = null;
                        return;
                        break;
                    case ServerCommand.Read:
                        ProcessRead();
                        break;
                    case ServerCommand.CancelRead:
                        //m_stream.Write((byte)ServerResponse.Success);
                        //m_stream.Write("Read has been canceled");
                        //m_netStream.Flush();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        void ProcessRead()
        {
            var key1Parser = KeyParserPrimary.CreateFromStream(m_stream);
            var key2Parser = KeyParserSecondary.CreateFromStream(m_stream);
            var readerOptions = new DataReaderOptions(m_stream);

            IPointStream scanner = m_historianReader.Read(key1Parser, key2Parser, readerOptions);

            ulong oldKey1 = 0, oldKey2 = 0, oldValue1 = 0, oldValue2 = 0;
            ulong key1, key2, value1, value2;
            while (scanner.Read(out key1, out key2, out value1, out value2))
            {
                m_stream.Write(true);
                m_stream.Write7Bit(oldKey1 ^ key1);
                m_stream.Write7Bit(oldKey2 ^ key2);
                m_stream.Write7Bit(oldValue1 ^ value1);
                m_stream.Write7Bit(oldValue2 ^ value2);

                if (m_netStream.AvailableReadBytes > 0)
                {
                    m_stream.Write(false);
                    m_netStream.Flush();
                    return;
                }

                oldKey1 = key1;
                oldKey2 = key2;
                oldValue1 = value1;
                oldValue2 = value2;
            }
            m_stream.Write(false);
            m_netStream.Flush();
        }

        void ProcessWrite()
        {
            ulong key1 = 0, key2 = 0, value1 = 0, value2 = 0;
            while (m_stream.ReadBoolean())
            {
                key1 = m_stream.Read7BitUInt64() ^ key1;
                key2 = m_stream.Read7BitUInt64() ^ key2;
                value1 = m_stream.Read7BitUInt64() ^ value1;
                value2 = m_stream.Read7BitUInt64() ^ value2;
                m_historianDatabase.Write(key1, key2, value1, value2);
            }
        }

        public void Dispose()
        {
            if (m_netStream.Connected)
                m_netStream.Disconnect();
        }
    }
}
