using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using GSF.TimeSeries;
using GSF.TimeSeries.Transport;

namespace openVisN
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// Retrieves current meta-data from openHistorian using GEP
        /// </summary>
        private sealed class MetadataRetriever : IDisposable
        {
            #region [ Members ]

            // Fields
            private DataSubscriber m_subscriber;
            private DataSet m_metadata;
            private Exception m_processException;
            private ManualResetEventSlim m_waitHandle;
            private bool m_disposed;

            #endregion

            #region [ Constructors ]

            /// <summary>
            /// Creates a new <see cref="MetadataRetriever"/> instance with the specified <paramref name="connectionString"/>.
            /// </summary>
            /// <param name="connectionString">GEP connection string for openHistorian.</param>
            private MetadataRetriever(string connectionString)
            {
                m_subscriber = new DataSubscriber
                {
                    ConnectionString = connectionString,
                    ReceiveInternalMetadata = true,
                    ReceiveExternalMetadata = true,
                    OperationalModes = OperationalModes.UseCommonSerializationFormat | OperationalModes.CompressMetadata
                };

                // Attach to needed subscriber events
                m_subscriber.ProcessException += m_subscriber_ProcessException;
                m_subscriber.ConnectionEstablished += m_subscriber_ConnectionEstablished;
                m_subscriber.MetaDataReceived += m_subscriber_MetaDataReceived;

                // Initialize the subscriber
                m_subscriber.Initialize();

                // Create a wait handle to allow time to receive meta-data
                m_waitHandle = new ManualResetEventSlim();

                // Start subscriber connection cycle
                m_subscriber.Start();
            }

            /// <summary>
            /// Releases the unmanaged resources before the <see cref="MetadataRetriever"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~MetadataRetriever()
            {
                Dispose(false);
            }

            #endregion

            #region [ Methods ]

            /// <summary>
            /// Releases all the resources used by the <see cref="MetadataRetriever"/> object.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="MetadataRetriever"/> object and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
            private void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        // This will be done regardless of whether the object is finalized or disposed.

                        if (disposing)
                        {
                            if ((object)m_subscriber != null)
                            {
                                // Detach from subscriber events
                                m_subscriber.ProcessException -= m_subscriber_ProcessException;
                                m_subscriber.ConnectionEstablished -= m_subscriber_ConnectionEstablished;
                                m_subscriber.MetaDataReceived -= m_subscriber_MetaDataReceived;

                                m_subscriber.Dispose();
                                m_subscriber = null;
                            }

                            if ((object)m_waitHandle != null)
                            {
                                m_waitHandle.Set(); // Release any waiting threads
                                m_waitHandle.Dispose();
                                m_waitHandle = null;
                            }
                        }
                    }
                    finally
                    {
                        m_disposed = true;  // Prevent duplicate dispose.
                    }
                }
            }

            /// <summary>
            /// Gets meta-data using GEP waiting no longer than specified <paramref name="timeout"/>.
            /// </summary>
            /// <param name="timeout">Specifies how long to wait, in milliseconds, for meta-data.</param>
            /// <returns>The meta-data received from the GEP publisher in <see cref="DataSet"/> format.</returns>
            private DataSet GetMetadata(int timeout)
            {
                // Wait for meta-data or an exception to occur for up to the specified maximum time, then time out with an exception
                if (!m_waitHandle.Wait(timeout))
                    throw new TimeoutException(string.Format("Waited for {0} seconds for meta-data, but none was received.", timeout / 1000.0D));

                // If meta-data was received, return it
                if ((object)m_metadata != null)
                    return m_metadata;

                // If a processing exception occured, re-throw it
                if ((object)m_processException != null)
                    throw new InvalidOperationException(m_processException.Message, m_processException);

                // Otherwise return null (unlikely to ever get to this return)
                return null;
            }

            private void m_subscriber_ConnectionEstablished(object sender, EventArgs e)
            {
                // Request meta-data upon successful connection
                m_subscriber.SendServerCommand(ServerCommand.MetaDataRefresh);
            }

            private void m_subscriber_MetaDataReceived(object sender, GSF.EventArgs<DataSet> e)
            {
                m_metadata = e.Argument;

                // Release waiting threads if meta-data received
                m_waitHandle.Set();
            }

            private void m_subscriber_ProcessException(object sender, GSF.EventArgs<Exception> e)
            {
                m_processException = e.Argument;

                // Release waiting threads if an error occured
                m_waitHandle.Set();
            }

            #endregion

            #region [ Static ]

            /// <summary>
            /// Gets meta-data from the <paramref name="gepHost" /> waiting no longer than the specified <paramref name="timeout"/>.
            /// </summary>
            /// <param name="gepHost">GEP publication host and port, e.g., "192.168.1.1:6175", to connect to for meta-data.</param>
            /// <param name="timeout">Specifies how long to wait, in milliseconds, for meta-data.</param>
            /// <returns>The meta-data received from the GEP publisher in <see cref="DataSet"/> format.</returns>
            public static DataSet GetMetadata(string gepHost, int timeout = Timeout.Infinite)
            {
                using (MetadataRetriever retriever = new MetadataRetriever("server=" + gepHost))
                {
                    return retriever.GetMetadata(timeout);
                }
            }

            #endregion
        }

        public FrmMain()
        {
            InitializeComponent();

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

            try
            {
                DataSet metadata = MetadataRetriever.GetMetadata("localhost:6175");

                // Reference meta-data tables
                measurementTable = metadata.Tables["MeasurementDetail"];
                deviceTable = metadata.Tables["DeviceDetail"];
                phasorTable = metadata.Tables["PhasorDetail"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception retrieving meta-data: " + ex.Message);
            }

            if ((object)measurementTable != null)
            {
                // Could filter measurements if desired (e.g., no stats)
                DataRow[] measurements = measurementTable.Select("SignalAcronym <> ' STAT'");

                // Do something with measurement records
                foreach (DataRow measurement in measurements)
                {
                    Guid signalID;
                    MeasurementKey measurementKey;
                    string historianInstance;
                    uint pointID;
                    string signalType;

                    Guid.TryParse(measurement["SignalID"].ToString(), out signalID);
                    MeasurementKey.TryParse(measurement["ID"].ToString(), signalID, out measurementKey);

                    historianInstance = measurementKey.Source;
                    pointID = measurementKey.ID;

                    signalType = measurement["SignalAcronym"].ToString();
                }
            }
        }
    }
}
