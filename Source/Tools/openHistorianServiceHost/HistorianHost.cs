using System;
using System.IO;
using System.Linq;
using openHistorian.Net;

namespace openHistorianServiceHost
{
    internal class HistorianHost : IDisposable
    {
        private readonly HistorianServer m_server;

        public HistorianHost()
        {
            Directory.GetFiles(@"G:\HistorianData\", "*.d2").ToList().ForEach(File.Delete);
            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", @"G:\HistorianData\", true);
            m_server = new HistorianServer(settings);
        }

        private bool m_disposed;

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="HistorianHost"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~HistorianHost()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="HistorianHost"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianHost"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    m_server.Dispose();
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true; // Prevent duplicate dispose.
                }
            }
        }
    }
}