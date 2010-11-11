//******************************************************************************************************
//  ITimeSeriesDataService.cs - Gbtc
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
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System.ServiceModel;
using System.ServiceModel.Web;

namespace TimeSeriesArchiver.DataServices
{
    /// <summary>
    /// Defines a REST web service for time-series data.
    /// </summary>
    /// <seealso cref="SerializableTimeSeriesData"/>
    [ServiceContract()]
    public interface ITimeSeriesDataService
    {
        #region [ Methods ]

        /// <summary>
        /// Writes <paramref name="data"/> received in <see cref="WebMessageFormat.Xml"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="data">An <see cref="SerializableTimeSeriesData"/> object.</param>
        [OperationContract(), 
        WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, UriTemplate = "/timeseriesdata/write/xml")]
        void WriteTimeSeriesDataAsXml(SerializableTimeSeriesData data);

        /// <summary>
        /// Writes <paramref name="data"/> received in <see cref="WebMessageFormat.Json"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="data">An <see cref="SerializableTimeSeriesData"/> object.</param>
        [OperationContract(), 
        WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, UriTemplate = "/timeseriesdata/write/json")]
        void WriteTimeSeriesDataAsJson(SerializableTimeSeriesData data);

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/timeseriesdata/read/current/{idList}/xml")]
        SerializableTimeSeriesData ReadSelectCurrentTimeSeriesDataAsXml(string idList);

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which current time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/timeseriesdata/read/current/{fromID}-{toID}/xml")]
        SerializableTimeSeriesData ReadRangeCurrentTimeSeriesDataAsXml(string fromID, string toID);

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/timeseriesdata/read/current/{idList}/json")]
        SerializableTimeSeriesData ReadSelectCurrentTimeSeriesDataAsJson(string idList);

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which current time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/timeseriesdata/read/current/{fromID}-{toID}/json")]
        SerializableTimeSeriesData ReadRangeCurrentTimeSeriesDataAsJson(string fromID, string toID);

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/timeseriesdata/read/historic/{idList}/{startTime}/{endTime}/xml")]
        SerializableTimeSeriesData ReadSelectHistoricTimeSeriesDataAsXml(string idList, string startTime, string endTime);

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/timeseriesdata/read/historic/{fromID}-{toID}/{startTime}/{endTime}/xml")]
        SerializableTimeSeriesData ReadRangeHistoricTimeSeriesDataAsXml(string fromID, string toID, string startTime, string endTime);

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/timeseriesdata/read/historic/{idList}/{startTime}/{endTime}/json")]
        SerializableTimeSeriesData ReadSelectHistoricTimeSeriesDataAsJson(string idList, string startTime, string endTime);

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/timeseriesdata/read/historic/{fromID}-{toID}/{startTime}/{endTime}/json")]
        SerializableTimeSeriesData ReadRangeHistoricTimeSeriesDataAsJson(string fromID, string toID, string startTime, string endTime);

        #endregion
    }
}
