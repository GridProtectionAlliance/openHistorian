//******************************************************************************************************
//  SerializableTimeSeriesData.cs - Gbtc
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
//  08/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  11/26/2009 - Pinal C. Patel
//       Removed Namespace from DataContract serialization.
//  12/17/2009 - Pinal C. Patel
//       Updated the serialization definition for XmlSerializer.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
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
    ///     <TimeSeriesDataPoint HistorianID="1" Time="21-Aug-2009 14:29:52.634" Value="59.9537773" Quality="Good" /> 
    ///     <TimeSeriesDataPoint HistorianID="2" Time="21-Aug-2009 14:29:52.668" Value="60.0351028" Quality="Good" /> 
    ///     <TimeSeriesDataPoint HistorianID="3" Time="21-Aug-2009 14:29:52.702" Value="59.99268" Quality="Good" /> 
    ///     <TimeSeriesDataPoint HistorianID="4" Time="21-Aug-2009 14:29:52.736" Value="59.99003" Quality="Good" /> 
    ///     <TimeSeriesDataPoint HistorianID="5" Time="21-Aug-2009 14:29:52.770" Value="59.9532661" Quality="Good" /> 
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
    ///       <HistorianID>1</HistorianID> 
    ///       <Time>21-Aug-2009 14:31:56.176</Time> 
    ///       <Value>60.0272522</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <HistorianID>2</HistorianID> 
    ///       <Time>21-Aug-2009 14:31:56.210</Time> 
    ///       <Value>60.0283241</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <HistorianID>3</HistorianID> 
    ///       <Time>21-Aug-2009 14:31:56.244</Time> 
    ///       <Value>60.0418167</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <HistorianID>4</HistorianID> 
    ///       <Time>21-Aug-2009 14:31:56.278</Time> 
    ///       <Value>60.0049438</Value> 
    ///       <Quality>Good</Quality> 
    ///     </TimeSeriesDataPoint>
    ///     <TimeSeriesDataPoint>
    ///       <HistorianID>5</HistorianID> 
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
    ///     [{"HistorianID":1,
    ///       "Time":"21-Aug-2009 14:37:04.804",
    ///       "Value":59.9637527,
    ///       "Quality":29},
    ///      {"HistorianID":2,
    ///       "Time":"21-Aug-2009 14:37:04.838",
    ///       "Value":60.0154762,
    ///       "Quality":29},
    ///      {"HistorianID":3,
    ///       "Time":"21-Aug-2009 14:37:04.872",
    ///       "Value":59.977684,
    ///       "Quality":29},
    ///      {"HistorianID":3,
    ///       "Time":"21-Aug-2009 14:37:04.906",
    ///       "Value":59.97335,
    ///       "Quality":29},
    ///      {"HistorianID":5,
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
        /// <param name="dataPoints">List of <see cref="IDataPoint"/> from which <see cref="SerializableTimeSeriesData"/> is to be initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dataPoints"/> is null.</exception>
        public SerializableTimeSeriesData(IEnumerable<IDataPoint> dataPoints)
            : this()
        {
            if (dataPoints == null)
                throw new ArgumentNullException("dataPoints");

            List<SerializableTimeSeriesDataPoint> serializableDataPoints = new List<SerializableTimeSeriesDataPoint>();
            foreach (IDataPoint dataPoint in dataPoints)
            {
                serializableDataPoints.Add(new SerializableTimeSeriesDataPoint(dataPoint));
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
