//******************************************************************************************************
//  MetadataUpdater.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/07/2009 - Pinal C. Patel
//       Generated original version of source code.
//  08/21/2009 - Pinal C. Patel
//       Removed ExtractMetadata() method as this can be achived using Serialization.Serialize() method.
//       Modified UpdateMetadata() overload for processing web service data to use Serialization class.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/16/2009 - Pinal C. Patel
//       Modified UpdateMetadata() overloads to save the metadata file upon update.
//  12/18/2009 - Pinal C. Patel
//       Updated UpdateMetadata() overloads to check for DBNull values.
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using openHistorian.Archives.V1;
using openHistorian.DataServices;

namespace openHistorian.Metadata
{
    /// <summary>
    /// A class that can update data in a <see cref="MetadataFile"/>.
    /// </summary>
    /// <seealso cref="MetadataFile"/>
    public class MetadataUpdater
    {
        #region [ Members ]

        // Fields
        private MetadataFile m_metadata;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataUpdater"/> class.
        /// </summary>
        /// <param name="metadata"><see cref="MetadataFile"/> that is to be updated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is null</exception>
        public MetadataUpdater(MetadataFile metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            m_metadata = metadata;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="MetadataFile"/> to be updated.
        /// </summary>
        public MetadataFile Metadata 
        {
            get
            {
                return m_metadata;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates the <see cref="Metadata"/> from <paramref name="tableData"/>
        /// </summary>
        /// <param name="tableData"><see cref="DataTable"/> containing the new metadata.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableData"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="tableData"/> does not contain 43 columns.</exception>
        public void UpdateMetadata(DataTable tableData)
        {
            if (tableData == null)
                throw new ArgumentNullException("tableData");

            if (tableData.Rows[0].ItemArray.Length != 43)
                throw new ArgumentException("tableData must contain 43 columns");

            // Column 00: Key
            // Column 01: DataType
            // Column 02: Name
            // Column 03: Synonym1
            // Column 04: Synonym2
            // Column 05: Synonym3
            // Column 06: Description
            // Column 07: HardwareInfo
            // Column 08: Remarks
            // Column 09: PlantCode
            // Column 10: UnitNumber
            // Column 11: SystemName
            // Column 12: SourceID
            // Column 13: Enabled
            // Column 14: ScanRate
            // Column 15: CompressionMinTime
            // Column 16: CompressionMaxTime
            // Column 17: EngineeringUnits
            // Column 18: LowWarning
            // Column 19: HighWarning
            // Column 20: LowAlarm
            // Column 21: HighAlarm
            // Column 22: LowRange
            // Column 23: HighRange
            // Column 24: CompressionLimit
            // Column 25: ExceptionLimit
            // Column 26: DisplayDigits
            // Column 27: SetDescription
            // Column 28: ClearDescription
            // Column 29: AlarmState
            // Column 30: ChangeSecurity
            // Column 31: AccessSecurity
            // Column 32: StepCheck
            // Column 33: AlarmEnabled
            // Column 34: AlarmFlags
            // Column 35: AlarmDelay
            // Column 36: AlarmToFile
            // Column 37: AlarmByEmail
            // Column 38: AlarmByPager
            // Column 39: AlarmByPhone
            // Column 40: AlarmEmails
            // Column 41: AlarmPagers
            // Column 42: AlarmPhones
            MetadataRecord metadataRecord;
            foreach (DataRow row in tableData.Rows)
            {
                metadataRecord = new MetadataRecord(Convert.ToInt32(row[0]));
                if (!Convert.IsDBNull(row[1]))
                    metadataRecord.GeneralFlags.DataType = (DataType)Convert.ToInt32(row[1]);
                if (!Convert.IsDBNull(row[2]))
                    metadataRecord.Name = Convert.ToString(row[2]);
                if (!Convert.IsDBNull(row[3]))
                    metadataRecord.Synonym1 = Convert.ToString(row[3]);
                if (!Convert.IsDBNull(row[4]))
                    metadataRecord.Synonym2 = Convert.ToString(row[4]);
                if (!Convert.IsDBNull(row[5]))
                    metadataRecord.Synonym3 = Convert.ToString(row[5]);
                if (!Convert.IsDBNull(row[6]))
                    metadataRecord.Description = Convert.ToString(row[6]);
                if (!Convert.IsDBNull(row[7]))
                    metadataRecord.HardwareInfo = Convert.ToString(row[7]);
                if (!Convert.IsDBNull(row[8]))
                    metadataRecord.Remarks = Convert.ToString(row[8]);
                if (!Convert.IsDBNull(row[9]))
                    metadataRecord.PlantCode = Convert.ToString(row[9]);
                if (!Convert.IsDBNull(row[10]))
                    metadataRecord.UnitNumber = Convert.ToInt32(row[10]);
                if (!Convert.IsDBNull(row[11]))
                    metadataRecord.SystemName = Convert.ToString(row[11]);
                if (!Convert.IsDBNull(row[12]))
                    metadataRecord.SourceID = Convert.ToInt32(row[12]);
                if (!Convert.IsDBNull(row[13]))
                    metadataRecord.GeneralFlags.Enabled = Convert.ToBoolean(row[13]);
                if (!Convert.IsDBNull(row[14]))
                    metadataRecord.ScanRate = Convert.ToSingle(row[14]);
                if (!Convert.IsDBNull(row[15]))
                    metadataRecord.CompressionMinTime = Convert.ToInt32(row[15]);
                if (!Convert.IsDBNull(row[16]))
                    metadataRecord.CompressionMaxTime = Convert.ToInt32(row[16]);
                if (!Convert.IsDBNull(row[30]))
                    metadataRecord.SecurityFlags.ChangeSecurity = Convert.ToInt32(row[30]);
                if (!Convert.IsDBNull(row[31]))
                    metadataRecord.SecurityFlags.AccessSecurity = Convert.ToInt32(row[31]);
                if (!Convert.IsDBNull(row[32]))
                    metadataRecord.GeneralFlags.StepCheck = Convert.ToBoolean(row[32]);
                if (!Convert.IsDBNull(row[33]))
                    metadataRecord.GeneralFlags.AlarmEnabled = Convert.ToBoolean(row[33]);
                if (!Convert.IsDBNull(row[34]))
                    metadataRecord.AlarmFlags.Value = Convert.ToInt32(row[34]);
                if (!Convert.IsDBNull(row[36]))
                    metadataRecord.GeneralFlags.AlarmToFile = Convert.ToBoolean(row[36]);
                if (!Convert.IsDBNull(row[37]))
                    metadataRecord.GeneralFlags.AlarmByEmail = Convert.ToBoolean(row[37]);
                if (!Convert.IsDBNull(row[38]))
                    metadataRecord.GeneralFlags.AlarmByPager = Convert.ToBoolean(row[38]);
                if (!Convert.IsDBNull(row[39]))
                    metadataRecord.GeneralFlags.AlarmByPhone = Convert.ToBoolean(row[39]);
                if (!Convert.IsDBNull(row[40]))
                    metadataRecord.AlarmEmails = Convert.ToString(row[40]);
                if (!Convert.IsDBNull(row[41]))
                    metadataRecord.AlarmPagers = Convert.ToString(row[41]);
                if (!Convert.IsDBNull(row[42]))
                    metadataRecord.AlarmPhones = Convert.ToString(row[42]);
                if (metadataRecord.GeneralFlags.DataType == DataType.Analog)
                {
                    if (!Convert.IsDBNull(row[17]))
                        metadataRecord.AnalogFields.EngineeringUnits = Convert.ToString(row[17]);
                    if (!Convert.IsDBNull(row[18]))
                        metadataRecord.AnalogFields.LowWarning = Convert.ToSingle(row[18]);
                    if (!Convert.IsDBNull(row[19]))
                        metadataRecord.AnalogFields.HighWarning = Convert.ToSingle(row[19]);
                    if (!Convert.IsDBNull(row[20]))
                        metadataRecord.AnalogFields.LowAlarm = Convert.ToSingle(row[20]);
                    if (!Convert.IsDBNull(row[21]))
                        metadataRecord.AnalogFields.HighAlarm = Convert.ToSingle(row[21]);
                    if (!Convert.IsDBNull(row[22]))
                        metadataRecord.AnalogFields.LowRange = Convert.ToSingle(row[22]);
                    if (!Convert.IsDBNull(row[23]))
                        metadataRecord.AnalogFields.HighRange = Convert.ToSingle(row[23]);
                    if (!Convert.IsDBNull(row[24]))
                        metadataRecord.AnalogFields.CompressionLimit = Convert.ToSingle(row[24]);
                    if (!Convert.IsDBNull(row[25]))
                        metadataRecord.AnalogFields.ExceptionLimit = Convert.ToSingle(row[25]);
                    if (!Convert.IsDBNull(row[26]))
                        metadataRecord.AnalogFields.DisplayDigits = Convert.ToInt32(row[26]);
                    if (!Convert.IsDBNull(row[35]))
                        metadataRecord.AnalogFields.AlarmDelay = Convert.ToSingle(row[35]);
                }
                else if (metadataRecord.GeneralFlags.DataType == DataType.Digital)
                {
                    if (!Convert.IsDBNull(row[27]))
                        metadataRecord.DigitalFields.SetDescription = Convert.ToString(row[27]);
                    if (!Convert.IsDBNull(row[28]))
                        metadataRecord.DigitalFields.ClearDescription = Convert.ToString(row[28]);
                    if (!Convert.IsDBNull(row[29]))
                        metadataRecord.DigitalFields.AlarmState = Convert.ToInt32(row[29]);
                    if (!Convert.IsDBNull(row[35]))
                        metadataRecord.DigitalFields.AlarmDelay = Convert.ToSingle(row[35]);
                }

                m_metadata.Write(metadataRecord.Key, metadataRecord);
            }
            m_metadata.Save();
        }

        /// <summary>
        /// Updates the <see cref="Metadata"/> from <paramref name="readerData"/>
        /// </summary>
        /// <param name="readerData"><see cref="IDataReader"/> providing the new metadata.</param>
        /// <exception cref="ArgumentNullException"><paramref name="readerData"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="readerData"/> does not contain 43 columns.</exception>
        public void UpdateMetadata(IDataReader readerData)
        {
            if (readerData == null)
                throw new ArgumentNullException("readerData");

            if (readerData.FieldCount != 43)
                throw new ArgumentException("readerData must contain 43 columns");

            MetadataRecord metadataRecord;
            while (readerData.Read())
            {
                metadataRecord = new MetadataRecord(Convert.ToInt32(readerData[0]));
                if (!Convert.IsDBNull(readerData[1]))
                    metadataRecord.GeneralFlags.DataType = (DataType)Convert.ToInt32(readerData[1]);
                if (!Convert.IsDBNull(readerData[2]))
                    metadataRecord.Name = Convert.ToString(readerData[2]);
                if (!Convert.IsDBNull(readerData[3]))
                    metadataRecord.Synonym1 = Convert.ToString(readerData[3]);
                if (!Convert.IsDBNull(readerData[4]))
                    metadataRecord.Synonym2 = Convert.ToString(readerData[4]);
                if (!Convert.IsDBNull(readerData[5]))
                    metadataRecord.Synonym3 = Convert.ToString(readerData[5]);
                if (!Convert.IsDBNull(readerData[6]))
                    metadataRecord.Description = Convert.ToString(readerData[6]);
                if (!Convert.IsDBNull(readerData[7]))
                    metadataRecord.HardwareInfo = Convert.ToString(readerData[7]);
                if (!Convert.IsDBNull(readerData[8]))
                    metadataRecord.Remarks = Convert.ToString(readerData[8]);
                if (!Convert.IsDBNull(readerData[9]))
                    metadataRecord.PlantCode = Convert.ToString(readerData[9]);
                if (!Convert.IsDBNull(readerData[10]))
                    metadataRecord.UnitNumber = Convert.ToInt32(readerData[10]);
                if (!Convert.IsDBNull(readerData[11]))
                    metadataRecord.SystemName = Convert.ToString(readerData[11]);
                if (!Convert.IsDBNull(readerData[12]))
                    metadataRecord.SourceID = Convert.ToInt32(readerData[12]);
                if (!Convert.IsDBNull(readerData[13]))
                    metadataRecord.GeneralFlags.Enabled = Convert.ToBoolean(readerData[13]);
                if (!Convert.IsDBNull(readerData[14]))
                    metadataRecord.ScanRate = Convert.ToSingle(readerData[14]);
                if (!Convert.IsDBNull(readerData[15]))
                    metadataRecord.CompressionMinTime = Convert.ToInt32(readerData[15]);
                if (!Convert.IsDBNull(readerData[16]))
                    metadataRecord.CompressionMaxTime = Convert.ToInt32(readerData[16]);
                if (!Convert.IsDBNull(readerData[30]))
                    metadataRecord.SecurityFlags.ChangeSecurity = Convert.ToInt32(readerData[30]);
                if (!Convert.IsDBNull(readerData[31]))
                    metadataRecord.SecurityFlags.AccessSecurity = Convert.ToInt32(readerData[31]);
                if (!Convert.IsDBNull(readerData[32]))
                    metadataRecord.GeneralFlags.StepCheck = Convert.ToBoolean(readerData[32]);
                if (!Convert.IsDBNull(readerData[33]))
                    metadataRecord.GeneralFlags.AlarmEnabled = Convert.ToBoolean(readerData[33]);
                if (!Convert.IsDBNull(readerData[34]))
                    metadataRecord.AlarmFlags.Value = Convert.ToInt32(readerData[34]);
                if (!Convert.IsDBNull(readerData[36]))
                    metadataRecord.GeneralFlags.AlarmToFile = Convert.ToBoolean(readerData[36]);
                if (!Convert.IsDBNull(readerData[37]))
                    metadataRecord.GeneralFlags.AlarmByEmail = Convert.ToBoolean(readerData[37]);
                if (!Convert.IsDBNull(readerData[38]))
                    metadataRecord.GeneralFlags.AlarmByPager = Convert.ToBoolean(readerData[38]);
                if (!Convert.IsDBNull(readerData[39]))
                    metadataRecord.GeneralFlags.AlarmByPhone = Convert.ToBoolean(readerData[39]);
                if (!Convert.IsDBNull(readerData[40]))
                    metadataRecord.AlarmEmails = Convert.ToString(readerData[40]);
                if (!Convert.IsDBNull(readerData[41]))
                    metadataRecord.AlarmPagers = Convert.ToString(readerData[41]);
                if (!Convert.IsDBNull(readerData[42]))
                    metadataRecord.AlarmPhones = Convert.ToString(readerData[42]);
                if (metadataRecord.GeneralFlags.DataType == DataType.Analog)
                {
                    if (!Convert.IsDBNull(readerData[17]))
                        metadataRecord.AnalogFields.EngineeringUnits = Convert.ToString(readerData[17]);
                    if (!Convert.IsDBNull(readerData[18]))
                        metadataRecord.AnalogFields.LowWarning = Convert.ToSingle(readerData[18]);
                    if (!Convert.IsDBNull(readerData[19]))
                        metadataRecord.AnalogFields.HighWarning = Convert.ToSingle(readerData[19]);
                    if (!Convert.IsDBNull(readerData[20]))
                        metadataRecord.AnalogFields.LowAlarm = Convert.ToSingle(readerData[20]);
                    if (!Convert.IsDBNull(readerData[21]))
                        metadataRecord.AnalogFields.HighAlarm = Convert.ToSingle(readerData[21]);
                    if (!Convert.IsDBNull(readerData[22]))
                        metadataRecord.AnalogFields.LowRange = Convert.ToSingle(readerData[22]);
                    if (!Convert.IsDBNull(readerData[23]))
                        metadataRecord.AnalogFields.HighRange = Convert.ToSingle(readerData[23]);
                    if (!Convert.IsDBNull(readerData[24]))
                        metadataRecord.AnalogFields.CompressionLimit = Convert.ToSingle(readerData[24]);
                    if (!Convert.IsDBNull(readerData[25]))
                        metadataRecord.AnalogFields.ExceptionLimit = Convert.ToSingle(readerData[25]);
                    if (!Convert.IsDBNull(readerData[26]))
                        metadataRecord.AnalogFields.DisplayDigits = Convert.ToInt32(readerData[26]);
                    if (!Convert.IsDBNull(readerData[35]))
                        metadataRecord.AnalogFields.AlarmDelay = Convert.ToSingle(readerData[35]);
                }
                else if (metadataRecord.GeneralFlags.DataType == DataType.Digital)
                {
                    if (!Convert.IsDBNull(readerData[27]))
                        metadataRecord.DigitalFields.SetDescription = Convert.ToString(readerData[27]);
                    if (!Convert.IsDBNull(readerData[28]))
                        metadataRecord.DigitalFields.ClearDescription = Convert.ToString(readerData[28]);
                    if (!Convert.IsDBNull(readerData[29]))
                        metadataRecord.DigitalFields.AlarmState = Convert.ToInt32(readerData[29]);
                    if (!Convert.IsDBNull(readerData[35]))
                        metadataRecord.DigitalFields.AlarmDelay = Convert.ToSingle(readerData[35]);
                }

                m_metadata.Write(metadataRecord.Key, metadataRecord);
            }
            m_metadata.Save();
        }

        /// <summary>
        /// Updates the <see cref="Metadata"/> from <paramref name="streamData"/>
        /// </summary>
        /// <param name="streamData"><see cref="Stream"/> containing serialized <see cref="SerializableMetadata"/>.</param>
        /// <param name="dataFormat"><see cref="TVA.SerializationFormat"/> in which the <see cref="SerializableMetadata"/> was serialized to <paramref name="streamData"/>.</param>
        public void UpdateMetadata(Stream streamData, TVA.SerializationFormat dataFormat)
        {
            // Deserialize serialized metadata.
            SerializableMetadata deserializedMetadata = TVA.Serialization.Deserialize<SerializableMetadata>(streamData, dataFormat);

            // Update metadata from the deserialized metadata.
            foreach (SerializableMetadataRecord deserializedMetadataRecord in deserializedMetadata.MetadataRecords)
            {
                m_metadata.Write(deserializedMetadataRecord.Key, deserializedMetadataRecord.Deflate());
            }
            m_metadata.Save();
        }

        #endregion       
    }
}
