//******************************************************************************************************
//  TimeSeriesDataService.cs - Gbtc
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
//  09/01/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/10/2009 - Pinal C. Patel
//       Modified the logic in ReadCurrentTimeSeriesData() overloads to remove try-catch and check for 
//       null reference instead.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/15/2009 - Pinal C. Patel
//       Changed the default port for the service from 5152 to 6152.
//  03/30/2010 - Pinal C. Patel
//       Added check to verify that the service is enabled before processing incoming requests.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using openHistorian.Files;

namespace openHistorian.DataServices
{
    /// <summary>
    /// Represents a REST web service for time-series data.
    /// </summary>
    /// <seealso cref="SerializableTimeSeriesData"/>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TimeSeriesDataService : DataService, ITimeSeriesDataService
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesDataService"/> class.
        /// </summary>
        public TimeSeriesDataService()
            :base()
        {
            ServiceUri = "http://localhost:6152/historian";
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Writes <paramref name="data"/> received in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="data">An <see cref="SerializableTimeSeriesData"/> object.</param>
        public void WriteTimeSeriesDataAsXml(SerializableTimeSeriesData data)
        {
            WriteTimeSeriesData(data);
        }

        /// <summary>
        /// Writes <paramref name="data"/> received in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="data">An <see cref="SerializableTimeSeriesData"/> object.</param>
        public void WriteTimeSeriesDataAsJson(SerializableTimeSeriesData data)
        {
            WriteTimeSeriesData(data);
        }

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadSelectCurrentTimeSeriesDataAsXml(string idList)
        {
            return ReadCurrentTimeSeriesData(idList);
        }

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which current time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadRangeCurrentTimeSeriesDataAsXml(string fromID, string toID)
        {
            return ReadCurrentTimeSeriesData(fromID, toID);
        }

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadSelectCurrentTimeSeriesDataAsJson(string idList)
        {
            return ReadCurrentTimeSeriesData(idList);
        }

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which current time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadRangeCurrentTimeSeriesDataAsJson(string fromID, string toID)
        {
            return ReadCurrentTimeSeriesData(fromID, toID);
        }

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadSelectHistoricTimeSeriesDataAsXml(string idList, string startTime, string endTime)
        {
            return ReadHistoricTimeSeriesData(idList, startTime, endTime);
        }

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadRangeHistoricTimeSeriesDataAsXml(string fromID, string toID, string startTime, string endTime)
        {
            return ReadHistoricTimeSeriesData(fromID, toID, startTime, endTime);
        }

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadSelectHistoricTimeSeriesDataAsJson(string idList, string startTime, string endTime)
        {
            return ReadHistoricTimeSeriesData(idList, startTime, endTime);
        }

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadRangeHistoricTimeSeriesDataAsJson(string fromID, string toID, string startTime, string endTime)
        {
            return ReadHistoricTimeSeriesData(fromID, toID, startTime, endTime);
        }

        private void WriteTimeSeriesData(SerializableTimeSeriesData data)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return;

                // Ensure that writing data is allowed.
                if (!CanWrite)
                    throw new InvalidOperationException("Write operation is prohibited");

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Write all time-series data to the archive.
                foreach (SerializableTimeSeriesDataPoint point in data.TimeSeriesDataPoints)
                {
                    Archive.WriteData(point.Deflate());
                }
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableTimeSeriesData ReadCurrentTimeSeriesData(string idList)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that reading data is allowed.
                if (!CanRead)
                    throw new InvalidOperationException("Read operation is prohibited");

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read current time-series data from the archive.
                int id = 0;
                byte[] buffer = null;
                StateRecord state = null;
                SerializableTimeSeriesData data = new SerializableTimeSeriesData();
                List<SerializableTimeSeriesDataPoint> points = new List<SerializableTimeSeriesDataPoint>();
                foreach (string singleID in idList.Split(',', ';'))
                {
                    buffer = Archive.ReadStateData(id = int.Parse(singleID));
                    if (buffer == null)
                    {
                        // ID is invalid.
                        continue;
                    }
                    else
                    {
                        // Add to resultset.
                        state = new StateRecord(id, buffer, 0, buffer.Length);
                        points.Add(new SerializableTimeSeriesDataPoint(state.CurrentData));
                    }
                }
                data.TimeSeriesDataPoints = points.ToArray();

                return data;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableTimeSeriesData ReadCurrentTimeSeriesData(string fromID, string toID)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that reading data is allowed.
                if (!CanRead)
                    throw new InvalidOperationException("Read operation is prohibited");

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read current time-series data from the archive.
                byte[] buffer = null;
                StateRecord state = null;
                SerializableTimeSeriesData data = new SerializableTimeSeriesData();
                List<SerializableTimeSeriesDataPoint> points = new List<SerializableTimeSeriesDataPoint>();
                for (int id = int.Parse(fromID); id <= int.Parse(toID); id++)
                {
                    buffer = Archive.ReadStateData(id);
                    if (buffer == null)
                    {
                        // ID is invalid.
                        continue;
                    }
                    else
                    {
                        // Add to resultset.
                        state = new StateRecord(id, buffer, 0, buffer.Length);
                        points.Add(new SerializableTimeSeriesDataPoint(state.CurrentData));
                    }
                }
                data.TimeSeriesDataPoints = points.ToArray();

                return data;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableTimeSeriesData ReadHistoricTimeSeriesData(string idList, string startTime, string endTime)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that reading data is allowed.
                if (!CanRead)
                    throw new InvalidOperationException("Read operation is prohibited");

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read historic time-series data from the archive.
                int id = 0;
                SerializableTimeSeriesData data = new SerializableTimeSeriesData();
                List<SerializableTimeSeriesDataPoint> points = new List<SerializableTimeSeriesDataPoint>();
                foreach (string singleID in idList.Split(',', ';'))
                {
                    id = int.Parse(singleID);
                    foreach (IDataPoint point in Archive.ReadData(id, startTime, endTime))
                    {
                        points.Add(new SerializableTimeSeriesDataPoint(point));
                    }
                }
                data.TimeSeriesDataPoints = points.ToArray();

                return data;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableTimeSeriesData ReadHistoricTimeSeriesData(string fromID, string toID, string startTime, string endTime)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that reading data is allowed.
                if (!CanRead)
                    throw new InvalidOperationException("Read operation is prohibited");

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read historic time-series data from the archive.
                SerializableTimeSeriesData data = new SerializableTimeSeriesData();
                List<SerializableTimeSeriesDataPoint> points = new List<SerializableTimeSeriesDataPoint>();
                for (int id = int.Parse(fromID); id <= int.Parse(toID); id++)
                {
                    foreach (IDataPoint point in Archive.ReadData(id, startTime, endTime))
                    {
                        points.Add(new SerializableTimeSeriesDataPoint(point));
                    }
                }
                data.TimeSeriesDataPoints = points.ToArray();

                return data;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        #endregion
    }
}
