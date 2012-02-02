//******************************************************************************************************
//  SerializableTimeSeriesDataPoint.cs - Gbtc
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
//  12/02/2009 - Pinal C. Patel
//       Changed the default format of Time to be cross-culture compatible.
//
//******************************************************************************************************

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using openHistorian.Archives;

namespace openHistorian.DataServices
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
    ///   Key="1" Time="2009-08-21 14:21:23.236" Value="60.0419579" Quality="Good" />
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableTimeSeriesDataPoint"/> serialized using <see cref="DataContractSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <TimeSeriesDataPoint xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    ///   <Key>1</Key>
    ///   <Time>2009-08-21 14:21:54.612</Time>
    ///   <Value>60.025547</Value>
    ///   <Quality>Good</Quality>
    /// </TimeSeriesDataPoint>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableTimeSeriesDataPoint"/> serialized using <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>:
    /// <code>
    /// {
    ///   "Key":1,
    ///   "Time":"2009-08-21 14:22:26.971",
    ///   "Value":59.9974136,
    ///   "Quality":29
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="IData"/>
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
        /// <param name="data"><see cref="IData"/> from which <see cref="SerializableTimeSeriesDataPoint"/> is to be initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        public SerializableTimeSeriesDataPoint(IData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Key = data.Key;
            Time = data.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Value = data.Value;
            Quality = data.Quality;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IData.Key"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 0)]
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="String"/> representation of <see cref="IData.Time"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 1)]
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IData.Value"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 2)]
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IData.Quality"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 3)]
        public Quality Quality { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns an <see cref="IData"/> object for this <see cref="SerializableTimeSeriesDataPoint"/>.
        /// </summary>
        /// <returns>An <see cref="IData"/> object.</returns>
        public IData Deflate()
        {
            // TODO: Eliminate the need for this by modifying ArchiveFile to use IDataPoint internally.
            return new ArchiveData(Key, TimeTag.Parse(Time), Value, Quality);
        }

        #endregion
    }
}
