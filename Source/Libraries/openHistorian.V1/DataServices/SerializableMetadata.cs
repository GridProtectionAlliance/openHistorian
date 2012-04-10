//******************************************************************************************************
//  SerializableMetadata.cs - Gbtc
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
//  08/07/2009 - Pinal C. Patel
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
using openHistorian.V1.Files;

namespace openHistorian.V1.DataServices
{
    /// <summary>
    /// Represents a container for <see cref="SerializableMetadataRecord"/>s that can be serialized using <see cref="XmlSerializer"/> or <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>.
    /// </summary>
    /// <example>
    /// This is the output for <see cref="SerializableMetadata"/> serialized using <see cref="XmlSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?> 
    /// <Metadata xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///   <MetadataRecords>
    ///     <MetadataRecord HistorianID="1" DataType="0" Name="TVA_CORD-BUS2:ABBV" Synonym1="4-PM1" Synonym2="VPHM" Synonym3="" 
    ///       Description="Cordova ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude" HardwareInfo="ABB RES521" Remarks="" 
    ///       PlantCode="P1" UnitNumber="1" SystemName="CORD" SourceID="3" Enabled="true" ScanRate="0.0333333351" CompressionMinTime="0" 
    ///       CompressionMaxTime="0" EngineeringUnits="Volts" LowWarning="475000" HighWarning="525000" LowAlarm="450000" HighAlarm="550000" 
    ///       LowRange="475000" HighRange="525000" CompressionLimit="0" ExceptionLimit="0" DisplayDigits="7" SetDescription="" ClearDescription="" 
    ///       AlarmState="0" ChangeSecurity="5" AccessSecurity="0" StepCheck="false" AlarmEnabled="false" AlarmFlags="0" AlarmDelay="0" AlarmToFile="false" 
    ///       AlarmByEmail="false" AlarmByPager="false" AlarmByPhone="false" AlarmEmails="" AlarmPagers="" AlarmPhones="" /> 
    ///   </MetadataRecords>
    /// </Metadata>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableMetadata"/> serialized using <see cref="DataContractSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <Metadata xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    ///   <MetadataRecords>
    ///     <MetadataRecord>
    ///       <HistorianID>1</HistorianID> 
    ///       <DataType>0</DataType> 
    ///       <Name>TVA_CORD-BUS2:ABBV</Name> 
    ///       <Synonym1>4-PM1</Synonym1> 
    ///       <Synonym2>VPHM</Synonym2> 
    ///       <Synonym3 /> 
    ///       <Description>Cordova ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude</Description> 
    ///       <HardwareInfo>ABB RES521</HardwareInfo> 
    ///       <Remarks /> 
    ///       <PlantCode>P1</PlantCode> 
    ///       <UnitNumber>1</UnitNumber> 
    ///       <SystemName>CORD</SystemName> 
    ///       <SourceID>3</SourceID> 
    ///       <Enabled>true</Enabled> 
    ///       <ScanRate>0.0333333351</ScanRate> 
    ///       <CompressionMinTime>0</CompressionMinTime> 
    ///       <CompressionMaxTime>0</CompressionMaxTime> 
    ///       <EngineeringUnits>Volts</EngineeringUnits> 
    ///       <LowWarning>475000</LowWarning> 
    ///       <HighWarning>525000</HighWarning> 
    ///       <LowAlarm>450000</LowAlarm> 
    ///       <HighAlarm>550000</HighAlarm> 
    ///       <LowRange>475000</LowRange> 
    ///       <HighRange>525000</HighRange> 
    ///       <CompressionLimit>0</CompressionLimit> 
    ///       <ExceptionLimit>0</ExceptionLimit> 
    ///       <DisplayDigits>7</DisplayDigits> 
    ///       <SetDescription /> 
    ///       <ClearDescription /> 
    ///       <AlarmState>0</AlarmState> 
    ///       <ChangeSecurity>5</ChangeSecurity> 
    ///       <AccessSecurity>0</AccessSecurity> 
    ///       <StepCheck>false</StepCheck> 
    ///       <AlarmEnabled>false</AlarmEnabled> 
    ///       <AlarmFlags>0</AlarmFlags> 
    ///       <AlarmDelay>0</AlarmDelay> 
    ///       <AlarmToFile>false</AlarmToFile> 
    ///       <AlarmByEmail>false</AlarmByEmail> 
    ///       <AlarmByPager>false</AlarmByPager> 
    ///       <AlarmByPhone>false</AlarmByPhone> 
    ///       <AlarmEmails /> 
    ///       <AlarmPagers /> 
    ///       <AlarmPhones /> 
    ///     </MetadataRecord>
    ///   </MetadataRecords>
    /// </Metadata>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableMetadata"/> serialized using <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>:
    /// <code>
    /// {
    ///   "MetadataRecords":
    ///     [{"HistorianID":1,
    ///       "DataType":0,
    ///       "Name":"TVA_CORD-BUS2:ABBV",
    ///       "Synonym1":"4-PM1",
    ///       "Synonym2":"VPHM",
    ///       "Synonym3":"",
    ///       "Description":"Cordova ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude",
    ///       "HardwareInfo":"ABB RES521",
    ///       "Remarks":"",
    ///       "PlantCode":"P1",
    ///       "UnitNumber":1,
    ///       "SystemName":"CORD",
    ///       "SourceID":3,
    ///       "Enabled":true,
    ///       "ScanRate":0.0333333351,
    ///       "CompressionMinTime":0,
    ///       "CompressionMaxTime":0,
    ///       "EngineeringUnits":"Volts",
    ///       "LowWarning":475000,
    ///       "HighWarning":525000,
    ///       "LowAlarm":450000,
    ///       "HighAlarm":550000,
    ///       "LowRange":475000,
    ///       "HighRange":525000,
    ///       "CompressionLimit":0,
    ///       "ExceptionLimit":0,
    ///       "DisplayDigits":7,
    ///       "SetDescription":"",
    ///       "ClearDescription":"",
    ///       "AlarmState":0,
    ///       "ChangeSecurity":5,
    ///       "AccessSecurity":0,
    ///       "StepCheck":false,
    ///       "AlarmEnabled":false,
    ///       "AlarmFlags":0,
    ///       "AlarmDelay":0,
    ///       "AlarmToFile":false,
    ///       "AlarmByEmail":false,
    ///       "AlarmByPager":false,
    ///       "AlarmByPhone":false,
    ///       "AlarmEmails":"",
    ///       "AlarmPagers":"",
    ///       "AlarmPhones":""}]
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MetadataFile"/>
    /// <seealso cref="SerializableMetadataRecord"/>
    /// <seealso cref="XmlSerializer"/>
    /// <seealso cref="DataContractSerializer"/>
    /// <seealso cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>
    [XmlType("Metadata"), DataContract(Name = "Metadata", Namespace = "")]
    public class SerializableMetadata
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMetadata"/> class.
        /// </summary>
        public SerializableMetadata()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMetadata"/> class.
        /// </summary>
        /// <param name="metadataFile"><see cref="MetadataFile"/> object from which <see cref="SerializableMetadata"/> is to be initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadataFile"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="metadataFile"/> is closed.</exception>
        public SerializableMetadata(MetadataFile metadataFile)
            : this()
        {
            if (metadataFile == null)
                throw new ArgumentNullException("metadataFile");

            if (!metadataFile.IsOpen)
                throw new ArgumentException("metadataFile is closed");

            // Process all records in the metadata file.
            List<SerializableMetadataRecord> serializableMetadataRecords = new List<SerializableMetadataRecord>();
            foreach (MetadataRecord metadataRecord in metadataFile.Read())
            {
                serializableMetadataRecords.Add(new SerializableMetadataRecord(metadataRecord));
            }
            MetadataRecords = serializableMetadataRecords.ToArray();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="SerializableMetadataRecord"/>s contained in the <see cref="SerializableMetadata"/>.
        /// </summary>
        [XmlArray(), DataMember()]
        public SerializableMetadataRecord[] MetadataRecords
        {
            get;
            set;
        }

        #endregion
    }
}
