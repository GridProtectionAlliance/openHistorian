using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DataExtractionUtility.Properties;
using GSF.TimeSeries;
using openVisN;

namespace DataExtractionUtility
{
    public class MeasurementRow
    {
        public long PointID;
        public Guid SignalID;
        public string DeviceName;
        public string SignalAcronym;
        public string Description;

        public MeasurementRow(DataRow row)
        {
            MeasurementKey.TryParse(row["ID"].ToString(), out MeasurementKey measurementKey);

            PointID = unchecked((long)measurementKey.ID);
            DeviceName = row["DeviceAcronym"].ToString();
            SignalAcronym = row["SignalAcronym"].ToString();
            Description = row["Description"].ToString();
        }
    }

    public class MetaData
    {
        public List<MeasurementRow> Measurements;

        public MetaData()
        {
            Measurements = new List<MeasurementRow>();

            // Do the following on button click or missing configuration, etc:

            // Note that openHistorian internal publisher controls how many tables / fields to send as meta-data to subscribers (user controllable),
            // as a result, not all fields in associated database views will be available. Below are the default SELECT filters the publisher
            // will apply to the "MeasurementDetail", "DeviceDetail" and "PhasorDetail" database views:

            // SELECT NodeID, UniqueID, OriginalSource, IsConcentrator, Acronym, Name, ParentAcronym, ProtocolName, FramesPerSecond, Enabled FROM DeviceDetail WHERE IsConcentrator = 0
            // SELECT Internal, DeviceAcronym, DeviceName, SignalAcronym, ID, SignalID, PointTag, SignalReference, Description, Enabled FROM MeasurementDetail
            // SELECT DeviceAcronym, Label, Type, Phase, SourceIndex FROM PhasorDetail

            DataTable measurementTable = null;
            DataTable deviceTable = null;
            DataTable phasorTable = null;

            string server = "Server=" + Settings.Default.ServerIP + "; Port=" + Settings.Default.HistorianGatewayPort + "; Interface=0.0.0.0";

            try
            {
                DataSet metadata = MetadataRetriever.GetMetadata(server);

                // Reference meta-data tables
                measurementTable = metadata.Tables["MeasurementDetail"];
                deviceTable = metadata.Tables["DeviceDetail"];
                phasorTable = metadata.Tables["PhasorDetail"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception retrieving meta-data: " + ex.Message);
            }

            if (measurementTable != null)
            {
                // Do something with measurement records
                foreach (DataRow measurement in measurementTable.Select("SignalAcronym <> 'STAT' and SignalAcronym <> 'DIGI'"))
                {
                    Measurements.Add(new MeasurementRow(measurement));
                }
            }
        }
    }
}
