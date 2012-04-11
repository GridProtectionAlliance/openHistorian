using System;
using System.Collections.Generic;
using System.Threading;
using openHistorian.V2.Unmanaged;
using openHistorian.V2.Unmanaged.Generic;
using openHistorian.V2.Unmanaged.Generic.TimeKeyPair;
namespace openHistorian.V2
{
    public class ArchiveEngine : IArchive
    {
        IncommingQueue m_newPointQueue;
        ArchiveLookupClass m_lookupClass;
        Thread m_insertThread;
        Thread m_rolloverThread;
        Archive m_currentArchive;

        public ArchiveEngine()
        {

        }

        public void Open()
        {
            m_newPointQueue = new IncommingQueue();
            m_lookupClass = new ArchiveLookupClass();
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
            m_rolloverThread = new Thread(ProcessRolloverData);
            m_rolloverThread.Start();
        }

        public void Close()
        {

        }

        void ProcessRolloverData()
        {
            while (true)
            {
                Thread.Sleep(10000);

                var oldFiles = m_lookupClass.GetLatestSnapshot().Clone();
                //determine what files need to be recombined.
                //recombine them
                //update the snapshot library
                //post update.
            }
        }

        /// <summary>
        /// This process fires 10 times per second and populates the 
        /// </summary>
        void ProcessInsertingData()
        {
            while (true)
            {
                Thread.Sleep(100);
                BinaryStream stream;
                int pointCount;
                m_newPointQueue.GetPointBlock(out stream, out pointCount);

                if (pointCount > 0)
                {
                    while (pointCount > 0)
                    {
                        pointCount--;

                        long time = stream.ReadInt64();
                        int id = stream.ReadInt32();
                        int flags = stream.ReadInt32();
                        float value = stream.ReadSingle();

                        m_currentArchive.AddPoint(new DateTime(time), id, flags, value);
                    }
                }
            }
        }

        public void WriteData(IDataPoint dataPoint)
        {
            m_newPointQueue.WriteData(dataPoint);
        }

        public IEnumerable<IDataPoint> ReadData(int historianID, string startTime, string endTime)
        {
            KeyType start = default(KeyType);
            KeyType end = default(KeyType);

            start.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(startTime);
            start.Key = 0;

            end.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(endTime).AddTicks(1);
            end.Key = 0;

            var lookup =  m_lookupClass.GetLatestSnapshot();

            foreach (var c in lookup.ArchiveTables)
            {
                if (c.Contains(start.Time,end.Time))
                {

                    foreach (var data in c.ArchiveFile.GetData(start.Time,end.Time))
                    {
                        if (data.Item2 == historianID)
                        {
                            yield return new DataPoint(default(KeyType), default(TreeTypeIntFloat));
                        }
                    }
                }
                
            }
        }

        public IEnumerable<IDataPoint> ReadData(IEnumerable<int> historianIDs, string startTime, string endTime)
        {
            int maxID = 0;
            foreach (int historianID in historianIDs)
            {
                maxID = Math.Max(maxID, historianID);
            }

            BitArray items = new BitArray(maxID + 1, false);

            foreach (int historianID in historianIDs)
            {
                items.SetBit(historianID);
            }

            KeyType start = default(KeyType);
            KeyType end = default(KeyType);

            start.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(startTime);
            start.Key = 0;

            end.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(endTime).AddTicks(1);
            end.Key = 0;

            var lookup = m_lookupClass.GetLatestSnapshot();

            foreach (var c in lookup.ArchiveTables)
            {
                if (c.Contains(start.Time, end.Time))
                {
                    foreach (var data in c.ArchiveFile.GetData(start.Time, end.Time))
                    {
                        if (data.Item2 <= maxID && items.GetBit((int)data.Item2))
                        {
                            yield return new DataPoint(default(KeyType), default(TreeTypeIntFloat));
                        }
                    }
                }
            }
        }
    }
}
