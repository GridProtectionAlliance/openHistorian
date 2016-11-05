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
using GSF.Diagnostics;

namespace GSF.TimeSeries.Data
{
    /// <summary>
    /// Creates a cached lookup so certain metadata so lookups can occur with quickly.
    /// </summary>
    public static class DataSourceLookups
    {
        private static readonly LogPublisher s_log = Logger.CreatePublisher(typeof(DataSourceLookups), MessageClass.Framework);

        private static readonly List<WeakReference<DataSourceLookupCache>> s_dataSetLookups = new List<WeakReference<DataSourceLookupCache>>();

        /// <summary>
        /// Gets/Creates the lookup cache for the provided dataset.
        /// </summary>
        /// <param name="dataSet">The non-null dataset provided by the time-series framework</param>
        /// <returns>Lookup cache for the provided dataset.</returns>
        public static DataSourceLookupCache GetLookupCache(DataSet dataSet)
        {
            if ((object)dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));

            //Since adding datasets will be rare, the penalty associated with a lock on the entire
            //set will be minor.
            lock (s_dataSetLookups)
            {
                DataSourceLookupCache target;

                for (int index = 0; index < s_dataSetLookups.Count; index++)
                {
                    WeakReference<DataSourceLookupCache> item = s_dataSetLookups[index];

                    if (item.TryGetTarget(out target) && target.DataSet != null)
                    {
                        if (ReferenceEquals(target.DataSet, dataSet))
                        {
                            return target;
                        }
                    }
                    else
                    {
                        s_log.Publish(MessageLevel.Info, "A DataSet object has been disposed or garbage collected. It will be removed from the list.");
                        s_dataSetLookups.RemoveAt(index);
                        index--;
                    }
                }

                s_log.Publish(MessageLevel.Info, "Creating a lookup cache for a dataset");
                target = new DataSourceLookupCache(dataSet);

                s_dataSetLookups.Add(new WeakReference<DataSourceLookupCache>(target));

                return target;
            }
        }

        /// <summary>
        /// Gets/Creates the <see cref="ActiveMeasurementsTableLookup"/> for the provided dataset.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns><see cref="ActiveMeasurementsTableLookup"/> for the provided dataset.</returns>
        public static ActiveMeasurementsTableLookup ActiveMeasurements(DataSet dataSet)
        {
            return GetLookupCache(dataSet).ActiveMeasurements;
        }
    }
}
