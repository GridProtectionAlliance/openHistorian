using System;
using System.Collections.Generic;
using openHistorian.V2.Collections;
using openHistorian.V2.Collections.BPlusTreeTypes;
using openHistorian.V2.Collections.Specialized;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Unmanaged;

namespace openHistorian.V2
{
    /// <summary>
    /// Provides a very basic implementation of IArchive for testing purposes.
    /// </summary>
    public class ArchiveEngineShell : IArchive
    {
        MemoryStream m_stream;
        BinaryStream m_binaryStream;
        BPlusTreeTSD m_tree;

        public ArchiveEngineShell()
        {
            m_stream = new MemoryStream();
            m_binaryStream = new BinaryStream(m_stream);
            m_tree = new BPlusTreeTSD(m_binaryStream, 4096);
        }

        public void Open()
        {
            m_stream = new MemoryStream();
            m_binaryStream = new BinaryStream(m_stream);
            m_tree = new BPlusTreeTSD(m_binaryStream, 4096);
            //throw new NotImplementedException();
        }

        public void Close()
        {
            m_tree = null;
            m_binaryStream = null;
            m_stream = null;
        }

        public void WriteData(IDataPoint dataPoint)
        {
            DateTimeLong key = default(DateTimeLong);
            key.Time = dataPoint.Time;
            key.Key = dataPoint.HistorianID;

            IntegerFloat value = new IntegerFloat((int)dataPoint.Flags, dataPoint.Value);

            m_tree.Add(key, value);
        }

        public IEnumerable<IDataPoint> ReadData(int historianID, string startTime, string endTime)
        {
            DateTimeLong start = default(DateTimeLong);
            DateTimeLong end = default(DateTimeLong);

            start.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(startTime);
            start.Key = 0;

            end.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(endTime).AddTicks(1);
            end.Key = 0;

            foreach (var kvp in m_tree.GetRange(start, end))
            {
                DateTimeLong key = kvp.Key;
                IntegerFloat value = kvp.Value;
                if (key.Key == historianID)
                    yield return new DataPoint(key, value);
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

            DateTimeLong start = default(DateTimeLong);
            DateTimeLong end = default(DateTimeLong);
            start.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(startTime);
            start.Key = 0;

            end.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(endTime).AddTicks(1);
            end.Key = 0;

            foreach (var kvp in m_tree.GetRange(start, end))
            {
                DateTimeLong key = kvp.Key;
                IntegerFloat value = kvp.Value;
                if (key.Key <= maxID && items.GetBit((int)key.Key))
                    yield return new DataPoint(key, value);
            }
        }
    }
}
