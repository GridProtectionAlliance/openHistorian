//******************************************************************************************************
//  TimeSeriesDataService.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
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
//  11/07/2010 - Pinal C. Patel
//       Modified to fix breaking changes made to SelfHostingService.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using openHistorian.Archives.V1;

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
            Endpoints = "http.rest://localhost:6152/rest; http.soap11://localhost:6152/soap";
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Writes received <paramref name="data"/> to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="data">An <see cref="SerializableTimeSeriesData"/> object.</param>
        public void WriteTimeSeriesData(SerializableTimeSeriesData data)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return;

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

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadSelectCurrentTimeSeriesData(string idList)
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

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

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which current time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadRangeCurrentTimeSeriesData(string fromID, string toID)
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

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

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadSelectHistoricTimeSeriesData(string idList, string startTime, string endTime)
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

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
                    foreach (IData item in Archive.ReadData(id, startTime, endTime))
                    {
                        points.Add(new SerializableTimeSeriesDataPoint(item));
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

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        public SerializableTimeSeriesData ReadRangeHistoricTimeSeriesData(string fromID, string toID, string startTime, string endTime)
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read historic time-series data from the archive.
                SerializableTimeSeriesData data = new SerializableTimeSeriesData();
                List<SerializableTimeSeriesDataPoint> points = new List<SerializableTimeSeriesDataPoint>();
                for (int id = int.Parse(fromID); id <= int.Parse(toID); id++)
                {
                    foreach (IData item in Archive.ReadData(id, startTime, endTime))
                    {
                        points.Add(new SerializableTimeSeriesDataPoint(item));
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

        private void SupportJson()
        {
            // Check if requestor accepts response in JSON format.
            string acceptHeader = ((HttpRequestMessageProperty)OperationContext.Current.RequestContext.RequestMessage.Properties[HttpRequestMessageProperty.Name]).Headers["Accept"];
            if (!string.IsNullOrEmpty(acceptHeader) && acceptHeader.Contains("application/json"))
                WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
        }

        #endregion
    }
}
