//using System;
//using System.Collections.Generic;
//using System.Linq;
//using openHistorian.Adapters;
//using openHistorian.Archives;
//using openHistorian.DataServices;
//using TVA;

//namespace openHistorian.Web.Adapters
//{
//    public class WebServiceOutputAdapter : DataAdapterBase
//    {
//        #region [ Members ]

//        // Constants

//        // Fields
//        private TimeSeriesDataService m_dataService;
//        private MetadataService m_definitionService;
//        private IdentifiableItem<string, IDataArchive> m_archive;
//        private bool m_disposed;
//        private bool m_initialized;

//        #endregion

//        #region [ Constructors ]

//        public WebServiceOutputAdapter()
//        {
//            m_dataService = new TimeSeriesDataService();
//            m_definitionService = new MetadataService();
//        }

//        #endregion

//        #region [ Properties ]

//        #endregion

//        #region [ Methods ]

//        public override void Initialize()
//        {
//            if (!m_initialized)
//            {
//                // Find the target archive for data.
//                IDictionary<string, IDataArchive> archives = FindSourceArchives(1, 1);
//                m_archive = new IdentifiableItem<string, IDataArchive>(archives.First().Key, archives.First().Value);

//                m_dataService.Archive = m_archive.Item;
//                m_dataService.PersistSettings = false;
//                m_dataService.ServiceProcessException += Service_ServiceProcessException;
//                m_dataService.Initialize();

//                m_definitionService.Archive = m_archive.Item;
//                m_definitionService.PersistSettings = false;
//                m_definitionService.ServiceProcessException += Service_ServiceProcessException;
//                m_definitionService.Initialize();

//                m_initialized = true;
//            }
//        }

//        public override void Start()
//        {
//        }

//        public override void Stop()
//        {
//        }

//        protected override void OnArchiveAdded(IDataArchive archive)
//        {
//            if (string.Compare(m_archive.ID, archive.Name) == 0)
//            {
//                // Save reference to the added archive.
//                m_archive.Item = archive;
//                OnStatusUpdate(UpdateType.Information, "Saved reference to \"{0}\"", m_archive.ID);
//            }
//        }

//        protected override void OnArchiveRemoved(IDataArchive archive)
//        {
//            if (string.Compare(m_archive.ID, archive.Name) == 0)
//            {
//                // Remove reference of the removed archive.
//                m_archive = null;
//                OnStatusUpdate(UpdateType.Information, "Removed reference to \"{0}\"", m_archive.ID);
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (!m_disposed)
//            {
//                try
//                {
//                    // This will be done regardless of whether the object is finalized or disposed.
//                    if (disposing)
//                    {
//                        // This will be done only when the object is disposed by calling Dispose().
//                        Stop();

//                        if (m_dataService != null)
//                        {
//                            m_dataService.ServiceProcessException -= Service_ServiceProcessException;
//                            m_dataService.Dispose();
//                        }

//                        if (m_definitionService != null)
//                        {
//                            m_definitionService.ServiceProcessException -= Service_ServiceProcessException;
//                            m_definitionService.Dispose();
//                        }                            
//                    }
//                }
//                finally
//                {
//                    m_disposed = true;          // Prevent duplicate dispose.
//                    base.Dispose(disposing);    // Call base class Dispose().
//                }
//            }
//        }

//        private void Service_ServiceProcessException(object sender, EventArgs<Exception> e)
//        {
//            OnExecutionException("Error processing request", e.Argument);
//        }

//        #endregion
//    }
//}
