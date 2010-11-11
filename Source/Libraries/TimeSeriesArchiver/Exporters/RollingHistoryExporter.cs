//******************************************************************************************************
//  RollingHistoryExporter.cs - Gbtc
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
//  07/04/2007 - Pinal C. Patel
//       Original version of source code generated.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using TVA.Data;
using TVA.IO;

namespace TimeSeriesArchiver.Exporters
{
    /// <summary>
    /// Represents an exporter that can export current and runtime historic time-series data in CSV or XML format to a file.
    /// </summary>
    /// <example>
    /// Definition of a sample <see cref="Export"/> that can be processed by <see cref="RollingHistoryExporter"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <Export xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///   <Name>RollingHistoryExport</Name>
    ///   <Type>Intervaled</Type>
    ///   <Interval>60</Interval>
    ///   <Exporter>RollingHistoryExporter</Exporter>
    ///   <Settings>
    ///     <ExportSetting>
    ///       <Name>OutputFile</Name>
    ///       <Value>c:\RollingHistoryExportOutput.xml</Value>
    ///     </ExportSetting>
    ///     <ExportSetting>
    ///       <Name>OutputFormat</Name>
    ///       <Value>XML</Value>
    ///     </ExportSetting>    
    ///     <ExportSetting>
    ///       <Name>OutputTimespan</Name>
    ///       <Value>360</Value>
    ///     </ExportSetting>        
    ///   </Settings>
    ///   <Records>
    ///     <ExportRecord>
    ///       <Instance>TP</Instance>
    ///       <Identifier>25609</Identifier>
    ///     </ExportRecord>
    ///     <ExportRecord>
    ///       <Instance>TP</Instance>
    ///       <Identifier>41517</Identifier>
    ///     </ExportRecord>
    ///  </Records>
    /// </Export>
    /// ]]>
    /// </code>
    /// <para>
    /// Description of custom settings required by <see cref="RollingHistoryExporter"/> in an <see cref="Export"/>:
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
    ///     <item>
    ///         <term>OutputTimespan</term>
    ///         <description>Span (represented in seconds) of the moving window for which the runtime historic data is to be exported.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </example>
    /// <seealso cref="Export"/>
    public class RollingHistoryExporter : ExporterBase
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingHistoryExporter"/> class.
        /// </summary>
        public RollingHistoryExporter()
            : this("RollingHistoryExporter")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingHistoryExporter"/> class.
        /// </summary>
        /// <param name="name"><see cref="ExporterBase.Name"/> of the exporter.</param>
        protected RollingHistoryExporter(string name)
            : base(name)
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Processes the <paramref name="export"/> using the current <see cref="DataListener.Data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        /// <exception cref="ArgumentException"><b>OutputFile</b>, <b>OutputFormat</b> or <b>OutputTimespan</b> setting is missing from the <see cref="Export.Settings"/> of the <paramref name="export"/>.</exception>
        protected override void ProcessExport(Export export)
        {
            // Ensure that required settings are present.
            ExportSetting outputFileSetting = export.FindSetting("OutputFile");
            if (outputFileSetting == null)
                throw new ArgumentException("OutputFile setting is missing");
            ExportSetting outputFormatSetting = export.FindSetting("OutputFormat");
            if (outputFormatSetting == null)
                throw new ArgumentException("OutputFormat setting is missing");
            ExportSetting outputTimespanSetting = export.FindSetting("OutputTimespan");
            if (outputTimespanSetting == null)
                throw new ArgumentException("OutputTimespan setting is missing");

            // Initialize local variables.
            DataSet output = null;
            string[] outputFiles = outputFileSetting.Value.Split(';', ',');
            DataSet current = GetExportDataAsDataset(export, null); 
            DateTime dataStartDate = DateTime.UtcNow.AddMinutes(-Convert.ToInt32(outputTimespanSetting.Value));

            // Make the output filenames absolute.
            for (int i = 0; i < outputFiles.Length; i++)
            {
                outputFiles[i] = FilePath.GetAbsolutePath(outputFiles[i].Trim());
            }

            // Try to read-in data that might have been exported previously.
            foreach (string outputFile in outputFiles)
            {
                if (File.Exists(outputFile))
                {
                    // Wait for a lock on the file before reading.
                    FilePath.WaitForReadLock(outputFile, FileLockWaitTime);

                    // Read the file data and keep it in memory to be merged later.
                    output = current.Clone();
                    switch (outputFormatSetting.Value.ToLower())
                    {
                        case "xml":
                            output.ReadXml(outputFile);
                            break;
                        case "csv":
                            MergeTables(output.Tables[0], File.ReadAllText(outputFile).ToDataTable(",", true), dataStartDate);
                            break;
                        default:
                            break;
                    }

                    break; // Don't look any further.
                }
            }

            if (output == null)
                // Output will be just current data since there is no previously exported data.
                output = current;
            else
                // Merge previously exported data loaded from file with the current data for the export.
                MergeTables(output.Tables[0], current.Tables[0], dataStartDate);

            // Delete any old data from the output that don't fall in the export's rolling window.
            string filterCondition = string.Format("Convert(Time, System.DateTime) < #{0}#", dataStartDate);
            foreach (DataRow row in output.Tables[0].Select(filterCondition))
            {
                row.Delete();
            }

            // Update the export timestamp, row count and interval.
            DataRowCollection info = output.Tables[1].Rows;
            info.Clear();
            info.Add(DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss tt"), output.Tables[0].Rows.Count, string.Format("{0} seconds", export.Interval));

            // Write the data for the export to the specified files in specified format.
            FileHelper.WriteToFile(outputFileSetting.Value, outputFormatSetting.Value, output);
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

        private void MergeTables(DataTable history, DataTable current, DateTime startDate)
        {
            DateTime dt;
            foreach (DataRow newData in current.Rows)
            {
                dt = Convert.ToDateTime(newData["Time"]);
                if (dt != TimeTag.MinValue)
                {
                    // Data is real and not made-up.
                    if (dt >= startDate)
                    {
                        // Data falls within the export's rolling window.
                        try
                        {
                            history.Rows.Add(newData.ItemArray);
                        }
                        catch (Exception)
                        {
                            // Exception is encountered if duplicate data is inserted.
                            AddDummyData(history, newData);
                        }
                    }
                    else
                    {
                        // Data is outside the export's rolling window.
                        AddDummyData(history, newData);
                    }
                }
                else
                {
                    // Data is not real but made-up.
                    AddDummyData(history, newData);
                }
            }
        }

        private void AddDummyData(DataTable table, DataRow data)
        {
            // "Dummy" data is data of a point that is manufatured by changing the timetag to current timetag
            // and putting in blanks in Value and Quality fields. This is needed in the following cases:
            // - Data that has been added as placeholder because there is no actual data for that point is
            //   available yet.
            // - Data for a point was last received before the export's rolling history window.
            // - Data for a point was not updated between the current and previous runs. This is required because
            //   Instance, ID and Time make up the primary key for the table and since the data between runs has
            //   not changed, we'll be adding duplicates resulting in an exception. So we add "dummy" data for that
            //   point since we have to provide data for every defined point during every run.
            table.Rows.Add(data["Instance"], data["ID"], new TimeTag(DateTime.UtcNow).ToString(), float.NaN, Quality.Unknown);
        }

        #endregion
    }
}