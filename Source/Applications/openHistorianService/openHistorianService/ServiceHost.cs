//******************************************************************************************************
//  ServiceHost.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  04/05/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.ServiceProcess;
using System.Threading.Tasks;
using openHistorian.Adapters;
using openHistorian.Archives;
using TVA;
using TVA.Adapters;

namespace openHistorianService
{
    public partial class ServiceHost : ServiceBase
    {
        #region [ Members ]

        // Fields
        DataArchives m_dataArchives;
        DataAdapters m_dataAdapters;

        #endregion

        #region [ Constructors ]

        public ServiceHost()
            : base()
        {
            InitializeComponent();

            // Register event handlers.
            m_serviceHelper.ServiceStarted += ServiceHelper_ServiceStarted;
            m_serviceHelper.ServiceStopping += ServiceHelper_ServiceStopping;
        }

        public ServiceHost(IContainer container)
            : this()
        {
            if (container != null)
                container.Add(this);
        }

        #endregion

        #region [ Methods ]

        private void HeartbeatProcess(string processName, object[] processArguments)
        {
        }

        private void ServiceHelper_ServiceStarted(object sender, EventArgs e)
        {
            // Load all data archives.
            m_dataArchives = new DataArchives();
            m_dataArchives.AdapterDirectory = "Configuration";
            m_dataArchives.AdapterFileFormat = AdapterFileFormat.SerializedXml;
            m_dataArchives.AdapterFileExtension = "*.dataarchive";
            m_dataArchives.PersistSettings = true;
            m_dataArchives.AdapterLoaded += DataArchives_AdapterLoaded;
            m_dataArchives.AdapterUnloaded += DataArchives_AdapterUnloaded;
            m_dataArchives.AdapterLoadException += DataArchives_AdapterLoadException;
            m_dataArchives.Initialize();

            // Load all data adapters.
            m_dataAdapters = new DataAdapters();
            m_dataAdapters.AdapterDirectory = "Configuration";
            m_dataAdapters.AdapterFileFormat = AdapterFileFormat.SerializedXml;
            m_dataAdapters.AdapterFileExtension = "*.dataadapter";
            m_dataAdapters.PersistSettings = true;
            m_dataAdapters.AdapterCreated += DataAdapters_AdapterCreated;
            m_dataAdapters.AdapterLoaded += DataAdapters_AdapterLoaded;
            m_dataAdapters.AdapterUnloaded += DataAdapters_AdapterUnloaded;
            m_dataAdapters.AdapterLoadException += DataAdapter_AdapterLoadException;
            m_dataAdapters.Initialize();

            // Add components to service helper.
            m_serviceHelper.ServiceComponents.Add(m_dataArchives);
            m_serviceHelper.ServiceComponents.Add(m_dataAdapters);

            // Add and schedule service processes.
            m_serviceHelper.AddScheduledProcess(HeartbeatProcess, "HeartbeatProcess", "* * * * *");
        }

        private void ServiceHelper_ServiceStopping(object sender, EventArgs e)
        {
            // Dispose all adapters.
            m_dataAdapters.Dispose();

            // Dispose all archives.
            m_dataArchives.Dispose();
        }

        private void DataArchives_AdapterLoaded(object sender, EventArgs<IDataArchive> e)
        {
            // Subscribe to data archive events.
            IDataArchive archive = e.Argument;
            archive.StatusUpdate += Adapters_StatusUpdate;
            archive.ExecutionException += Adapters_ExecutionException;
            
            // Notify about the newly loaded data archive.
            m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Data archive \"{0}\" of type \"{1}\" loaded\r\n\r\n", archive.Name, archive.GetType().Name);

            // Open the archive asynchronously.
            Task.Factory.StartNew(() => 
            {
                try
                {
                    m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Opening archive \"{0}\"\r\n\r\n", archive.Name);
                    archive.Open(true);
                    m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Archive \"{0}\" opened\r\n\r\n", archive.Name);
                }
                catch (Exception ex)
                {
                    m_serviceHelper.ErrorLogger.Log(ex);
                    m_serviceHelper.UpdateStatus(UpdateType.Alarm, "[SYSTEM] Error opening archive \"{0}\": {1}\r\n\r\n", archive.Name, ex.Message);
                }
            });
        }

        private void DataArchives_AdapterUnloaded(object sender, EventArgs<IDataArchive> e)
        {
            // Notify about the data archive that was unloaded.
            if (!RemotingServices.IsTransparentProxy(e.Argument))
                m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Data archive \"{0}\" of type \"{1}\" unloaded\r\n\r\n", e.Argument.Name, e.Argument.GetType().Name);
        }

        private void DataArchives_AdapterLoadException(object sender, EventArgs<Exception> e)
        {
            // Log the error.
            m_serviceHelper.ErrorLogger.Log(e.Argument);
            // Display the error message.
            m_serviceHelper.UpdateStatus(UpdateType.Alarm, "[SYSTEM] Data archive load error - {0}\r\n\r\n", e.Argument.Message);
        }

        private void DataAdapters_AdapterCreated(object sender, EventArgs<IDataAdapter> e)
        {
            e.Argument.Archives = new ReadOnlyObservableCollection<IDataArchive>((ObservableCollection<IDataArchive>)m_dataArchives.Adapters);
        }

        private void DataAdapters_AdapterLoaded(object sender, EventArgs<IDataAdapter> e)
        {
            // Subscribe to data adapter events.
            IDataAdapter adapter = e.Argument;
            adapter.StatusUpdate += Adapters_StatusUpdate;
            adapter.ExecutionException += Adapters_ExecutionException;

            // Notify about the newly loaded data adapter.
            m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Data adapter \"{0}\" of type \"{1}\" loaded\r\n\r\n", adapter.Name, adapter.GetType().Name);

            // Start the adapter asynchronously.
            Task.Factory.StartNew(() =>
            {
                try
                {
                    m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Starting adapter \"{0}\"\r\n\r\n", adapter.Name);
                    adapter.Start();
                    m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Adapter \"{0}\" started\r\n\r\n", adapter.Name);
                }
                catch (Exception ex)
                {
                    m_serviceHelper.ErrorLogger.Log(ex);
                    m_serviceHelper.UpdateStatus(UpdateType.Alarm, "[SYSTEM] Error starting adapter \"{0}\": {1}\r\n\r\n", adapter.Name, ex.Message);
                }
            });
        }

        private void DataAdapters_AdapterUnloaded(object sender, EventArgs<IDataAdapter> e)
        {
            // Notify about the data adapter that was unloaded.
            if (!RemotingServices.IsTransparentProxy(e.Argument))
                m_serviceHelper.UpdateStatus(UpdateType.Information, "[SYSTEM] Data adapter \"{0}\" of type \"{1}\" unloaded\r\n\r\n", e.Argument.Name, e.Argument.GetType().Name);
        }

        private void DataAdapter_AdapterLoadException(object sender, EventArgs<Exception> e)
        {
            // Log the error.
            m_serviceHelper.ErrorLogger.Log(e.Argument);
            // Display the error message.
            m_serviceHelper.UpdateStatus(UpdateType.Alarm, "[SYSTEM] Data adapter load error - {0}\r\n\r\n", e.Argument.Message);
        }

        private void Adapters_StatusUpdate(object sender, EventArgs<UpdateType, string> e)
        {
            // Display status updates from data archive and data adapters.
            m_serviceHelper.UpdateStatus(e.Argument1, string.Format("[{0}] {1}\r\n\r\n", (sender as IAdapter).Name.ToUpper(), e.Argument2));
        }

        private void Adapters_ExecutionException(object sender, EventArgs<string, Exception> e)
        {
            // Log error from data archives and data adapters.
            m_serviceHelper.ErrorLogger.Log(e.Argument2);
            // Display error message from data archives and data adapters.
            m_serviceHelper.UpdateStatus(UpdateType.Alarm, string.Format("[{0}] {1} - {2}\r\n\r\n", (sender as IAdapter).Name.ToUpper(), e.Argument1, e.Argument2.Message));
        }

        #endregion
    }
}
