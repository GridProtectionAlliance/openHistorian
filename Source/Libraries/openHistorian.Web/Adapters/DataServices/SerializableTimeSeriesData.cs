//******************************************************************************************************
//  SerializableTimeSeriesData.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  11/26/2009 - Pinal C. Patel
//       Removed Namespace from DataContract serialization.
//  12/17/2009 - Pinal C. Patel
//       Updated the serialization definition for XmlSerializer.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace openHistorian.DataServices
{
    /// <summary>
    /// Represents a container for <see cref="SerializableTimeSeriesDataPoint"/>s that can be serialized using <see cref="XmlSerializer"/> or <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>.
    /// </summary>
    /// <example>
    /// This is the output for <see cref="SerializableTimeSeriesData"/> serialized using <see cref="XmlSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?> 
    /// <TimeSeriesData xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///   <TimeSeriesDataPoints>
    ///     <TimeSeriesDataPoint Key="1" Time="21-Aug-2009 14:29:52.634" Value="59.9537773" Quality="Good" /> 
    ///     <TimeSeriesDataPoint Key="2" Time="21-Aug-2009 14:29:52.668" Value="60.0351028" Quality="Good" /> 
    ///     <TimeSeriesDataPoint Key="3" Time="21-Aug-2009 14:29:52.702" Value="59.99268" Quality="Good" /> 
    ///     <TimeSeriesDataPoint Key="4" Time="21-Aug-2009 14:29:52.736" Value="59.99003" Quality="Good" /> 
    ///     <TimeSeriesDataPoint Key="5" Time="21-Aug-2009 14:29:52.770" Value="59.9532661" Quality="Good" /> 
    ///   </TimeSeriesDataPoints>
    /// </TimeSeriesData>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableTimeSeriesData"/> serialized using <see cref="DataContractSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <TimeSeriesData xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    ///   <TimeSeriesDataPoints>
    ///     <TimeSeriesDataPoint>
    ///       <Key>1</Key> 
    ///       <Time>21-Aug-2009 14:31:56.176</Time> 
    ///       <Value>60.0272522</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <Key>2</Key> 
    ///       <Time>21-Aug-2009 14:31:56.210</Time> 
    ///       <Value>60.0283241</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <Key>3</Key> 
    ///       <Time>21-Aug-2009 14:31:56.244</Time> 
    ///       <Value>60.0418167</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <Key>4</Key> 
    ///       <Time>21-Aug-2009 14:31:56.278</Time> 
    ///       <Value>60.0049438</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <Key>5</Key> 
    ///       <Time>21-Aug-2009 14:31:56.312</Time> 
    ///       <Value>59.9982834</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///   </TimeSeriesDataPoints>
    /// </TimeSeriesData>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableTimeSeriesData"/> serialized using <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>:
    /// <code>
    /// {
    ///   "TimeSeriesDataPoints":
    ///     [{"Key":1,
    ///       "Time":"21-Aug-2009 14:37:04.804",
    ///       "Value":59.9637527,
    ///       "Quality":29},
    ///      {"Key":2,
    ///       "Time":"21-Aug-2009 14:37:04.838",
    ///       "Value":60.0154762,
    ///       "Quality":29},
    ///      {"Key":3,
    ///       "Time":"21-Aug-2009 14:37:04.872",
    ///       "Value":59.977684,
    ///       "Quality":29},
    ///      {"Key":3,
    ///       "Time":"21-Aug-2009 14:37:04.906",
    ///       "Value":59.97335,
    ///       "Quality":29},
    ///      {"Key":5,
    ///       "Time":"21-Aug-2009 14:37:04.940",
    ///       "Value":59.974678,
    ///       "Quality":29}]
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="SerializableTimeSeriesDataPoint"/>
    /// <seealso cref="XmlSerializer"/>
    /// <seealso cref="DataContractSerializer"/>
    /// <seealso cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>
    [XmlType("TimeSeriesData"), DataContract(Name = "TimeSeriesData", Namespace = "")]
    public class SerializableTimeSeriesData
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSeriesData"/> class.
        /// </summary>
        public SerializableTimeSeriesData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSeriesData"/> class.
        /// </summary>
        /// <param name="data">List of <see cref="IData"/> from which <see cref="SerializableTimeSeriesData"/> is to be initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        public SerializableTimeSeriesData(IEnumerable<IData> data)
            : this()
        {
            if (data == null)
                throw new ArgumentNullException("data");

            List<SerializableTimeSeriesDataPoint> serializableDataPoints = new List<SerializableTimeSeriesDataPoint>();
            foreach (IData item in data)
            {
                serializableDataPoints.Add(new SerializableTimeSeriesDataPoint(item));
            }
            TimeSeriesDataPoints = serializableDataPoints.ToArray();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="SerializableTimeSeriesDataPoint"/>s contained in the <see cref="SerializableTimeSeriesData"/>.
        /// </summary>
        [XmlArray(), DataMember()]
        public SerializableTimeSeriesDataPoint[] TimeSeriesDataPoints { get; set; }

        #endregion
    }
}
