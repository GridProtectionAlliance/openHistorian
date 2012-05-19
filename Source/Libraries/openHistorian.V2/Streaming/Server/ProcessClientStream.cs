//******************************************************************************************************
//  ProcessClientStream.cs - Gbtc
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
//  5/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using openHistorian.V2.Service;

namespace openHistorian.V2.Streaming.Server
{
    class ProcessClientStream : ITransportHost
    {
        static Guid s_applicationId = new Guid("{211500A0-A136-11E1-B4DC-C8C96088709B}");
        ITransportStreaming m_clientStream;
        IOutboundBuffer m_send;
        IInboundBuffer m_receive;
        bool m_connected;
        Thread m_processClientThread;
        ITransportCommand[] m_routingTable;
        HistorianEngine m_engine;

        public ProcessClientStream(ITransportStreaming clientStream, HistorianEngine engine)
        {
            m_engine = engine;
            m_connected = true;
            m_routingTable = new ITransportCommand[256];

            m_clientStream = clientStream;
            m_send = clientStream.SendStream;
            m_receive = clientStream.ReceiveStream;

            CommandAdd(new ProcessDisconnect(this));
            CommandAdd(new ProcessKeepAlive(this));
            CommandAdd(new ProcessAuthentication(this,s_applicationId));

            m_processClientThread = new Thread(ProcessClient);
            m_processClientThread.Start();
        }

        public IOutboundBuffer Send
        {
            get
            {
                return m_send;
            }
        }

        public IInboundBuffer Receive
        {
            get
            {
                return m_receive;
            }
        }
        public HistorianEngine Engine
        {
            get
            {
                return m_engine;
            }
        }
        
        public void CommandAdd(ITransportCommand command)
        {
            m_routingTable[command.CommandCode] = command;
        }
        public void CommandRemove(byte commandCode)
        {
            m_routingTable[commandCode] = null;
        }
        public bool CommandExists(byte commandCode)
        {
            return m_routingTable[commandCode] != null;
        }

        public void ExitLevel()
        {
            m_connected = false;
            m_clientStream.Disconnect();
        }

        public void Terminate()
        {
            m_connected = false;
            m_clientStream.Disconnect();
        }

        void ProcessClient()
        {
            while (m_connected)
            {
                byte nextCommand = m_receive.ReadByte();
                ITransportCommand command = m_routingTable[nextCommand];
                if (command != null)
                {
                    command.Execute();
                }
                else
                {
                    Terminate();
                }
            }
            m_clientStream.Disconnect();
        }

        //enum Commands : int
        //{
        //    Disconnect = 0,
        //    KeepAlive = 1,
        //    Authenticate = 3,
        //    ArchivePoints = 4,
        //    QueryPoints = 5
        //}
        
        //enum QueryPointsCommands : int
        //{
        //    DefinePoints = 1,
        //    DefineTimeBoundry = 2,
        //    Execute = 3,
        //    Done = 4
        //}
        
        //void QueryPoints()
        //{
        //    List<Guid> points = new List<Guid>();
        //    DateTime startTime = DateTime.MinValue;
        //    DateTime stopTime = DateTime.MinValue;
        //    int referenceNumber;

        //    while (true)
        //    {
        //        switch ((QueryPointsCommands)reader.ReadInt32())
        //        {
        //            case QueryPointsCommands.Done:
        //                return;
        //            case QueryPointsCommands.DefinePoints:
        //                int pointCount = reader.ReadInt32();
        //                points.Capacity = pointCount;
        //                points.Clear();
        //                for (int x = 0; x < pointCount; x++)
        //                {
        //                    referenceNumber = reader.ReadInt32();
        //                    points.Add(m_localPoints[referenceNumber]);
        //                }
        //                break;
        //            case QueryPointsCommands.DefineTimeBoundry:
        //                startTime = new DateTime(reader.ReadInt64());
        //                stopTime = new DateTime(reader.ReadInt64());
        //                break;
        //            case QueryPointsCommands.Execute:
        //                ExecuteQuery(reader, writer, points, startTime, stopTime);
        //                break;
        //        }
        //    }
        //}



        //void ExecuteQuery(BinaryReader reader, BinaryWriter writer, List<Guid> points, DateTime startTime, DateTime stopTime)
        //{

        //}



    }
}
