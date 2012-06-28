//******************************************************************************************************
//  ProcessArchivePoints.cs - Gbtc
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
using System.Collections.Generic;

namespace openHistorian.V2.Streaming.Server
{
    class ProcessArchivePoints : ITransportCommand
    {
        ProcessDatabaseInstance m_databaseInstance;
        SortedList<int, Guid> m_localPoints;
        SortedList<int, long> m_pointMapping;

        enum ArchivePointsCommands : int
        {
            Done = 0,
            Archive = 1
        }

        enum DefinePointsCommands : byte
        {
            Done = 0,
            ClearList = 1,
            RemovePoint = 2,
            AddPoint = 3
        }

        ITransportHost m_host;
        public ProcessArchivePoints(ITransportHost host, ProcessDatabaseInstance databaseInstance)
        {
            m_databaseInstance = databaseInstance;
            m_databaseInstance.InstanceChanged += OnDatabaseInstance_InstanceChanged;
            m_localPoints = new SortedList<int, Guid>();
            m_pointMapping = new SortedList<int, long>();
            m_host = host;
        }

        public byte CommandCode
        {
            get
            {
                return 3;
            }
        }

        bool m_connected;
        public void Execute()
        {
            if (m_databaseInstance.DatabaseEngine == null)
            {
                m_host.Terminate();
                return;
            }
            m_connected = true;
            while (m_connected)
            {
                switch (m_host.Receive.ReadByte())
                {
                    case 0: //exit to root
                        return;
                    case 1:
                        DefinePoints();
                        break;
                    case 2:
                        ArchivePoints();
                        break;
                    default: //unknown command
                        m_host.Terminate();
                        return;
                }
            }
        }

        void DefinePoints()
        {
            int referenceNumber;
            Guid pointId;

            while (true)
            {
                switch ((DefinePointsCommands)m_host.Receive.ReadByte())
                {
                    case DefinePointsCommands.Done:
                        return;
                    case DefinePointsCommands.ClearList:
                        m_localPoints.Clear();
                        m_pointMapping.Clear();
                        break;
                    case DefinePointsCommands.RemovePoint:
                        referenceNumber = (int)m_host.Receive.Read7BitUInt32();
                        if (m_localPoints.ContainsKey(referenceNumber))
                        {
                            m_localPoints.Remove(referenceNumber);
                            m_pointMapping.Remove(referenceNumber);
                        }
                        break;
                    case DefinePointsCommands.AddPoint:
                        referenceNumber = (int)m_host.Receive.Read7BitUInt32();
                        pointId = m_host.Receive.ReadGuid();
                        if (m_localPoints.ContainsKey(referenceNumber))
                        {
                            m_localPoints[referenceNumber] = pointId;
                            //m_pointMapping[referenceNumber] = m_databaseInstance.DatabaseEngine.LookupPointId(pointId);
                        }
                        else
                        {
                            m_localPoints.Add(referenceNumber, pointId);
                            //m_pointMapping.Add(referenceNumber, m_databaseInstance.DatabaseEngine.LookupPointId(pointId));
                        }
                        break;
                    default:
                        m_connected = false;
                        m_host.Terminate();
                        return;
                }
            }
        }
        void ArchivePoints()
        {
            while (true)
            {
                switch ((ArchivePointsCommands)m_host.Receive.ReadByte())
                {
                    case ArchivePointsCommands.Done:
                        return;
                    case ArchivePointsCommands.Archive:
                        int referenceNumber = m_host.Receive.ReadInt32();
                        long pointId = m_pointMapping[referenceNumber];
                        long time = m_host.Receive.ReadInt64();
                        long value1 = m_host.Receive.ReadInt64();
                        long value2 = m_host.Receive.ReadInt64();
                        if (pointId>=0)
                            ;//m_databaseInstance.DatabaseEngine.WriteData(time, pointId, value1, value2);
                        break;
                    default:
                        m_connected = false;
                        m_host.Terminate();
                        return;
                }
            }
        }

        void OnDatabaseInstance_InstanceChanged()
        {
            for (int x = 0; x < m_localPoints.Count; x++)
            {
                //m_pointMapping.Values[x] = m_databaseInstance.DatabaseEngine.LookupPointId(m_localPoints.Values[x]);
            }
        }


    }
}
