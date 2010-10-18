//******************************************************************************************************
//  RawDataExporter.cs - Gbtc
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
//  10/17/2007 - Pinal C. Patel
//       Original version of source code generated.
//  02/14/2008 - Pinal C. Patel
//       Added Protected GetBufferedData() method for inheritting class to use the biffered data.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/16/2009 - Pinal C. Patel
//       Modified to use the process queue exposed by the base class for processing real-time data.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using TVA;

namespace openHistorian.Exporters
{
    /// <summary>
    /// Represents an exporter that can export real-time time-series data in CSV or XML format to a file.
    /// </summary>
    /// <example>
    /// Definition of a sample <see cref="Export"/> that can be processed by <see cref="RawDataExporter"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <Export xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///   <Name>RawDataExport</Name>
    ///   <Type>Intervaled</Type>
    ///   <Interval>10</Interval>
    ///   <Exporter>RawDataExporter</Exporter>
    ///   <Settings>
    ///     <ExportSetting>
    ///       <Name>OutputFile</Name>
    ///       <Value>c:\RawDataExporterOutput.xml</Value>
    ///     </ExportSetting>
    ///     <ExportSetting>
    ///       <Name>OutputFormat</Name>
    ///       <Value>XML</Value>
    ///     </ExportSetting>    
    ///   </Settings>
    ///   <Records>
    ///     <ExportRecord>
    ///       <Instance>P3</Instance>
    ///       <Identifier>3261</Identifier>
    ///     </ExportRecord>
    ///     <ExportRecord>
    ///       <Instance>P3</Instance>
    ///       <Identifier>3266</Identifier>
    ///     </ExportRecord>  
    ///  </Records>
    /// </Export>
    /// ]]>
    /// </code>
    /// <para>
    /// Description of custom settings required by <see cref="RawDataExporter"/> in an <see cref="Export"/>:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Setting Name</term>
    ///         <description>Setting Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>OutputFile</term>
    ///         <description>Name of the CSV or XML file (including path) where export data is to be written.</description>
    ///     </item>
    ///     <item>
    ///         <term>OutputFormat</term>
    ///         <description>Format (CSV or XML) in which export data is to be written to the output file.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </example>
    /// <seealso cref="Export"/>
    public class RawDataExporter : ExporterBase
    {
        #region [ Members ]

        // Fields
        private Dictionary<string, DataSet> m_rawData;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RawDataExporter"/> class.
        /// </summary>
        public RawDataExporter()
            : this("RawDataExporter")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawDataExporter"/> class.
        /// </summary>
        /// <param name="name"><see cref="ExporterBase.Name"/> of the exporter.</param>
        protected RawDataExporter(string name)
            : base(name)
        {
            m_rawData = new Dictionary<string, DataSet>();

            // Register handlers.
            ExportAddedHandler = CreateBuffer;
            ExportRemovedHandler = RemoveBuffer;
            RealTimeExportQueue.ProcessItemsFunction = ProcessRealTimeExport;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Processes the <paramref name="export"/> using the current <see cref="DataListener.Data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        /// <exception cref="ArgumentException"><b>OutputFile</b> or <b>OutputFormat</b> setting is missing from the <see cref="Export.Settings"/> of the <paramref name="export"/>.</exception>
        protected override void ProcessExport(Export export)
        {
            DataSet rawData = GetBufferedData(export.Name);
            try
            {
                // Ensure that required settings are present.
                ExportSetting outputFileSetting = export.FindSetting("OutputFile");
                if (outputFileSetting == null)
                    throw new ArgumentException("OutputFile setting is missing");
                ExportSetting outputFormatSetting = export.FindSetting("OutputFormat");
                if (outputFormatSetting == null)
                    throw new ArgumentException("OutputFormat setting is missing");

                if (rawData != null)
                {
                    // Buffered data exists for exporting.
                    lock (rawData)
                    {
                        // Update the export timestamp, row count and interval.
                        rawData.Tables[1].Rows.Add(DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss tt"), rawData.Tables[0].Rows.Count, string.Format("{0} seconds", export.Interval));

                        // Write the buffered raw data for the export to the specified files in specified format.
                        FileHelper.WriteToFile(outputFileSetting.Value, outputFormatSetting.Value, rawData);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (rawData != null)
                {
                    lock (rawData)
                    {
                        // Clear the buffered data to make space for new data that is to be buffered.
                        rawData.Tables[0].Clear();
                        rawData.Tables[1].Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Processes the <paramref name="export"/> using the real-time <paramref name="data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        /// <param name="listener"><see cref="DataListener"/> that provided the <paramref name="data"/>.</param>
        /// <param name="data">Real-time time-series data received by the <paramref name="listener"/>.</param>
        /// <exception cref="NotSupportedException">Always</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ProcessRealTimeExport(Export export, DataListener listener, IList<IDataPoint> data)
        {
            throw new NotSupportedException();  // Real-Time exports not supported.
        }

        /// <summary>
        /// Handles the <see cref="DataListener.DataExtracted"/> event for all the <see cref="ExporterBase.Listeners"/>.
        /// </summary>
        /// <param name="sender"><see cref="DataListener"/> object that raised the event.</param>
        /// <param name="e"><see cref="EventArgs{T}"/> object where <see cref="EventArgs{T}.Argument"/> is the collection of real-time time-sereis data received.</param>
        protected override void ProcessRealTimeData(object sender, EventArgs<IList<IDataPoint>> e)
        {
            DataListener listener = (DataListener)sender;
            List<Export> exportsList = new List<Export>();
            lock (Exports)
            {
                // Get a local copy of all the exports this exporter is responsible for.
                exportsList.AddRange(Exports);
            }

            // Queue the export and parsed data for processing if:
            // - Export has records that are from the listener whose data we have received
            // - Export is not "Real-Time", since real-time exports are not supported by this exporter
            foreach (Export export in exportsList)
            {
                if (export.FindRecords(listener.ID).Count > 0 && export.Type != ExportType.RealTime)
                    RealTimeExportQueue.Add(new RealTimeData(export, listener, e.Argument));
            }
        }

        /// <summary>
        /// Returns a <see cref="DataSet"/> containing the buffered real-time time-series data.
        /// </summary>
        /// <param name="exportName"><see cref="Export.Name"/> of the export whose buffered data is to be retrieved.</param>
        /// <returns>A <see cref="DataSet"/> object if buffered data exists; otherwise null.</returns>
        protected DataSet GetBufferedData(string exportName)
        {
            DataSet data = null;
            lock (m_rawData)
            {
                // Get the buffered data for the export.
                m_rawData.TryGetValue(exportName, out data);
            }

            return data;
        }

        private void ProcessRealTimeExport(RealTimeData[] items)
        {
            DataSet rawData;
            IList<ExportRecord> exportRecords;
            foreach (RealTimeData item in items)
            {
                try
                {                  
                    lock (m_rawData)
                    {
                        m_rawData.TryGetValue(item.Export.Name, out rawData);
                    }

                    if (rawData != null)
                    {
                        // Buffer-up the parsed data for the export.
                        exportRecords = item.Export.FindRecords(item.Listener.ID);
                        if (exportRecords.Count == 1 && exportRecords[0].Identifier == -1)
                        {
                            // Include all data from the listener.
                            if (Monitor.TryEnter(rawData))
                            {
                                try
                                {
                                    foreach (IDataPoint dataPoint in item.Data)
                                    {
                                        rawData.Tables[0].Rows.Add(item.Listener.ID, dataPoint.HistorianID, dataPoint.Time.ToString(), dataPoint.Value, (int)dataPoint.Quality);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                finally
                                {
                                    Monitor.Exit(rawData);
                                }
                            }
                        }
                        else
                        {
                            // Buffer data for selected records only (filtered).
                            ExportRecord comparer = new ExportRecord(item.Listener.ID, -1);
                            if (Monitor.TryEnter(rawData))
                            {
                                try
                                {
                                    foreach (IDataPoint dataPoint in item.Data)
                                    {
                                        if (exportRecords.FirstOrDefault(record => record.Identifier == dataPoint.HistorianID) != null)
                                            rawData.Tables[0].Rows.Add(item.Listener.ID, dataPoint.HistorianID, dataPoint.Time.ToString(), dataPoint.Value, (int)dataPoint.Quality);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                finally
                                {
                                    Monitor.Exit(rawData);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnStatusUpdate(string.Format("Data prep failed for export \"{0}\" - {1}", item.Export.Name, ex.Message));
                }
            }
        }

        private void CreateBuffer(Export export)
        {
            DataSet rawData = null;
            lock (m_rawData)
            {
                if (!m_rawData.TryGetValue(export.Name, out rawData))
                {
                    // Dataset for buffering export data does not exist.
                    rawData = ExporterBase.DatasetTemplate(null);

                    // Save the dataset.
                    m_rawData.Add(export.Name, rawData);

                    // Provide a status update.
                    OnStatusUpdate(string.Format("Data buffer created for export {0}", export.Name));
                }
            }
        }

        private void RemoveBuffer(Export export)
        {
            lock (m_rawData)
            {
                DataSet data = null;
                if (m_rawData.TryGetValue(export.Name, out data))
                {
                    // Remove the dataset used for buffering export data.
                    data.Dispose();
                    m_rawData.Remove(export.Name);

                    // Provide a status update.
                    OnStatusUpdate(string.Format("Data buffer of export {0} removed", export.Name));
                }
            }
        }
      
        #endregion
    }
}