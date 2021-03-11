using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using GSF.TimeSeries;

namespace ComparisonUtility
{
    public class Metadata
    {
        public ulong PointID;
        public Guid SignalID;
        public string PointTag;
        public string SignalReference;
        public string DeviceName;
        public string SignalAcronym;
        public string Description;

        private Metadata(DataRow row)
        {
            Guid.TryParse(row["SignalID"].ToString(), out SignalID);
            MeasurementKey.TryParse(row["ID"].ToString(), out MeasurementKey measurementKey);

            PointID = measurementKey.ID;
            PointTag = row["PointTag"].ToString();
            SignalReference = row["SignalReference"].ToString();
            DeviceName = row["DeviceAcronym"].ToString();
            SignalAcronym = row["SignalAcronym"].ToString();
            Description = row["Description"].ToString();
        }

        public static List<Metadata> Query(string host, int port, int timeout = -1)
        {
            List<Metadata> measurements = new List<Metadata>();

            // Note that openHistorian internal publisher controls how many tables / fields to send as meta-data to subscribers (user controllable),
            // as a result, not all fields in associated database views will be available. Below are the default SELECT filters the publisher
            // will apply to the "MeasurementDetail", "DeviceDetail" and "PhasorDetail" database views:

            // SELECT NodeID, UniqueID, OriginalSource, IsConcentrator, Acronym, Name, AccessID, ParentAcronym, ProtocolName, FramesPerSecond, CompanyAcronym, VendorAcronym, VendorDeviceName, Longitude, Latitude, InterconnectionName, ContactList, Enabled, UpdatedOn FROM DeviceDetail WHERE IsConcentrator = 0
            // SELECT DeviceAcronym, ID, SignalID, PointTag, SignalReference, SignalAcronym, PhasorSourceIndex, Description, Internal, Enabled, UpdatedOn FROM MeasurementDetail
            // SELECT DeviceAcronym, Label, Type, Phase, SourceIndex, UpdatedOn FROM PhasorDetail
            // SELECT VersionNumber FROM SchemaVersion

            DataTable measurementTable = null;
            //DataTable deviceTable = null;
            //DataTable phasorTable = null;

            string connectionString = $"server={host}:{port}; interface=0.0.0.0";

            try
            {
                DataSet metadata = MetadataRetriever.GetMetadata(connectionString, timeout);

                // Reference meta-data tables
                measurementTable = metadata.Tables["MeasurementDetail"];
                //deviceTable = metadata.Tables["DeviceDetail"];
                //phasorTable = metadata.Tables["PhasorDetail"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception retrieving meta-data: " + ex.Message);
            }

            if (measurementTable != null)
            {
                // Do something with measurement records
                foreach (DataRow measurement in measurementTable.Select("SignalAcronym <> 'STAT'"))
                {
                    measurements.Add(new Metadata(measurement));
                }
            }

            return measurements;
        }
    }
}
