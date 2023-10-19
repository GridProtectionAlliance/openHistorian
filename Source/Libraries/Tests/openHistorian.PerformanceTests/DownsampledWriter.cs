using System;
using GSF.Diagnostics;
using GSF.Snap.Services;
using GSF.Units;
using NUnit.Framework;
using openHistorian.Net;

namespace openHistorian.PerformanceTests
{
    // SnapDB Engine Code
    internal class SnapDBEngine : IDisposable
    {
        private readonly Action<string> m_logMessage;
        private HistorianServer m_server;
        private LogSubscriber m_logSubscriber;
        private bool m_disposed;

        public SnapDBEngine(Action<string> logMessage, string instanceName, string destinationFilesLocation, string targetFileSize = null, string directoryNamingMethod = null, bool readOnly = false)
        {
            m_logMessage = logMessage;

            m_logSubscriber = Logger.CreateSubscriber(VerboseLevel.High);
            m_logSubscriber.NewLogMessage += m_logSubscriber_Log;

            if (string.IsNullOrEmpty(instanceName))
                instanceName = "PPA";
            else
                instanceName = instanceName.Trim();

            // Establish archive information for this historian instance
            HistorianServerDatabaseConfig archiveInfo = new(instanceName, destinationFilesLocation, !readOnly);

            if (!double.TryParse(targetFileSize, out double targetSize))
                targetSize = 1.5D;

            archiveInfo.TargetFileSize = (long)(targetSize * SI.Giga);

            if (!int.TryParse(directoryNamingMethod, out int methodIndex) || !Enum.IsDefined(typeof(ArchiveDirectoryMethod), methodIndex))
                methodIndex = (int)ArchiveDirectoryMethod.TopDirectoryOnly;

            archiveInfo.DirectoryMethod = (ArchiveDirectoryMethod)methodIndex;

            m_server = new HistorianServer(archiveInfo);
            m_logMessage("[SnapDB] Engine initialized");
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="SnapDBEngine"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~SnapDBEngine()
        {
            Dispose(false);
        }

        public SnapServer ServerHost => m_server.Host;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try

            {
                if (m_server is not null)
                {
                    m_server.Dispose();
                    m_server = null;
                }

                if (m_logSubscriber is not null)
                {
                    m_logSubscriber.NewLogMessage -= m_logSubscriber_Log;
                    m_logSubscriber = null;
                }

                m_logMessage("[SnapDB] Engine terminated");
            }
            finally
            {
                m_disposed = true;  // Prevent duplicate dispose.
            }
        }

        // Expose SnapDB log messages via Adapter status and exception event raisers
        private void m_logSubscriber_Log(LogMessage logMessage)
        {
            m_logMessage(logMessage.Exception is null ? 
                $"[SnapDB] {logMessage.Level}: {logMessage.GetMessage()}" : 
                $"[SnapDB] Exception during {logMessage.EventPublisherDetails.EventName}: {logMessage.GetMessage()}");
        }
    }
    
    [TestFixture]
    public class DownsampledWriter
    {
        [Test]
        public void TestDataExtraction()
        {

        }

        private object OpenArchiveFile(string archiveFileName)
        {
            throw new NotImplementedException();
        }

        private void ExtractData(object source, DateTime startTime, DateTime endTime, string targetFilePath, int downsampling = 0)
        {
            //string completeFileName = GetDestinationFileName(stream.ArchiveFile, sourceFileName, instanceName, destinationPath, method);
            //string pendingFileName = Path.Combine(FilePath.GetDirectoryName(completeFileName), FilePath.GetFileNameWithoutExtension(completeFileName) + ".~d2i");

            //SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(pendingFileName, completeFileName, 4096, null, encoder.EncodingMethod, stream);

            //migratedPoints = stream.PointCount;
        }
    }
}
