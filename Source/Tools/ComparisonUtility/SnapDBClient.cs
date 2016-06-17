using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<ulong> m_pointIDs;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private readonly int m_frameRate;
        private TreeStream<HistorianKey, HistorianValue> m_stream;
        private long m_totalSeeks;
        private bool m_disposed;

        public SnapDBClient(string hostAddress, int port, string instanceName, ulong startTime, ulong endTime, int frameRate, IEnumerable<ulong> pointIDs)
        {
            m_client = new HistorianClient(hostAddress, port);
            m_database = m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName);
            m_key = new HistorianKey();
            m_value = new HistorianValue();
            m_frameRate = frameRate;
            m_pointIDs = pointIDs;
            RestartStream(startTime, endTime);
        }

        ~SnapDBClient()
        {
            Dispose(false);
        }

        public long TotalSeeks => m_totalSeeks;

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

        public bool Resync(ulong startTime, ulong endTime, ulong startPointID, DataPoint point, ref long missingPoints)
        {
            if (DataPoint.CompareTimestamps(m_key.Timestamp, startTime, m_frameRate) > 0)
                RestartStream(startTime, endTime);

            bool success = true;

            // Scan to desired point
            if (m_key.PointID != startPointID || DataPoint.CompareTimestamps(m_key.Timestamp, startTime, m_frameRate) < 0)
            {
                if (!m_stream.Read(m_key, m_value))
                    success = false;

                missingPoints++;
            }

            point.Timestamp = m_key.Timestamp;
            point.PointID = m_key.PointID;
            point.Value = m_value.Value1;
            point.Flags = m_value.Value3;

            return success;
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

        private void RestartStream(ulong startTime, ulong endTime)
        {
            SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(DataPoint.RoundTimestamp(startTime, m_frameRate), DataPoint.RoundTimestamp(endTime, m_frameRate));
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(m_pointIDs);
            m_stream?.Dispose();
            m_stream = m_database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
            m_totalSeeks++;
        }
    }
}