//******************************************************************************************************
//  SortedDataBlockScanner.cs - Gbtc
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
//  ----------------------------------------------------------------------------------------------------
//  11/08/2011 - J. Ritchie Carroll
//       Generated original version of source code.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using TVA;

namespace openHistorian.V1.Files
{
    /// <summary>
    /// Reads a series of data points from an <see cref="ArchiveFile"/> in sorted order.
    /// </summary>
    public class TimeSortedDataPointScanner
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Individual data point scanner.
        /// </summary>
        private class DataPointScanner
        {
            #region [ Members ]

            // Fields
            private List<ArchiveDataBlock> m_dataBlocks;
            private TimeTag m_startTime;
            private TimeTag m_endTime;
            private EventHandler<EventArgs<Exception>> m_dataReadExceptionHandler;

            #endregion

            #region [ Constructors ]

            /// <summary>
            /// Creates a new <see cref="DataPointScanner"/> instance.
            /// </summary>
            /// <param name="dataBlockAllocationTable"><see cref="ArchiveFileAllocationTable"/> for the file to be scanned.</param>
            /// <param name="historianID">Historian ID to scan for.</param>
            /// <param name="startTime">Desired start time.</param>
            /// <param name="endTime">Desired end time.</param>
            /// <param name="dataReadExceptionHandler">Read exception handler.</param>
            public DataPointScanner(ArchiveFileAllocationTable dataBlockAllocationTable, int historianID, TimeTag startTime, TimeTag endTime, EventHandler<EventArgs<Exception>> dataReadExceptionHandler)
            {
                // Find all data blocks for desired point over given time range
                m_dataBlocks = dataBlockAllocationTable.FindDataBlocks(historianID, startTime, endTime, false);
                m_startTime = startTime;
                m_endTime = endTime;
                m_dataReadExceptionHandler = dataReadExceptionHandler;
            }

            #endregion

            #region [ Methods ]

            /// <summary>
            /// Reads all <see cref="IDataPoint"/>s from the <see cref="ArchiveDataBlock"/>s.
            /// </summary>
            /// <returns>Each <see cref="IDataPoint"/> read from the <see cref="ArchiveDataBlock"/>s.</returns>
            public IEnumerable<IDataPoint> Read()
            {
                int count = m_dataBlocks.Count;
                int index = 0;

                // Loop through each data block
                foreach (ArchiveDataBlock dataBlock in m_dataBlocks)
                {
                    // Attach to data read exception event for the data block
                    dataBlock.DataReadException += m_dataReadExceptionHandler;

                    // Pre-read all data points in this block
                    List<IDataPoint> dataPoints = new List<IDataPoint>();

                    if (index == 0 || index == count - 1)
                    {
                        // Read data through first and last data blocks validating time range
                        foreach (IDataPoint dataPoint in dataBlock.Read())
                        {
                            if (dataPoint.Time.CompareTo(m_startTime) >= 0 && dataPoint.Time.CompareTo(m_endTime) <= 0)
                                dataPoints.Add(dataPoint);
                        }
                    }
                    else
                    {
                        // Read all of the data from the rest of the data blocks
                        foreach (IDataPoint dataPoint in dataBlock.Read())
                        {
                            dataPoints.Add(dataPoint);
                        }
                    }

                    // Detach from data read exception event for the data block
                    dataBlock.DataReadException -= m_dataReadExceptionHandler;

                    foreach (IDataPoint dataPoint in dataPoints)
                    {
                        yield return dataPoint;
                    }

                    index++;
                }
            }

            #endregion
        }

        // Fields
        private List<DataPointScanner> m_dataPointScanners;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="TimeSortedDataPointScanner"/>. 
        /// </summary>
        /// <param name="dataBlockAllocationTable"><see cref="ArchiveFileAllocationTable"/> for the file to be scanned.</param>
        /// <param name="historianIDs">Historian ID's to scan.</param>
        /// <param name="startTime">Desired start time.</param>
        /// <param name="endTime">Desired end time.</param>
        /// <param name="dataReadExceptionHandler">Read exception handler.</param>
        public TimeSortedDataPointScanner(ArchiveFileAllocationTable dataBlockAllocationTable, IEnumerable<int> historianIDs, TimeTag startTime, TimeTag endTime, EventHandler<EventArgs<Exception>> dataReadExceptionHandler)
        {
            m_dataPointScanners = new List<DataPointScanner>();

            // Create data point scanners for each historian ID
            foreach (int historianID in historianIDs)
            {
                m_dataPointScanners.Add(new DataPointScanner(dataBlockAllocationTable, historianID, startTime, endTime, dataReadExceptionHandler));
            }
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Reads all <see cref="IDataPoint"/>s in time sorted order for the specified historian IDs.
        /// </summary>
        /// <returns>Each <see cref="IDataPoint"/> for the specified historian IDs.</returns>
        public IEnumerable<IDataPoint> Read()
        {
            List<IEnumerator<IDataPoint>> enumerators = new List<IEnumerator<IDataPoint>>();

            // Setup enumerators for scanners that have data
            foreach (DataPointScanner scanner in m_dataPointScanners)
            {
                IEnumerator<IDataPoint> enumerator = scanner.Read().GetEnumerator();

                // Add enumerator to the list if it has at least one value
                if (enumerator.MoveNext())
                    enumerators.Add(enumerator);
            }

            // Start publishing data points in time-sorted order
            if (enumerators.Count > 0)
            {
                List<int> completed = new List<int>();
                IDataPoint dataPoint;

                do
                {
                    TimeTag publishTime = TimeTag.MaxValue;

                    // Find minimum publication time for current values
                    foreach (IEnumerator<IDataPoint> enumerator in enumerators)
                    {
                        dataPoint = enumerator.Current;

                        if (dataPoint.Time.CompareTo(publishTime) < 0)
                            publishTime = dataPoint.Time;
                    }

                    int index = 0;

                    // Publish all values at the current time
                    foreach (IEnumerator<IDataPoint> enumerator in enumerators)
                    {
                        dataPoint = enumerator.Current;

                        if (dataPoint.Time.CompareTo(publishTime) <= 0)
                        {
                            // Attempt to advance to next data point, tracking completed enumerators
                            if (!enumerator.MoveNext())
                                completed.Add(index);

                            yield return dataPoint;
                        }

                        index++;
                    }

                    // Remove completed enumerators
                    if (completed.Count > 0)
                    {
                        completed.Sort();

                        for (int i = completed.Count - 1; i >= 0; i--)
                        {
                            enumerators.RemoveAt(completed[i]);
                        }

                        completed.Clear();
                    }
                }
                while (enumerators.Count > 0);
            }
        }

        #endregion
    }
}
