using System;
using System.Collections.Generic;
using GSF;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using openHistorian.Net;
using openHistorian.Snap;

namespace ComparisonUtility
{
    public sealed class SnapDBClient : IDisposable
    {
        private readonly HistorianClient m_client;
        private readonly ClientDatabaseBase<HistorianKey, HistorianValue> m_database;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private readonly IEnumerable<ulong> m_pointIDs;
        private TreeStream<HistorianKey, HistorianValue> m_stream;
        private bool m_disposed;

        public SnapDBClient(string hostAddress, int port, string instanceName, ulong startTime, ulong endTime, IEnumerable<ulong> pointIDs)
        {
            m_client = new HistorianClient(hostAddress, port);
            m_database = m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName);
            m_key = new HistorianKey();
            m_value = new HistorianValue();
            m_pointIDs = pointIDs;
            Resync(startTime, endTime);
        }

        ~SnapDBClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if ((object)m_stream != null)
                            m_stream.Dispose();

                        if ((object)m_database != null)
                            m_database.Dispose();

                        if ((object)m_client != null)
                            m_client.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        public void Resync(ulong startTime, ulong endTime, ulong startPointID = 0, DataPoint point = null)
        {
            SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime);
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(m_pointIDs);

            m_stream?.Dispose();
            m_stream = m_database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

            if (startPointID == 0)
                return;

            // Scan to desired point
            do
            {
                if (!m_stream.Read(m_key, m_value))
                    break;
            }
            while (m_key.PointID != startPointID && m_key.Timestamp / Ticks.PerMillisecond * Ticks.PerMillisecond <= startTime);

            if ((object)point == null)
                return;

            point.Timestamp = m_key.Timestamp;
            point.PointID = m_key.PointID;
            point.Value = m_value.Value1;
            point.Flags = m_value.Value3;
        }

        public bool ReadNext(DataPoint point)
        {
            if ((object)m_stream == null)
                throw new NullReferenceException("Stream is not initialized");

            if (m_stream.Read(m_key, m_value))
            {
                point.Timestamp = m_key.Timestamp;
                point.PointID = m_key.PointID;
                point.Value = m_value.Value1;
                point.Flags = m_value.Value3;

                return true;
            }

            return false;
        }
    }
}