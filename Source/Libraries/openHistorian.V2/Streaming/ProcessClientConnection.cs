using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

namespace openHistorian.V2.Streaming
{
    class ProcessClientConnection
    {
        enum Commands : int
        {
            KeepAlive = 1,
            Disconnect = 2,
            DefinePoints = 3,
            QueryPoints = 4,
            ArchivePoints = 5
        }

        enum DefinePointsCommands : int
        {
            Done = 1,
            ClearList = 2,
            AddPoint = 3,
            RemovePoint = 4
        }
        enum QueryPointsCommands : int
        {
            DefinePoints = 1,
            DefineTimeBoundry = 2,
            Execute = 3,
            Done = 4
        }
        enum ArchivePointsCommands : int
        {
            Archive = 1,
            Done = 2
        }

        TcpClient m_client;
        Thread m_processClientThread;
        SortedList<int, Guid> m_localPoints;

        public ProcessClientConnection(TcpClient client)
        {
            m_localPoints = new SortedList<int, Guid>();
            m_client = client;
            m_processClientThread = new Thread(ProcessClient);
            m_processClientThread.Start();
        }

        void ProcessClient()
        {
            m_client.GetStream();
            NetworkStream stream = m_client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);

            switch ((Commands)reader.ReadInt32())
            {
                case Commands.KeepAlive:
                    break;
                case Commands.Disconnect:
                    stream.Close();
                    m_client.Close();
                    break;
                case Commands.DefinePoints:
                    DefinePoints(reader);
                    break;
                case Commands.QueryPoints:
                    QueryPoints(reader, writer);
                    break;
                case Commands.ArchivePoints:
                    ArchivePoints(reader, writer);
                    break;
            }
        }

        void DefinePoints(BinaryReader reader)
        {
            int referenceNumber;
            Guid pointId;

            while (true)
            {
                switch ((DefinePointsCommands)reader.ReadInt32())
                {
                    case DefinePointsCommands.Done:
                        return;
                    case DefinePointsCommands.ClearList:
                        m_localPoints.Clear();
                        break;
                    case DefinePointsCommands.RemovePoint:
                        referenceNumber = reader.ReadInt32();
                        if (m_localPoints.ContainsKey(referenceNumber))
                        {
                            m_localPoints.Remove(referenceNumber);
                        }
                        break;
                    case DefinePointsCommands.AddPoint:
                        referenceNumber = reader.ReadInt32();
                        pointId = new Guid(reader.ReadBytes(16));
                        if (m_localPoints.ContainsKey(referenceNumber))
                        {
                            m_localPoints[referenceNumber] = pointId;
                        }
                        else
                        {
                            m_localPoints.Add(referenceNumber, pointId);
                        }
                        break;
                }
            }
        }

        void QueryPoints(BinaryReader reader, BinaryWriter writer)
        {
            List<Guid> points = new List<Guid>();
            DateTime startTime = DateTime.MinValue;
            DateTime stopTime = DateTime.MinValue;
            int referenceNumber;

            while (true)
            {
                switch ((QueryPointsCommands)reader.ReadInt32())
                {
                    case QueryPointsCommands.Done:
                        return;
                    case QueryPointsCommands.DefinePoints:
                        int pointCount = reader.ReadInt32();
                        points.Capacity = pointCount;
                        points.Clear();
                        for (int x = 0; x < pointCount; x++)
                        {
                            referenceNumber = reader.ReadInt32();
                            points.Add(m_localPoints[referenceNumber]);
                        }
                        break;
                    case QueryPointsCommands.DefineTimeBoundry:
                        startTime = new DateTime(reader.ReadInt64());
                        stopTime = new DateTime(reader.ReadInt64());
                        break;
                    case QueryPointsCommands.Execute:
                        ExecuteQuery(reader, writer, points, startTime, stopTime);
                        break;
                }
            }
        }

        void ArchivePoints(BinaryReader reader, BinaryWriter writer)
        {

            while (true)
            {
                switch ((ArchivePointsCommands)reader.ReadInt32())
                {
                    case ArchivePointsCommands.Done:
                        return;
                    case ArchivePointsCommands.Archive:
                        int pointCount = reader.ReadInt32();
                        for (int x = 0; x < pointCount; x++)
                        {
                            int referenceNumber = reader.ReadInt32();
                            Guid pointId = m_localPoints[referenceNumber];
                            DateTime time = new DateTime(reader.ReadInt64());
                            int flags = reader.ReadInt32();
                            float value = reader.ReadSingle();
                            ArchivePoint(pointId, time, flags, value);
                        }
                        break;
                }
            }
        }

        void ExecuteQuery(BinaryReader reader, BinaryWriter writer, List<Guid> points, DateTime startTime, DateTime stopTime)
        {

        }

        void ArchivePoint(Guid pointId, DateTime time, int flags, float value)
        {

        }

    }
}
