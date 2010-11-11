//******************************************************************************************************
//  ExporterBase.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  -----------------------------------------------------------------------------------------------------
//  06/13/2007 - Pinal C. Patel
//       Original version of source code generated.
//  02/13/2008 - Pinal C. Patel
//       Modified DataSetTemplate() method to handle null strings for input and output tables.
//       Changed the type for dataset columns ID to Integer, Value to Single and Quality to Integer.
//  02/25/2008 - Pinal C. Patel
//       Modified the DataSetTemplate() method to remove table 3 as it had little use.
//  06/05/2008 - Pinal C. Patel
//       Made use of new properties LastProcessResult and LastProcessError added to Export class.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/16/2009 - Pinal C. Patel
//       Modified to use new DataListener.DataExtracted event instead of using 
//       DataListener.Parser.DataParsed event and manually extracting time-series data points.
//       Exposed the process queue used for processing real-time exports as a protected member.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated new header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using TVA;
using TVA.Collections;
using TimeSeriesArchiver.Files;

namespace TimeSeriesArchiver.Exporters
{
    /// <summary>
    /// Base class for an exporter of real-time time-series data.
    /// </summary>
    /// <seealso cref="Export"/>
    /// <seealso cref="DataListener"/>
    public abstract class ExporterBase : IExporter
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// A class that can be used to save real-time time-series data for <see cref="Export"/>s of type <see cref="ExportType.RealTime"/>.
        /// </summary>
        /// <seealso cref="Export"/>
        protected class RealTimeData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RealTimeData"/> class.
            /// </summary>
            /// <param name="export">The <see cref="Export"/> to which the <paramref name="data"/> belongs.</param>
            /// <param name="listener">The <see cref="DataListener"/> that provided the <paramref name="data"/>.</param>
            /// <param name="data">The real-time time-series data received by the <paramref name="listener"/>.</param>
            public RealTimeData(Export export, DataListener listener, IList<IDataPoint> data)
            {
                this.Export = export;
                this.Listener = listener;
                this.Data = data;
            }

            /// <summary>
            /// Gets or sets the <see cref="Export"/> to which the <see cref="Data"/> belongs.
            /// </summary>
            public Export Export;

            /// <summary>
            /// Gets or sets the <see cref="DataListener"/> that provided the <see cref="Data"/>.
            /// </summary>
            public DataListener Listener;

            /// <summary>
            /// Gets or sets the real-time time-series data received by the <see cref="Listener"/>.
            /// </summary>
            public IList<IDataPoint> Data;
        }

        // Constants

        /// <summary>
        /// Number of seconds to wait to obtain a write lock on a file.
        /// </summary>
        public const int FileLockWaitTime = 3;

        /// <summary>
        /// Maximum number of request that could be queued in the real-time and non real-time queues.
        /// </summary>
        private const int MaximumQueuedRequest = 1000;

        // Events

        /// <summary>
        /// Occurs when the exporter want to provide a status update.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the status update message.
        /// </remarks>
        public event EventHandler<EventArgs<string>> StatusUpdate;

        /// <summary>
        /// Occurs when the exporter finishes processing an <see cref="Export"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="Export"/> that the exporter finished processing.
        /// </remarks>
        public event EventHandler<EventArgs<Export>> ExportProcessed;

        /// <summary>
        /// Occurs when the exporter fails to process an <see cref="Export"/> due to an <see cref="Exception"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="Export"/> that the exporter failed to process.
        /// </remarks>
        public event EventHandler<EventArgs<Export>> ExportProcessException;

        // Fields
        private string m_name;
        private ObservableCollection<Export> m_exports;
        private ObservableCollection<DataListener> m_listeners;
        private Action<Export> m_exportAddedHandler;
        private Action<Export> m_exportRemovedHandler;
        private Action<Export> m_exportUpdatedHandler;
        private Timer m_exportTimer;
        private ProcessQueue<RealTimeData> m_realTimeExportQueue;
        private ProcessQueue<Export> m_nonRealTimeExportQueue;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the exporter.
        /// </summary>
        /// <param name="name">Name of the exporter.</param>
        protected ExporterBase(string name)
        {
            m_name = name;
            m_exports = new ObservableCollection<Export>();
            m_exports.CollectionChanged += Exports_CollectionChanged;
            m_listeners = new ObservableCollection<DataListener>();
            m_listeners.CollectionChanged += Listeners_CollectionChanged;
            m_exportTimer = new Timer(1000);
            m_exportTimer.Elapsed += ExportTimer_Elapsed;
            m_realTimeExportQueue = ProcessQueue<RealTimeData>.CreateRealTimeQueue(ProcessRealTimeExports);
            m_nonRealTimeExportQueue = ProcessQueue<Export>.CreateRealTimeQueue(ProcessExport);

            m_exportTimer.Start();
            m_realTimeExportQueue.Start();
            m_nonRealTimeExportQueue.Start();
        }

        /// <summary>
        /// Releases the unmanaged resources before the exporter is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ExporterBase()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the exporter.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_name = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Export"/>s associated with the exporter.
        /// </summary>
        /// <remarks>
        /// WARNING: <see cref="Exports"/> is therad unsafe. Synchronized access is required.
        /// </remarks>
        public IList<Export> Exports
        {
            get
            {
                return m_exports;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataListener"/>s providing real-time time-series data to the exporter.
        /// </summary>
        /// <remarks>
        /// WARNING: <see cref="Listeners"/> is therad unsafe. Synchronized access is required.
        /// </remarks>
        public IList<DataListener> Listeners
        {
            get
            {
                return m_listeners;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> to be invoked when a new <see cref="Export"/> is added to the <see cref="Exports"/>.
        /// </summary>
        protected Action<Export> ExportAddedHandler
        {
            get
            {
                return m_exportAddedHandler;
            }
            set
            {
                m_exportAddedHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> to be invoked when an existing <see cref="Export"/> is removed from the <see cref="Exports"/>.
        /// </summary>
        protected Action<Export> ExportRemovedHandler
        {
            get
            {
                return m_exportRemovedHandler;
            }
            set
            {
                m_exportRemovedHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> to be invoked when an existing <see cref="Export"/> from the <see cref="Exports"/> is updated.
        /// </summary>
        protected Action<Export> ExportUpdatedHandler
        {
            get
            {
                return m_exportUpdatedHandler;
            }
            set
            {
                m_exportUpdatedHandler = value;
            }
        }

        /// <summary>
        /// Gets the internal <see cref="ProcessQueue{T}"/> used for processing <see cref="Exports"/> defined as <see cref="ExportType.RealTime"/>.
        /// </summary>
        protected ProcessQueue<RealTimeData> RealTimeExportQueue 
        {
            get
            {
                return m_realTimeExportQueue;
            }
        }

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        /// <summary>
        /// When overridden in a derived class, processes the <paramref name="export"/> using the current <see cref="DataListener.Data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        protected abstract void ProcessExport(Export export);

        /// <summary>
        /// When overridden in a derived class, processes the <paramref name="export"/> using the real-time <paramref name="data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        /// <param name="listener"><see cref="DataListener"/> that provided the <paramref name="data"/>.</param>
        /// <param name="data">Real-time time-series data received by the <paramref name="listener"/>.</param>
        protected abstract void ProcessRealTimeExport(Export export, DataListener listener, IList<IDataPoint> data);

        #endregion

        /// <summary>
        /// Releases all the resources used by the exporter.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Processes <see cref="Export"/> with the specified <paramref name="exportName"/>.
        /// </summary>
        /// <param name="exportName"><see cref="Export.Name"/> of the <see cref="Export"/> to be processed.</param>
        /// <exception cref="InvalidOperationException"><see cref="Export"/> does not exist for the specified <paramref name="exportName"/>.</exception>
        public void ProcessExport(string exportName)
        {
            Export export = FindExport(exportName);
            if (export != null)
                // Queue the export for processing regardless of its type.
                m_nonRealTimeExportQueue.Add(export);
            else
                throw new InvalidOperationException(string.Format("Export \"{0}\" does not exist", exportName));
        }

        /// <summary>
        /// Returns the <see cref="Export"/> for the specified <paramref name="exportName"/> from the <see cref="Exports"/>.
        /// </summary>
        /// <param name="exportName"><see cref="Export.Name"/> of the <see cref="Export"/> to be retrieved.</param>
        /// <returns>An <see cref="Export"/> object if a match is found; otherwise null.</returns>
        public Export FindExport(string exportName)
        {
            lock (m_exports)
            {
                return m_exports.FirstOrDefault(export => (string.Compare(export.Name, exportName, true) == 0));
            }
        }

        /// <summary>
        /// Returns the <see cref="DataListener"/> for the specified <paramref name="listenerName"/> from the <see cref="Listeners"/>.
        /// </summary>
        /// <param name="listenerName"><see cref="DataListener.Name"/> of the <see cref="DataListener"/> to be retrieved.</param>
        /// <returns>A <see cref="DataListener"/> object if a match is found; otherwise null.</returns>
        public DataListener FindListener(string listenerName)
        {
            lock (m_listeners)
            {
                return m_listeners.FirstOrDefault(listener => (string.Compare(listener.Name, listenerName, true) == 0));
            }
        }

        /// <summary>
        /// Determines whether the current exporter object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current exporter object is to be compared for equality.</param>
        /// <returns>true if the current exporter object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            ExporterBase other = obj as ExporterBase;
            if (other == null)
                return false;
            else
                return string.Compare(m_name, other.Name, true) == 0;
        }

        /// <summary>
        /// Returns the hash code for the current exporter object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the exporter and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        if (m_realTimeExportQueue != null)
                            m_realTimeExportQueue.Dispose();

                        if (m_nonRealTimeExportQueue != null)
                            m_nonRealTimeExportQueue.Dispose();

                        if (m_exportTimer != null)
                        {
                            m_exportTimer.Elapsed -= ExportTimer_Elapsed;
                            m_exportTimer.Dispose();
                        }

                        // Remove all associated exports.
                        if (m_exports != null)
                        {
                            lock (m_exports)
                            {
                                while (m_exports.GetEnumerator().MoveNext())
                                {
                                    m_exports.RemoveAt(0);
                                }
                            }
                            m_exports.CollectionChanged -= Exports_CollectionChanged;
                        }

                        // Remove all associated listeners.
                        if (m_listeners != null)
                        {
                            lock (m_listeners)
                            {
                                while (m_listeners.GetEnumerator().MoveNext())
                                {
                                    m_listeners.RemoveAt(0);
                                }
                            }
                            m_listeners.CollectionChanged -= Listeners_CollectionChanged;
                        }
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="StatusUpdate"/> event.
        /// </summary>
        /// <param name="status">Status update message to send to <see cref="StatusUpdate"/> event.</param>
        protected virtual void OnStatusUpdate(string status)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, new EventArgs<string>(status));
        }

        /// <summary>
        /// Raises the <see cref="ExportProcessed"/> event.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to send to <see cref="ExportProcessed"/> event.</param>
        protected virtual void OnExportProcessed(Export export)
        {
            export.LastProcessError = null;
            export.LastProcessResult = ExportProcessResult.Success;
            if (ExportProcessed != null)
                ExportProcessed(this, new EventArgs<Export>(export));
        }

        /// <summary>
        /// Raises the <see cref="ExportProcessException"/> event.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to send to <see cref="ExportProcessException"/> event.</param>
        /// <param name="exception"><see cref="Exception"/> to send to <see cref="ExportProcessException"/> event.</param>
        protected virtual void OnExportProcessException(Export export, Exception exception)
        {
            export.LastProcessError = exception;
            export.LastProcessResult = ExportProcessResult.Failure;
            if (ExportProcessException != null)
                ExportProcessException(this, new EventArgs<Export>(export));
        }

        /// <summary>
        /// Handles the <see cref="DataListener.DataExtracted"/> event for all the <see cref="Listeners"/>.
        /// </summary>
        /// <param name="sender"><see cref="DataListener"/> object that raised the event.</param>
        /// <param name="e"><see cref="EventArgs{T}"/> object where <see cref="EventArgs{T}.Argument"/> is the collection of real-time time-sereis data received.</param>
        protected virtual void ProcessRealTimeData(object sender, EventArgs<IList<IDataPoint>> e)
        {
            DataListener listener = (DataListener)sender;
            List<Export> exportsList = new List<Export>();
            // Get a local copy of all the exports associated with this exporter.
            lock (m_exports)
            {
                exportsList.AddRange(m_exports);
            }

            // Process all the exports.
            foreach (Export export in exportsList)
            {
                if (export.Type == ExportType.RealTime && export.FindRecords(listener.ID).Count > 0)
                {
                    // This export is configured to be processed in real-time and has one or more records 
                    // from this listener to be exported, so we'll queue the real-time data for processing.
                    m_realTimeExportQueue.Add(new RealTimeData(export, listener, e.Argument));
                }
            }

            // We prevent flooding the queue by allowing a fixed number of request to queued at a given time.
            // Doing so also prevents running out-of-memory that could be caused by exporters taking longer than
            // normal to process its exports. Longer than normal processing time will back-log processing to a
            // point that it might become impossible to catch-up because of the rate at which data may be parsed.
            int drops = 0;
            while (m_realTimeExportQueue.Count > MaximumQueuedRequest)
            {
                m_realTimeExportQueue.RemoveAt(0);
                drops++;
            }
            if (drops > 0)
                OnStatusUpdate(string.Format("Dropped {0} time-series data points to prevent flooding.", drops));
        }

        /// <summary>
        /// Returns the current time-series data for the specified <paramref name="export"/> organized by listener.
        /// </summary>
        /// <param name="export"><see cref="Export"/> whose current time-series data is to be returned.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> object where the <b>key</b> is the <see cref="DataListener.Name"/> and <b>value</b> is the time-series data.</returns>
        protected Dictionary<string, IList<IDataPoint>> GetExportData(Export export)
        {
            // Arrange all export records by listeners.
            Dictionary<string, IList<ExportRecord>> exportRecords = new Dictionary<string, IList<ExportRecord>>(StringComparer.CurrentCultureIgnoreCase);
            foreach (ExportRecord record in export.Records)
            {
                if (!exportRecords.ContainsKey(record.Instance))
                    exportRecords.Add(record.Instance, new List<ExportRecord>());

                exportRecords[record.Instance].Add(record);
            }

            // Gather time-series data for the export records.
            DataListener listener;
            List<IDataPoint> listenerData = new List<IDataPoint>();
            Dictionary<string, IList<IDataPoint>> exportData = new Dictionary<string, IList<IDataPoint>>();
            foreach (string listenerName in exportRecords.Keys)
            {
                // Don't proceed if the listener doesn't exist.
                if ((listener = FindListener(listenerName)) == null)
                    continue;

                // Get a local copy of the listener's current data.
                listenerData.Clear();
                lock (listener.Data)
                {
                    listenerData.AddRange(listener.Data);
                }

                exportData.Add(listenerName, new List<IDataPoint>());
                if (exportRecords[listenerName].Count == 1 && exportRecords[listenerName][0].Identifier == -1)
                {
                    // Include all current data from the listener.
                    exportData[listenerName].AddRange(listenerData);
                }
                else
                {
                    // Specific records have been defined for this listener, so we'll gather data for those records.
                    foreach (ExportRecord record in exportRecords[listenerName])
                    {
                        if (record.Identifier <= listenerData.Count)
                        {
                            // Data does exist for the defined point.
                            exportData[listenerName].Add(listenerData[record.Identifier - 1]);
                        }
                        else
                        {
                            // Data does not exist for the point, so provide empty data.
                            exportData[listenerName].Add(new ArchiveDataPoint(record.Identifier));
                        }
                    }
                }
            }
            return exportData;
        }

        /// <summary>
        /// Returns the current time-series data for the specified <paramref name="export"/> in a <see cref="DataSet"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> whose current time-series data is to be returned.</param>
        /// <param name="dataTableName">Name of the <see cref="DataTable"/> containing the time-series data.</param>
        /// <returns>A <see cref="DataSet"/> object.</returns>
        protected DataSet GetExportDataAsDataset(Export export, string dataTableName)
        {
            DataSet result = ExporterBase.DatasetTemplate(dataTableName);
            Dictionary<string, IList<IDataPoint>> exportData = GetExportData(export);

            // Populate the dataset with time-series data.
            foreach (string listenerName in exportData.Keys)
            {
                foreach (IDataPoint dataPoint in exportData[listenerName])
                {
                    result.Tables[0].Rows.Add(listenerName, dataPoint.HistorianID, dataPoint.Time.ToString(), dataPoint.Value, (int)dataPoint.Quality);
                }
            }

            // Set the export timestamp, row count and interval.
            result.Tables[1].Rows.Add(DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss tt"), result.Tables[0].Rows.Count, string.Format("{0} seconds", export.Interval));

            return result;
        }

        /// <summary>
        /// Processes non-real-time exports.
        /// </summary>
        private void ProcessExports(Export[] items)
        {
            Stopwatch watch = new Stopwatch();
            foreach (Export item in items)
            {
                try
                {
                    watch.Reset();
                    watch.Start();
                    ProcessExport(item);
                    watch.Stop();
                    item.LastProcessTime = watch.Elapsed.TotalSeconds;
                    OnExportProcessed(item);
                }
                catch (Exception ex)
                {
                    OnExportProcessException(item, ex);
                }
            }
        }

        /// <summary>
        /// Processes real-time exports.
        /// </summary>
        private void ProcessRealTimeExports(RealTimeData[] items)
        {
            IList<ExportRecord> exportRecords;
            foreach (RealTimeData item in items)
            {
                try
                {
                    List<IDataPoint> filteredData = new List<IDataPoint>();

                    exportRecords = item.Export.FindRecords(item.Listener.ID);
                    if (exportRecords.Count == 1 && exportRecords[0].Identifier == -1)
                    {
                        // Include all data from the listener.
                        filteredData.AddRange(item.Data);
                    }
                    else
                    {
                        // Export data for selected records only (filtered).
                        foreach (IDataPoint dataPoint in item.Data)
                        {
                            if (exportRecords.FirstOrDefault(record => record.Identifier == dataPoint.HistorianID) != null)
                                filteredData.Add(dataPoint);
                        }
                    }

                    ProcessRealTimeExport(item.Export, item.Listener, filteredData);
                    item.Export.LastProcessResult = ExportProcessResult.Success;
                }
                catch (Exception ex)
                {
                    OnExportProcessException(item.Export, ex);
                }
            }
        }

        private void ExportTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<Export> exports = new List<Export>();
            lock (m_exports)
            {
                exports.AddRange(m_exports);
            }

            // Process all exports and queue them for processing if it's time.
            foreach (Export export in exports)
            {
                if (export.Type == ExportType.Intervaled && export.ShouldProcess())
                {
                    m_nonRealTimeExportQueue.Add(export);
                }
            }
        }

        private void Exports_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // Notify that a new export is added.
                    if (m_exportAddedHandler != null)
                    {
                        foreach (Export export in e.NewItems)
                        {
                            try
                            {
                                m_exportAddedHandler(export);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Notify that an existing export is removed.
                    if (m_exportRemovedHandler != null)
                    {
                        foreach (Export export in e.OldItems)
                        {
                            try
                            {
                                m_exportRemovedHandler(export);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // Notify that an existing export is updated.
                    if (m_exportUpdatedHandler != null)
                    {
                        foreach (Export export in e.NewItems)
                        {
                            try
                            {
                                m_exportUpdatedHandler(export);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Listeners_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // Register event handler.
                    foreach (DataListener listener in e.NewItems)
                    {
                        listener.DataExtracted += ProcessRealTimeData;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Unregister event handler.
                    foreach (DataListener listener in e.NewItems)
                    {
                        listener.DataExtracted -= ProcessRealTimeData;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region [ Static ]

        // Static Methods

        /// <summary>
        /// Returns a template <see cref="DataSet"/> that can be used for storing time-series data in a tabular format.
        /// </summary>
        /// <param name="dataTableName">Name of the <see cref="DataTable"/> that will be used for storing the time-series data.</param>
        /// <returns>A <see cref="DataSet"/> object.</returns>
        /// <remarks>
        /// <para>
        /// The returned <see cref="DataSet"/> consists of two <see cref="DataTable"/>s with the following structure:<br/>
        /// </para>
        /// <para>
        /// Table 1 is to be used for storing time-series data.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Column Name</term>
        ///         <description>Column Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Instance</term>
        ///         <description>Historian instance providing the time-series data.</description>
        ///     </item>
        ///     <item>
        ///         <term>ID</term>
        ///         <description><see cref="IDataPoint.HistorianID"/> of the time-series data.</description>
        ///     </item>
        ///     <item>
        ///         <term>Time</term>
        ///         <description><see cref="IDataPoint.Time"/> of the time-series data.</description>
        ///     </item>
        ///     <item>
        ///         <term>Value</term>
        ///         <description><see cref="IDataPoint.Value"/> of the time-series data.</description>
        ///     </item>
        ///     <item>
        ///         <term>Quality</term>
        ///         <description><see cref="IDataPoint.Quality"/> of the time-series data.</description>
        ///     </item>
        /// </list>
        /// Table 2 is to be used for providing information about Table 1.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Column Name</term>
        ///         <description>Column Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>RunTime</term>
        ///         <description><see cref="DateTime"/> (in UTC) when the data in Table 1 was populated.</description>
        ///     </item>
        ///     <item>
        ///         <term>RecordCount</term>
        ///         <description>Number of time-series data points in Table 1.</description>
        ///     </item>
        ///     <item>
        ///         <term>RefreshSchedule</term>
        ///         <description>Interval (in seconds) at which the data in Table 1 is to be refreshed.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static DataSet DatasetTemplate(string dataTableName)
        {
            // Provide output table name if none provided.
            if (string.IsNullOrEmpty(dataTableName))
                dataTableName = "Measurements";

            DataSet data = new DataSet(dataTableName);
            // -- Table 1 --
            data.Tables.Add(dataTableName);
            DataTable dataTable = data.Tables[dataTableName];
            dataTable.Columns.Add("Instance", typeof(string));
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Time", typeof(string));
            dataTable.Columns.Add("Value", typeof(float));
            dataTable.Columns.Add("Quality", typeof(int));
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0], dataTable.Columns[1], dataTable.Columns[2] };
            // -- Table 2 --
            data.Tables.Add("ExportInformation");
            DataTable infoTable = data.Tables["ExportInformation"];
            infoTable.Columns.Add("RunTime", typeof(string));
            infoTable.Columns.Add("RecordCount", typeof(string));
            infoTable.Columns.Add("RefreshSchedule", typeof(string));

            // We'll output the xml data as attributes to save space.
            foreach (DataTable table in data.Tables)
            {
                foreach (DataColumn column in table.Columns)
                {
                    column.ColumnMapping = MappingType.Attribute;
                }
            }

            return data;
        }

        #endregion
    }
}
