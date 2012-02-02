//******************************************************************************************************
//  ITimeSeriesDataService.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  09/01/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************


using System.ServiceModel;
using System.ServiceModel.Web;

namespace openHistorian.DataServices
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
        /// Writes received <paramref name="data"/> to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="data">An <see cref="SerializableTimeSeriesData"/> object.</param>
        [OperationContract(), 
        WebInvoke(Method = "POST", UriTemplate = "/data/write")]
        void WriteTimeSeriesData(SerializableTimeSeriesData data);

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/data/read/current/{idList}")]
        SerializableTimeSeriesData ReadSelectCurrentTimeSeriesData(string idList);

        /// <summary>
        /// Reads current time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which current time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which current time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/data/read/current/{fromID}-{toID}")]
        SerializableTimeSeriesData ReadRangeCurrentTimeSeriesData(string fromID, string toID);

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/data/read/historic/{idList}/{startTime}/{endTime}")]
        SerializableTimeSeriesData ReadSelectHistoricTimeSeriesData(string idList, string startTime, string endTime);

        /// <summary>
        /// Reads historic time-series data from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which historic time-series data is to be read.</param>
        /// <param name="startTime">Start time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <param name="endTime">End time in <see cref="System.String"/> format of the timespan for which historic time-series data is to be read.</param>
        /// <returns>An <see cref="SerializableTimeSeriesData"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/data/read/historic/{fromID}-{toID}/{startTime}/{endTime}")]
        SerializableTimeSeriesData ReadRangeHistoricTimeSeriesData(string fromID, string toID, string startTime, string endTime);

        #endregion
    }
}
