using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.Unmanaged;
using openHistorian.V2.Unmanaged.Generic;
using openHistorian.V2.Unmanaged.Generic.TimeKeyPair;

namespace openHistorian.V2
{
    public class ArchiveEngine : IArchive
    {
        MemoryStream m_stream;
        BinaryStream m_binaryStream;
        BPlusTreeTSD m_tree;

        public ArchiveEngine()
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
            KeyType key = default(KeyType);
            key.Time = dataPoint.Time;
            key.Key = dataPoint.HistorianID;

            TreeTypeIntFloat value = new TreeTypeIntFloat((int)dataPoint.Flags, dataPoint.Value);

            m_tree.AddData(key, value);
        }

        public IEnumerable<IDataPoint> ReadData(int historianID, string startTime, string endTime)
        {
            KeyType start = default(KeyType);
            KeyType end = default(KeyType);

            start.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(startTime);
            start.Key = 0;

            end.Time = TimeSeriesFramework.Adapters.AdapterBase.ParseTimeTag(endTime).AddTicks(1);
            end.Key = 0;

            var reader = m_tree.ExecuteScan(start, end);
            while (reader.Next())
            {
                KeyType key = reader.GetKey();
                if (key.Key == historianID)
                {
                    yield return new DataPoint(key, reader.GetValue());
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

            var reader = m_tree.ExecuteScan(start, end);
            while (reader.Next())
            {
                KeyType key = reader.GetKey();
                if (key.Key <=maxID && items.GetBit((int)key.Key))
                {
                    yield return new DataPoint(key, reader.GetValue());
                }
            }
        }
    }
}
