//******************************************************************************************************
//  Export.cs - Gbtc
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
//  06/12/2007 - Pinal C. Patel
//       Original version of source code generated.
//  06/05/2008 - Pinal C. Patel
//       Renamed property LastProcessingTime to LastProcessTime.
//       Added new properties LastProcessResult, LastProcessError and LastProcessTimestamp.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  08/05/2009 - Josh L. Patterson
//       Edited Comments.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated new header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TVA.Units;

namespace openHistorian.V1.Exporters
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates the processing frequency of an <see cref="Export"/>.
    /// </summary>
    public enum ExportType
    {
        /// <summary>
        /// <see cref="Export"/> is to be processed only when a request is made to process it.
        /// </summary>
        Manual,
        /// <summary>
        /// <see cref="Export"/> is to be processed as time-series data is received in real-time.
        /// </summary>
        RealTime,
        /// <summary>
        /// <see cref="Export"/> is to be processed at a set interval regardless of change in the time-series data.
        /// </summary>
        Intervaled
    }

    /// <summary>
    /// Indicates the processing result of an <see cref="Export"/>.
    /// </summary>
    public enum ExportProcessResult
    {
        /// <summary>
        /// <see cref="Export"/> has not been processed yet.
        /// </summary>
        Unknown,
        /// <summary>
        /// <see cref="Export"/> was processed successfully.
        /// </summary>
        Success,
        /// <summary>
        /// <see cref="Export"/> failed to process successfully.
        /// </summary>
        Failure
    }

    #endregion

    /// <summary>
    /// A class with information that can be used by an exporter for exporting time-series data.
    /// </summary>
    /// <seealso cref="ExportRecord"/>
    /// <seealso cref="ExportSetting"/>
    [Serializable()]
    public class Export
    {
        #region [ Members ]

        // Fields
        private string m_name;
        private ExportType m_type;
        private double m_interval;
        private string m_exporter;
        [NonSerialized()]
        private ExportProcessResult m_lastProcessResult;
        [NonSerialized()]
        private Exception m_lastProcessError;
        [NonSerialized()]
        private Time m_lastProcessTime;
        [NonSerialized()]
        private DateTime m_lastProcessTimestamp;
        private List<ExportSetting> m_settings;
        private List<ExportRecord> m_records;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Export"/> class.
        /// </summary>
        /// <seealso cref="ExportRecord"/>
        /// <seealso cref="ExportSetting"/>
        public Export()
        {
            m_name = "Export";
            m_exporter = "Exporter";
            m_settings = new List<ExportSetting>();
            m_records = new List<ExportRecord>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the <see cref="Export"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ExportType"/> of the <see cref="Export"/>.
        /// </summary>
        public ExportType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        /// <summary>
        /// Gets or sets the interval (in seconds) at which the <see cref="Export"/> is to be processed if its <see cref="Type"/> is <see cref="ExportType.Intervaled"/>.
        /// </summary>
        public double Interval
        {
            get
            {
                return m_interval;
            }
            set
            {
                m_interval = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the exporter (any <see cref="Type"/> that implements the <see cref="IExporter"/> interface) responsible for processing the export.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string Exporter
        {
            get
            {
                return m_exporter;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_exporter = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ExportProcessResult"/> of the <see cref="Export"/> when it was last processed.
        /// </summary>
        [XmlIgnore()]
        public ExportProcessResult LastProcessResult
        {
            get
            {
                return m_lastProcessResult;
            }
            set
            {
                m_lastProcessResult = value;
            }
        }

        /// <summary>
        /// Gets or sets any <see cref="Exception"/> encountered when the <see cref="Export"/> was last processed.
        /// </summary>
        [XmlIgnore()]
        public Exception LastProcessError
        {
            get
            {
                return m_lastProcessError;
            }
            set
            {
                m_lastProcessError = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Time"/> it took to process the <see cref="Export"/> when it was last processed.
        /// </summary>
        /// <remarks>
        /// <see cref="LastProcessTime"/> will be zero if the <see cref="Export.Type"/> is <see cref="ExportType.RealTime"/>.
        /// </remarks>
        [XmlIgnore()]
        public Time LastProcessTime
        {
            get
            {
                return m_lastProcessTime;
            }
            set
            {
                m_lastProcessTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> of when the <see cref="Export"/> was last processed.
        /// </summary>
        /// <remarks>
        /// <see cref="LastProcessTimestamp"/> will be <see cref="DateTime.MinValue"/> if the <see cref="Export.Type"/> is <see cref="ExportType.RealTime"/>.
        /// </remarks>
        [XmlIgnore()]
        public DateTime LastProcessTimestamp
        {
            get
            {
                return m_lastProcessTimestamp;
            }
            set
            {
                m_lastProcessTimestamp = value;
            }
        }

        /// <summary>
        /// Gets the custom <see cref="ExportSetting"/>s used by the <see cref="Exporter"/> of the <see cref="Export"/>.
        /// </summary>
        public virtual List<ExportSetting> Settings
        {
            get
            {
                return m_settings;
            }
        }

        /// <summary>
        /// Gets the <see cref="ExportRecord"/>s whose time-series data is to be exported by the <see cref="Exporter"/>.
        /// </summary>
        public virtual List<ExportRecord> Records
        {
            get
            {
                return m_records;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns the <see cref="ExportSetting"/> for the specified <paramref name="settingName"/> from the <see cref="Settings"/>.
        /// </summary>
        /// <param name="settingName"><see cref="ExportSetting.Name"/> of the <see cref="ExportSetting"/> to be retrieved.</param>
        /// <returns>An <see cref="ExportSetting"/> object if a match is found; otherwise null.</returns>
        public ExportSetting FindSetting(string settingName)
        {
            return m_settings.Find(setting => (string.Compare(setting.Name, settingName, true) == 0));
        }

        /// <summary>
        /// Returns the <see cref="ExportRecord"/> for the specified <paramref name="instance"/> and <paramref name="identifier"/> from the <see cref="Records"/>.
        /// </summary>
        /// <param name="instance"><see cref="ExportRecord.Instance"/> name of the <see cref="ExportRecord"/> to be retrieved.</param>
        /// <param name="identifier"><see cref="ExportRecord.Identifier"/> of the <see cref="ExportRecord"/> to be retrieved.</param>
        /// <returns>An <see cref="ExportRecord"/> object if a match is found; otherwise null.</returns>
        public ExportRecord FindRecord(string instance, int identifier)
        {
            return m_records.Find(record => (string.Compare(record.Instance, instance, true) == 0 && record.Identifier == identifier));
        }

        /// <summary>
        /// Returns the <see cref="ExportRecord"/>s for the specified <paramref name="instance"/> from the <see cref="Records"/>.
        /// </summary>
        /// <param name="instance"><see cref="ExportRecord.Instance"/> name of the <see cref="ExportRecord"/>s to be retrieved.</param>
        /// <returns>An <see cref="List{T}"/> object containing matching <see cref="ExportRecord"/>s.</returns>
        public IList<ExportRecord> FindRecords(string instance)
        {
            return m_records.FindAll(record => (string.Compare(record.Instance, instance, true) == 0));
        }

        /// <summary>
        /// Determines if it is time to process the <see cref="Export"/> if its <see cref="Type"/> is <see cref="ExportType.Intervaled"/>.
        /// </summary>
        /// <returns><see cref="Boolean"/> indicating whether it is time to process the <see cref="Export"/>.</returns>
        public bool ShouldProcess()
        {
            DateTime currentTime = DateTime.Now;
            if (m_type == ExportType.Intervaled &&
                m_interval > 0 &&
                currentTime.Subtract(m_lastProcessTimestamp).TotalSeconds >= m_interval)
            {
                m_lastProcessTimestamp = currentTime;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the current <see cref="Export"/> object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="Export"/> object is to be compared for equality.</param>
        /// <returns>true if the current <see cref="Export"/> object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            Export other = obj as Export;
            if (other == null)
                return false;
            else
                // We'll compare name since *Name* is considered as the "ID" for *Export*.
                return (string.Compare(m_name, other.Name, true) == 0);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Export"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        #endregion
    }
}