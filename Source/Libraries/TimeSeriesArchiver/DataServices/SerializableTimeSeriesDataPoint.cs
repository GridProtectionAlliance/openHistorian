//******************************************************************************************************
//  SerializableTimeSeriesDataPoint.cs - Gbtc
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
//  12/02/2009 - Pinal C. Patel
//       Changed the default format of Time to be cross-culture compatible.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TimeSeriesArchiver.Files;

namespace TimeSeriesArchiver.DataServices
{
    /// <summary>
    /// Represents a time-series data-point that can be serialized using <see cref="XmlSerializer"/>, <see cref="DataContractSerializer"/> or <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>.
    /// </summary>
    /// <example>
    /// This is the output for <see cref="SerializableTimeSeriesDataPoint"/> serialized using <see cref="XmlSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0"?>
    /// <TimeSeriesDataPoint xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
    ///   HistorianID="1" Time="2009-08-21 14:21:23.236" Value="60.0419579" Quality="Good" />
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableTimeSeriesDataPoint"/> serialized using <see cref="DataContractSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <TimeSeriesDataPoint xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    ///   <HistorianID>1</HistorianID>
    ///   <Time>2009-08-21 14:21:54.612</Time>
    ///   <Value>60.025547</Value>
    ///   <Quality>Good</Quality>
    /// </TimeSeriesDataPoint>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableTimeSeriesDataPoint"/> serialized using <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>:
    /// <code>
    /// {
    ///   "HistorianID":1,
    ///   "Time":"2009-08-21 14:22:26.971",
    ///   "Value":59.9974136,
    ///   "Quality":29
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="IDataPoint"/>
    /// <seealso cref="XmlSerializer"/>
    /// <seealso cref="DataContractSerializer"/>
    /// <seealso cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>
    [XmlType("TimeSeriesDataPoint"), DataContract(Name = "TimeSeriesDataPoint", Namespace = "")]
    public class SerializableTimeSeriesDataPoint
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSeriesDataPoint"/> class.
        /// </summary>
        public SerializableTimeSeriesDataPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableTimeSeriesDataPoint"/> class.
        /// </summary>
        /// <param name="dataPoint"><see cref="IDataPoint"/> from which <see cref="SerializableTimeSeriesDataPoint"/> is to be initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dataPoint"/> is null.</exception>
        public SerializableTimeSeriesDataPoint(IDataPoint dataPoint)
        {
            if (dataPoint == null)
                throw new ArgumentNullException("dataPoint");

            HistorianID = dataPoint.HistorianID;
            Time = dataPoint.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Value = dataPoint.Value;
            Quality = dataPoint.Quality;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IDataPoint.HistorianID"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 0)]
        public int HistorianID { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="String"/> representation of <see cref="IDataPoint.Time"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 1)]
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDataPoint.Value"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 2)]
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDataPoint.Quality"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 3)]
        public Quality Quality { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns an <see cref="IDataPoint"/> object for this <see cref="SerializableTimeSeriesDataPoint"/>.
        /// </summary>
        /// <returns>An <see cref="IDataPoint"/> object.</returns>
        public IDataPoint Deflate()
        {
            // TODO: Eliminate the need for this by modifying ArchiveFile to use IDataPoint internally.
            return new ArchiveDataPoint(HistorianID, TimeTag.Parse(Time), Value, Quality);
        }

        #endregion
    }
}
