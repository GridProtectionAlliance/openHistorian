//******************************************************************************************************
//  SerializableMetadataRecord.cs - Gbtc
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
//  08/18/2009 - Pinal C. Patel
//       Set the order of property serialization/deserialization for DataMember attribute.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  11/26/2009 - Pinal C. Patel
//       Removed Namespace from DataContract serialization.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using openHistorian.Files;

namespace openHistorian.DataServices
{
    /// <summary>
    /// Represents a flattened <see cref="MetadataRecord"/> that can be serialized using <see cref="XmlSerializer"/>, <see cref="DataContractSerializer"/> or <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>.
    /// </summary>
    /// <example>
    /// This is the output for <see cref="SerializableMetadataRecord"/> serialized using <see cref="XmlSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?> 
    /// <MetadataRecord HistorianID="1" DataType="0" Name="TVA_CORD-BUS2:ABBV" Synonym1="4-PM1" Synonym2="VPHM" Synonym3="" Description="Cordova ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude" HardwareInfo="ABB RES521" 
    ///   Remarks="" PlantCode="P1" UnitNumber="1" SystemName="CORD" SourceID="3" Enabled="true" ScanRate="0.0333333351" CompressionMinTime="0" CompressionMaxTime="0" EngineeringUnits="Volts" LowWarning="475000" HighWarning="525000" 
    ///   LowAlarm="450000" HighAlarm="550000" LowRange="475000" HighRange="525000" CompressionLimit="0" ExceptionLimit="0" DisplayDigits="7" SetDescription="" ClearDescription="" AlarmState="0" ChangeSecurity="5" AccessSecurity="0" 
    ///   StepCheck="false" AlarmEnabled="false" AlarmFlags="0" AlarmDelay="0" AlarmToFile="false" AlarmByEmail="false" AlarmByPager="false" AlarmByPhone="false" AlarmEmails="" AlarmPagers="" AlarmPhones="" 
    ///   xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" /> 
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableMetadataRecord"/> serialized using <see cref="DataContractSerializer"/>:
    /// <code>
    /// <![CDATA[
    /// <MetadataRecord xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
    ///   <HistorianID>1</HistorianID> 
    ///   <DataType>0</DataType> 
    ///   <Name>TVA_CORD-BUS2:ABBV</Name> 
    ///   <Synonym1>4-PM1</Synonym1> 
    ///   <Synonym2>VPHM</Synonym2> 
    ///   <Synonym3 /> 
    ///   <Description>Cordova ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude</Description> 
    ///   <HardwareInfo>ABB RES521</HardwareInfo> 
    ///   <Remarks /> 
    ///   <PlantCode>P1</PlantCode> 
    ///   <UnitNumber>1</UnitNumber> 
    ///   <SystemName>CORD</SystemName> 
    ///   <SourceID>3</SourceID> 
    ///   <Enabled>true</Enabled> 
    ///   <ScanRate>0.0333333351</ScanRate> 
    ///   <CompressionMinTime>0</CompressionMinTime> 
    ///   <CompressionMaxTime>0</CompressionMaxTime> 
    ///   <EngineeringUnits>Volts</EngineeringUnits> 
    ///   <LowWarning>475000</LowWarning> 
    ///   <HighWarning>525000</HighWarning> 
    ///   <LowAlarm>450000</LowAlarm> 
    ///   <HighAlarm>550000</HighAlarm> 
    ///   <LowRange>475000</LowRange> 
    ///   <HighRange>525000</HighRange> 
    ///   <CompressionLimit>0</CompressionLimit> 
    ///   <ExceptionLimit>0</ExceptionLimit> 
    ///   <DisplayDigits>7</DisplayDigits> 
    ///   <SetDescription /> 
    ///   <ClearDescription /> 
    ///   <AlarmState>0</AlarmState> 
    ///   <ChangeSecurity>5</ChangeSecurity> 
    ///   <AccessSecurity>0</AccessSecurity> 
    ///   <StepCheck>false</StepCheck> 
    ///   <AlarmEnabled>false</AlarmEnabled> 
    ///   <AlarmFlags>0</AlarmFlags> 
    ///   <AlarmDelay>0</AlarmDelay> 
    ///   <AlarmToFile>false</AlarmToFile> 
    ///   <AlarmByEmail>false</AlarmByEmail> 
    ///   <AlarmByPager>false</AlarmByPager> 
    ///   <AlarmByPhone>false</AlarmByPhone> 
    ///   <AlarmEmails /> 
    ///   <AlarmPagers /> 
    ///   <AlarmPhones /> 
    /// </MetadataRecord>
    /// ]]>
    /// </code>
    /// This is the output for <see cref="SerializableMetadataRecord"/> serialized using <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>:
    /// <code>
    /// {
    ///   "HistorianID":1,
    ///   "DataType":0,
    ///   "Name":"TVA_CORD-BUS2:ABBV",
    ///   "Synonym1":"4-PM1",
    ///   "Synonym2":"VPHM",
    ///   "Synonym3":"",
    ///   "Description":"Cordova ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude",
    ///   "HardwareInfo":"ABB RES521",
    ///   "Remarks":"",
    ///   "PlantCode":"P1",
    ///   "UnitNumber":1,
    ///   "SystemName":"CORD",
    ///   "SourceID":3,
    ///   "Enabled":true,
    ///   "ScanRate":0.0333333351,
    ///   "CompressionMinTime":0,
    ///   "CompressionMaxTime":0,
    ///   "EngineeringUnits":"Volts",
    ///   "LowWarning":475000,
    ///   "HighWarning":525000,
    ///   "LowAlarm":450000,
    ///   "HighAlarm":550000,
    ///   "LowRange":475000,
    ///   "HighRange":525000,
    ///   "CompressionLimit":0,
    ///   "ExceptionLimit":0,
    ///   "DisplayDigits":7,
    ///   "SetDescription":"",
    ///   "ClearDescription":"",
    ///   "AlarmState":0,
    ///   "ChangeSecurity":5,
    ///   "AccessSecurity":0,
    ///   "StepCheck":false,
    ///   "AlarmEnabled":false,
    ///   "AlarmFlags":0,
    ///   "AlarmDelay":0,
    ///   "AlarmToFile":false,
    ///   "AlarmByEmail":false,
    ///   "AlarmByPager":false,
    ///   "AlarmByPhone":false,
    ///   "AlarmEmails":"",
    ///   "AlarmPagers":"",
    ///   "AlarmPhones":""
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MetadataRecord"/>
    /// <seealso cref="XmlSerializer"/>
    /// <seealso cref="DataContractSerializer"/>
    /// <seealso cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>
    [XmlType("MetadataRecord"), DataContract(Name = "MetadataRecord", Namespace = "")]
    public class SerializableMetadataRecord
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMetadataRecord"/> class.
        /// </summary>
        public SerializableMetadataRecord()
        {
            Name = string.Empty;
            Synonym1 = string.Empty;
            Synonym2 = string.Empty;
            Synonym3 = string.Empty;
            Description = string.Empty;
            HardwareInfo = string.Empty;
            Remarks = string.Empty;
            PlantCode = string.Empty;
            SystemName = string.Empty;
            EngineeringUnits = string.Empty;
            SetDescription = string.Empty;
            ClearDescription = string.Empty;
            AlarmEmails = string.Empty;
            AlarmPagers = string.Empty;
            AlarmPhones = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableMetadataRecord"/> class.
        /// </summary>
        /// <param name="metadataRecord"><see cref="MetadataRecord"/> from which <see cref="SerializableMetadataRecord"/> is to be initialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadataRecord"/> is null.</exception>
        public SerializableMetadataRecord(MetadataRecord metadataRecord)
            : this()
        {
            if (metadataRecord == null)
                throw new ArgumentNullException("metadataRecord");

            HistorianID = metadataRecord.HistorianID;
            DataType = (int)metadataRecord.GeneralFlags.DataType;
            Name = metadataRecord.Name;
            Synonym1 = metadataRecord.Synonym1;
            Synonym2 = metadataRecord.Synonym2;
            Synonym3 = metadataRecord.Synonym3;
            Description = metadataRecord.Description;
            HardwareInfo = metadataRecord.HardwareInfo;
            Remarks = metadataRecord.Remarks;
            PlantCode = metadataRecord.PlantCode;
            UnitNumber = metadataRecord.UnitNumber;
            SystemName = metadataRecord.SystemName;
            SourceID = metadataRecord.SourceID;
            Enabled = metadataRecord.GeneralFlags.Enabled;
            ScanRate = metadataRecord.ScanRate;
            CompressionMinTime = metadataRecord.CompressionMinTime;
            CompressionMaxTime = metadataRecord.CompressionMaxTime;
            ChangeSecurity = metadataRecord.SecurityFlags.ChangeSecurity;
            AccessSecurity = metadataRecord.SecurityFlags.AccessSecurity;
            StepCheck = metadataRecord.GeneralFlags.StepCheck;
            AlarmEnabled = metadataRecord.GeneralFlags.AlarmEnabled;
            AlarmFlags = metadataRecord.AlarmFlags.Value;
            AlarmToFile = metadataRecord.GeneralFlags.AlarmToFile;
            AlarmByEmail = metadataRecord.GeneralFlags.AlarmByEmail;
            AlarmByPager = metadataRecord.GeneralFlags.AlarmByPager;
            AlarmByPhone = metadataRecord.GeneralFlags.AlarmByPhone;
            AlarmEmails = metadataRecord.AlarmEmails;
            AlarmPagers = metadataRecord.AlarmPagers;
            AlarmPhones = metadataRecord.AlarmPhones;
            if (DataType == 0)
            {
                // Analog properties.
                EngineeringUnits = metadataRecord.AnalogFields.EngineeringUnits;
                LowWarning = metadataRecord.AnalogFields.LowWarning;
                HighWarning = metadataRecord.AnalogFields.HighWarning;
                LowAlarm = metadataRecord.AnalogFields.LowAlarm;
                HighAlarm = metadataRecord.AnalogFields.HighAlarm;
                LowRange = metadataRecord.AnalogFields.LowRange;
                HighRange = metadataRecord.AnalogFields.HighRange;
                CompressionLimit = metadataRecord.AnalogFields.CompressionLimit;
                ExceptionLimit = metadataRecord.AnalogFields.ExceptionLimit;
                DisplayDigits = metadataRecord.AnalogFields.DisplayDigits;
                AlarmDelay = metadataRecord.AnalogFields.AlarmDelay;
            }
            else if (DataType == 1)
            {
                // Digital properties.
                SetDescription = metadataRecord.DigitalFields.SetDescription;
                ClearDescription = metadataRecord.DigitalFields.ClearDescription;
                AlarmState = metadataRecord.DigitalFields.AlarmState;
                AlarmDelay = metadataRecord.DigitalFields.AlarmDelay;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.HistorianID"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 0)]
        public int HistorianID { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.DataType"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 1)]
        public int DataType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.Name"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.Synonym1"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 3)]
        public string Synonym1 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.Synonym2"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 4)]
        public string Synonym2 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.Synonym3"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 5)]
        public string Synonym3 { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.Description"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 6)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.HardwareInfo"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 7)]
        public string HardwareInfo { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.Remarks"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 8)]
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.PlantCode"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 9)]
        public string PlantCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.UnitNumber"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 10)]
        public int UnitNumber { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.SystemName"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 11)]
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.SourceID"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 12)]
        public int SourceID { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.Enabled"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 13)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.ScanRate"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 14)]
        public float ScanRate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.CompressionMinTime"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 15)]
        public int CompressionMinTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.CompressionMaxTime"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 16)]
        public int CompressionMaxTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.EngineeringUnits"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 17)]
        public string EngineeringUnits { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.LowWarning"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 18)]
        public float LowWarning { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.HighWarning"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 19)]
        public float HighWarning { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.LowAlarm"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 20)]
        public float LowAlarm { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.HighAlarm"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 21)]
        public float HighAlarm { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.LowRange"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 22)]
        public float LowRange { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.HighRange"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 23)]
        public float HighRange { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.CompressionLimit"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 24)]
        public float CompressionLimit { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.ExceptionLimit"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 25)]
        public float ExceptionLimit { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.DisplayDigits"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 26)]
        public int DisplayDigits { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordDigitalFields.SetDescription"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 27)]
        public string SetDescription { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordDigitalFields.ClearDescription"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 28)]
        public string ClearDescription { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordDigitalFields.AlarmState"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 29)]
        public int AlarmState { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordSecurityFlags.ChangeSecurity"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 30)]
        public int ChangeSecurity { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordSecurityFlags.AccessSecurity"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 31)]
        public int AccessSecurity { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.StepCheck"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 32)]
        public bool StepCheck { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.AlarmEnabled"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 33)]
        public bool AlarmEnabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAlarmFlags.Value"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 33)]
        public int AlarmFlags { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordAnalogFields.AlarmDelay"/> or <see cref="MetadataRecordDigitalFields.AlarmDelay"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 34)]
        public float AlarmDelay { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.AlarmToFile"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 35)]
        public bool AlarmToFile { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.AlarmByEmail"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 36)]
        public bool AlarmByEmail { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.AlarmByPager"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 37)]
        public bool AlarmByPager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecordGeneralFlags.AlarmByPhone"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 38)]
        public bool AlarmByPhone { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.AlarmEmails"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 39)]
        public string AlarmEmails { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.AlarmPagers"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 40)]
        public string AlarmPagers { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MetadataRecord.AlarmPhones"/>.
        /// </summary>
        [XmlAttribute(), DataMember(Order = 41)]
        public string AlarmPhones { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns an <see cref="MetadataRecord"/> object for this <see cref="SerializableMetadataRecord"/>.
        /// </summary>
        /// <returns>An <see cref="MetadataRecord"/> object.</returns>
        public MetadataRecord Deflate()
        {
            MetadataRecord metadataRecord = new MetadataRecord(HistorianID);
            metadataRecord.GeneralFlags.DataType = (DataType)DataType;
            metadataRecord.Name = Name;
            metadataRecord.Synonym1 = Synonym1;
            metadataRecord.Synonym2 = Synonym2;
            metadataRecord.Synonym3 = Synonym3;
            metadataRecord.Description = Description;
            metadataRecord.HardwareInfo = HardwareInfo;
            metadataRecord.Remarks = Remarks;
            metadataRecord.PlantCode = PlantCode;
            metadataRecord.UnitNumber = UnitNumber;
            metadataRecord.SystemName = SystemName;
            metadataRecord.SourceID = SourceID;
            metadataRecord.GeneralFlags.Enabled = Enabled;
            metadataRecord.ScanRate = ScanRate;
            metadataRecord.CompressionMinTime = CompressionMinTime;
            metadataRecord.CompressionMaxTime = CompressionMaxTime;
            metadataRecord.SecurityFlags.ChangeSecurity = ChangeSecurity;
            metadataRecord.SecurityFlags.AccessSecurity = AccessSecurity;
            metadataRecord.GeneralFlags.StepCheck = StepCheck;
            metadataRecord.GeneralFlags.AlarmEnabled = AlarmEnabled;
            metadataRecord.AlarmFlags.Value = AlarmFlags;
            metadataRecord.GeneralFlags.AlarmToFile = AlarmToFile;
            metadataRecord.GeneralFlags.AlarmByEmail = AlarmByEmail;
            metadataRecord.GeneralFlags.AlarmByPager = AlarmByPager;
            metadataRecord.GeneralFlags.AlarmByPhone = AlarmByPhone;
            metadataRecord.AlarmEmails = AlarmEmails;
            metadataRecord.AlarmPagers = AlarmPagers;
            metadataRecord.AlarmPhones = AlarmPhones;
            if (DataType == 0)
            {
                // Analog properties.
                metadataRecord.AnalogFields.EngineeringUnits = EngineeringUnits;
                metadataRecord.AnalogFields.LowWarning = LowWarning;
                metadataRecord.AnalogFields.HighWarning = HighWarning;
                metadataRecord.AnalogFields.LowAlarm = LowAlarm;
                metadataRecord.AnalogFields.HighAlarm = HighAlarm;
                metadataRecord.AnalogFields.LowRange = LowRange;
                metadataRecord.AnalogFields.HighRange = HighRange;
                metadataRecord.AnalogFields.CompressionLimit = CompressionLimit;
                metadataRecord.AnalogFields.ExceptionLimit = ExceptionLimit;
                metadataRecord.AnalogFields.DisplayDigits = DisplayDigits;
                metadataRecord.AnalogFields.AlarmDelay = AlarmDelay;
            }
            else if (DataType == 1)
            {
                // Digital properties.
                metadataRecord.DigitalFields.SetDescription = SetDescription;
                metadataRecord.DigitalFields.ClearDescription = ClearDescription;
                metadataRecord.DigitalFields.AlarmState = AlarmState;
                metadataRecord.DigitalFields.AlarmDelay = AlarmDelay;
            }

            return metadataRecord;
        }

        #endregion
    }
}
