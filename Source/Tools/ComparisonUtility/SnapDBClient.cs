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
        private readonly TreeStream<HistorianKey, HistorianValue> m_stream;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private bool m_disposed;

        public SnapDBClient(string hostAddress, int port, string instanceName, ulong startTime, ulong endTime, int frameRate, IEnumerable<ulong> pointIDs)
        {
            m_client = new HistorianClient(hostAddress, port);
            m_database = m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName);
            m_key = new HistorianKey();
            m_value = new HistorianValue();

            SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(DataPoint.RoundTimestamp(startTime, frameRate), DataPoint.RoundTimestamp(endTime, frameRate));
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(pointIDs);

            m_stream = m_database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
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
                        if (m_stream != null)
                            m_stream.Dispose();

                        if (m_database != null)
                            m_database.Dispose();

                        if (m_client != null)
                            m_client.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        public bool ReadNext(DataPoint point)
        {
            if (m_stream is null)
                throw new NullReferenceException("Stream is not initialized");

            if (!m_stream.Read(m_key, m_value))
                return false;

            point.Timestamp = m_key.Timestamp;
            point.PointID = m_key.PointID;
            point.Value = m_value.Value1;
            point.Flags = m_value.Value3;

            return true;
        }
    }
}