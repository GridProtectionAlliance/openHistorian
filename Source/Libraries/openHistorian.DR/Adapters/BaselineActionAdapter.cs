//*******************************************************************************************************
//  BaselineActionAdapter.cs - Gbtc
//
//  Tennessee Valley Authority, 2011
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  12/01/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using openHistorian.Adapters;
using openHistorian.Archives;
using TVA;
using TVA.Scheduling;

namespace openHistorian.DR.Adapters
{
    public class BaselineActionAdapter : DataAdapterBase
    {
        #region [ Members ]

        // Nested Types

        // Constants
        public const string DefaultSystemTime = "Central Standard Time";
        public const string DefaultBaselineTag = "";
        public const string DefaultProcessingInterval = "0 0 * * *";

        private const string UniversalTime = "UTC";
        private const string StartTimeFormat = "MM/dd/yyyy {0}:00:00";
        private const string EndTimeFormat = "MM/dd/yyyy {0}:59:59";

        // Fields
        private string m_systemTime;
        private string m_baselineTag;
        private string m_processingInterval;
        private ScheduleManager m_scheduler;
        private IdentifiableItem<string, IDataArchive> m_archive;
        private bool m_initialized;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public BaselineActionAdapter()
        {
            m_systemTime = DefaultSystemTime;
            m_baselineTag = DefaultBaselineTag;
            m_processingInterval = DefaultProcessingInterval;
            m_scheduler = new ScheduleManager();
        }

        #endregion

        #region [ Properties ]

        public string SystemTime 
        {
            get
            {
                return m_systemTime;
            }
            set
            {
                m_systemTime = value;
            }
        }

        public string BaselineTag
        {
            get
            {
                return m_baselineTag;
            }
            set
            {
                m_baselineTag = value;
            }
        }

        public string ProcessingInterval
        {
            get
            {
                return m_processingInterval;
            }
            set
            {
                m_processingInterval = value;
            }
        }

        #endregion

        #region [ Methods ]

        public override void Initialize()
        {
            base.Initialize();
            if (!m_initialized)
            {
                // Find the source archive for data.
                IDictionary<string, IDataArchive> archives = FindSourceArchives(1, 1);
                m_archive = new IdentifiableItem<string, IDataArchive>(archives.First().Key, archives.First().Value);

                m_scheduler.AddSchedule("Calculate.Baseline", m_processingInterval);
                m_scheduler.ScheduleDue += m_scheduler_ScheduleDue;

                m_initialized = true;
            }
        }

        public override void Start()
        {
            if (!m_scheduler.IsRunning)
                m_scheduler.Start();
        }

        public override void Stop()
        {
            if (m_scheduler.IsRunning)
                m_scheduler.Stop();
        }

        protected override void OnArchiveAdded(IDataArchive archive)
        {
            if (string.Compare(m_archive.ID, archive.Name) == 0)
            {
                // Save reference to the added archive.
                m_archive.Item = archive;
                OnStatusUpdate(UpdateType.Information, "Saved reference to \"{0}\"", m_archive.ID);
            }
        }

        protected override void OnArchiveRemoved(IDataArchive archive)
        {
            if (string.Compare(m_archive.ID, archive.Name) == 0)
            {
                // Remove reference of the removed archive.
                m_archive = null;
                OnStatusUpdate(UpdateType.Information, "Removed reference to \"{0}\"", m_archive.ID);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        Stop();

                        if (m_scheduler != null)
                        {
                            m_scheduler.ScheduleDue -= m_scheduler_ScheduleDue;
                            m_scheduler.Dispose();
                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        private void m_scheduler_ScheduleDue(object sender, TVA.EventArgs<Schedule> e)
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime startTime = DateTime.MinValue;
                DateTime endTime = DateTime.MinValue;
                List<decimal> inputs = new List<decimal>();
                foreach (var mapping in Mappings)
                {
                    OnStatusUpdate(UpdateType.Information, "Calculating baseline for {0}", mapping.Source);

                    // For each of the specified source.
                    for (int i = 0; i < 24; i++)
                    {
                        // For every hour of the day.
                        inputs.Clear();
                        for (int j = -3; j < 0; j++)
                        {
                            // For the past three days.
                            startTime = DateTime.Parse(today.AddDays(j).ToString(string.Format(StartTimeFormat, i))).TimeZoneToTimeZone(m_systemTime, UniversalTime);
                            endTime = DateTime.Parse(today.AddDays(j).ToString(string.Format(EndTimeFormat, i))).TimeZoneToTimeZone(m_systemTime, UniversalTime);

                            IData[] data = m_archive.Item.ReadData((int)((DataKey)mapping.Source), startTime.ToString(), endTime.ToString()).ToArray();
                            if (data.Length > 0)
                                inputs.Add(data.Average(dataPoint => (decimal)dataPoint.Value));
                        }

                        if (inputs.Count > 0)
                        {
                            Data baseline = new Data(mapping.Target)
                            {
                                Time = TimeTag.Parse(startTime.AddDays(1).ToString()),
                                Value = (float)inputs.Average(),
                                Quality = Quality.Good
                            };
                            m_archive.Item.WriteData(baseline);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(m_baselineTag))
                {
                    OnStatusUpdate(UpdateType.Information, "Calculating overall baseline");
                    for (int i = 0; i < 24; i++)
                    {
                        // For every hour of the day.
                        inputs.Clear();
                        foreach (var mapping in Mappings)
                        {
                            // For each of the specified target.
                            startTime = DateTime.Parse(today.ToString(string.Format(StartTimeFormat, i))).TimeZoneToTimeZone(m_systemTime, UniversalTime);

                            IData[] data = m_archive.Item.ReadData((int)((DataKey)mapping.Target), startTime.ToString(), startTime.ToString()).ToArray();
                            if (data.Length > 0)
                                inputs.Add((decimal)data[0].Value);
                        }

                        if (inputs.Count > 0)
                        {
                            Data baseline = new Data(m_baselineTag)
                            {
                                Time = TimeTag.Parse(startTime.ToString()),
                                Value = (float)inputs.Sum(),
                                Quality = Quality.Good
                            };
                            m_archive.Item.WriteData(baseline);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnExecutionException("Error calculating baseline", ex);
            }
        }

        #endregion
    }
}
