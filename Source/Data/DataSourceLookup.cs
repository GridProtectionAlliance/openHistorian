//******************************************************************************************************
//  DataSourceLookups.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/31/2016 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using GSF.Data;
using GSF.Diagnostics;

namespace GSF.TimeSeries.Data
{
    /// <summary>
    /// Creates a cached lookup so certain metadata so lookups can occur with quickly.
    /// </summary>
    public static class DataSourceLookups
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(DataSourceLookups), MessageClass.Framework);

        private static List<WeakReference<DataSourceLookupCache>> s_dataSetLookups = new List<WeakReference<DataSourceLookupCache>>();

        /// <summary>
        /// Gets/Creates the lookup cache for the provided dataset.
        /// </summary>
        /// <param name="dataSet">The non-null dataset provided by the time-series framework</param>
        /// <returns></returns>
        public static DataSourceLookupCache GetLookupCache(DataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));

            //Since adding datasets will be rare, the penalty associated with a lock on the entire
            //set will be minor.
            lock (s_dataSetLookups)
            {
                DataSourceLookupCache target;
                for (int index = 0; index < s_dataSetLookups.Count; index++)
                {
                    var item = s_dataSetLookups[index];
                    if (item.TryGetTarget(out target) && target.DataSet != null)
                    {
                        if (ReferenceEquals(target.DataSet, dataSet))
                        {
                            return target;
                        }
                    }
                    else
                    {
                        Log.Publish(MessageLevel.Info, "A DataSet object has been disposed or garbage collected. It will be removed from the list.");
                        s_dataSetLookups.RemoveAt(index);
                        index--;
                    }
                }

                Log.Publish(MessageLevel.Info, "Creating a lookup cache for a dataset");
                target = new DataSourceLookupCache(dataSet);
                s_dataSetLookups.Add(new WeakReference<DataSourceLookupCache>(target));
                return target;
            }
        }

        /// <summary>
        /// Gets/Creates the <see cref="ActiveMeasurementsTableLookup"/> for the provided dataset.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static ActiveMeasurementsTableLookup ActiveMeasurements(DataSet dataSet)
        {
            return GetLookupCache(dataSet).ActiveMeasurements;
        }
    }

    public class DataSourceLookupCache
    {
        internal DataSet DataSet { get; private set; }

        public readonly ActiveMeasurementsTableLookup ActiveMeasurements;

        internal DataSourceLookupCache(DataSet dataSet)
        {
            DataSet = dataSet;
            dataSet.Disposed += DataSet_Disposed;

            ActiveMeasurements = new ActiveMeasurementsTableLookup(dataSet);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DataSet_Disposed(object sender, EventArgs e)
        {
            DataSet = null;
            //Do Nothing. The purpose of this is so a strong reference to this class is maintained by DataSet.
        }
    }

    public class ActiveMeasurementsTableLookup
    {
        private readonly Dictionary<uint, List<DataRow>> m_lookupByDeviceID;
        private readonly Dictionary<string, List<DataRow>> m_lookupByDeviceNameNoStats;
        private readonly List<DataRow> m_emptySet;

        internal ActiveMeasurementsTableLookup(DataSet dataSet)
        {
            m_emptySet = new List<DataRow>();
            m_lookupByDeviceID = new Dictionary<uint, List<DataRow>>();
            m_lookupByDeviceNameNoStats = new Dictionary<string, List<DataRow>>(StringComparer.CurrentCultureIgnoreCase);

            if (dataSet.Tables.Contains("ActiveMeasurements"))
            {
                DataTable table = dataSet.Tables["ActiveMeasurements"];

                foreach (DataRow row in table.Rows)
                {
                    var id = row.AsUInt32("DeviceID");
                    if (id.HasValue)
                    {
                        List<DataRow> rowList;
                        if (!m_lookupByDeviceID.TryGetValue(id.Value, out rowList))
                        {
                            rowList = new List<DataRow>();
                            m_lookupByDeviceID[id.Value] = rowList;
                        }
                        rowList.Add(row);
                    }

                    if (!row.AsString("SignalType", string.Empty).Equals("STAT", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var device = row.AsString("Device");
                        if (device != null)
                        {
                            List<DataRow> rowList;
                            if (!m_lookupByDeviceNameNoStats.TryGetValue(device, out rowList))
                            {
                                rowList = new List<DataRow>();
                                m_lookupByDeviceNameNoStats[device] = rowList;
                            }
                            rowList.Add(row);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all of the rows with the provided deviceID. 
        /// Returns an empty set if the deviceID could not be found.
        /// </summary>
        /// <param name="deviceId">the deviceID to lookup.</param>
        /// <returns>
        /// Returns an empty set if the deviceID could not be found.
        /// </returns>
        public IEnumerable<DataRow> LookupByDeviceID(uint deviceId)
        {
            List<DataRow> rows;
            if (m_lookupByDeviceID.TryGetValue(deviceId, out rows))
            {
                return rows;
            }
            return m_emptySet;
        }

        /// <summary>
        /// Gets all of the rows with the provided device name. This will exclude
        /// all ActiveMeasurements that are classified as SignalType='STAT'. This is because 
        /// 'STAT's are not associated with a device in the database.
        /// Returns an empty set if the deviceID could not be found.
        /// </summary>
        /// <param name="deviceName">the device to lookup.</param>
        /// <returns>
        /// Returns an empty set if the device could not be found.
        /// </returns>
        public IEnumerable<DataRow> LookupByDeviceNameNoStat(string deviceName)
        {
            List<DataRow> rows;
            if (m_lookupByDeviceNameNoStats.TryGetValue(deviceName, out rows))
            {
                return rows;
            }
            return m_emptySet;
        }
    }
}
