//******************************************************************************************************
//  ProcessClient.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using openHistorian.IO;

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
        IHistorianDataReader m_historianRW;

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
                ProcessRequests();
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
        void ProcessRequests()
        {
            while (true)
            {
                switch ((ServerCommand)m_stream.ReadByte())
                {
                    case ServerCommand.Connect:
                        if (m_historianRW != null)
                        {
                            //m_stream.Write((byte)ServerResponse.Error);
                            //m_stream.Write("Already connected to a database.");
                            //m_netStream.Flush();
                            return;
                        }
                        string databaseName = m_stream.ReadString();
                        m_historianDatabase = m_historian.ConnectToDatabase(databaseName);
                        m_historianRW = m_historianDatabase.OpenDataReader();
                        break;
                    case ServerCommand.Disconnect:
                        if (m_historianRW == null)
                        {
                            //m_stream.Write((byte)ServerResponse.Success);
                            //m_stream.Write("Good Bye");
                            //m_netStream.Flush();
                            return;
                        }
                        else
                        {
                            m_historianRW.Close();
                            m_historianRW = null;
                            //m_stream.Write((byte)ServerResponse.Success);
                            //m_stream.Write("Disconnected from database");
                            //m_netStream.Flush();
                        }
                        break;
                    case ServerCommand.Read:
                        if (m_historianRW == null)
                        {
                            //m_stream.Write((byte)ServerResponse.Error);
                            //m_stream.Write("Not connected to a database");
                            //m_netStream.Flush();
                            return;
                        }
                        ProcessRead();
                        break;
                    case ServerCommand.CancelRead:
                        //m_stream.Write((byte)ServerResponse.Success);
                        //m_stream.Write("Read has been canceled");
                        //m_netStream.Flush();
                        break;
                    case ServerCommand.Write:
                        if (m_historianRW == null)
                        {
                            //m_stream.Write((byte)ServerResponse.Error);
                            //m_stream.Write("Not connected to a database");
                            //m_netStream.Flush();
                            return;
                        }
                        ProcessWrite();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        void ProcessRead()
        {
            ulong startKey1 = m_stream.ReadUInt64();
            ulong endKey1 = m_stream.ReadUInt64();
            int countOfKey2 = m_stream.ReadInt32();

            List<ulong> keys = new List<ulong>(countOfKey2);
            while (countOfKey2 > 0)
            {
                countOfKey2--;
                keys.Add(m_stream.ReadUInt64());
            }
            IPointStream scanner;
            if (countOfKey2 > 0)
            {
                scanner = m_historianRW.Read(startKey1, endKey1, keys);
            }
            else
            {
                if (startKey1 == endKey1)
                    scanner = m_historianRW.Read(startKey1);
                else
                    scanner = m_historianRW.Read(startKey1, endKey1);
            }

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
