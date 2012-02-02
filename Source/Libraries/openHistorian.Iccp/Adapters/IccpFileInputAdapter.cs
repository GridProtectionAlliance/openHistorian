//*******************************************************************************************************
//  IccpFileInputAdapter.cs - Gbtc
//
//  Tennessee Valley Authority, 2011
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  12/01/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
using openHistorian.Adapters;
using openHistorian.Archives;
using TVA;
using TVA.Data;

namespace openHistorian.Iccp.Adapters
{
    public class IccpFileInputAdapter : DataAdapterBase
    {
        #region [ Members ]

        // Constants
        public const string DefaultFilePath = "";
        public const string DefaultFileSpec = "";
        public const double DefaultPollingInterval = -1;
        public const bool DefaultDeleteProcessedFiles = false;
        public const string DefaultProcessedFilesDirectory = "";

        private const string FileNameDateTimeFormat = "yyMMddHHmmss";
        private const string ConnectionStringTemplate = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""{0}"";Extended Properties=""text;HDR=No;FMT=Delimited"";";

        // Fields
        private string m_filePath;
        private string m_fileSpec;
        private double m_pollingInterval;
        private bool m_deleteProcessedFiles;
        private string m_processedFilesDirectory;
        private bool m_fileNameHasDateTime;
        private Dictionary<string, string> m_lookup;
        private IdentifiableItem<string, IDataArchive> m_archive;
        private Timer m_fileTimer;
        private FileSystemWatcher m_fileWatcher;
        private bool m_disposed;
        private bool m_initialized;

        #endregion

        #region [ Constructors ]

        public IccpFileInputAdapter()
        {
            m_filePath = DefaultFilePath;
            m_fileSpec = DefaultFileSpec;
            m_pollingInterval = DefaultPollingInterval;
            m_deleteProcessedFiles = DefaultDeleteProcessedFiles;
            m_processedFilesDirectory = DefaultProcessedFilesDirectory;
        }

        #endregion

        #region [ Properties ]

        public string FilePath
        {
            get
            {
                return m_filePath;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_filePath = value;
            }
        }

        public string FileSpec
        {
            get
            {
                return m_fileSpec;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_fileSpec = value;
            }
        }

        public double PollingInterval
        {
            get
            {
                return m_pollingInterval;
            }
            set
            {
                m_pollingInterval = value;
            }
        }

        public bool DeleteProcessedFiles
        {
            get
            {
                return m_deleteProcessedFiles;
            }
            set
            {
                m_deleteProcessedFiles = value;
            }
        }

        public string ProcessedFilesDirectory
        {
            get
            {
                return m_processedFilesDirectory;
            }
            set
            {
                m_processedFilesDirectory = value;
            }
        }

        public bool FileNameHasDateTime
        {
            get
            {
                return m_fileNameHasDateTime;
            }
            set
            {
                m_fileNameHasDateTime = value;
            }
        }

        #endregion

        #region [ Methods ]

        public override void Initialize()
        {
            base.Initialize();
            if (!m_initialized)
            {
                // Find the target archive for received data.
                IDictionary<string, IDataArchive> archives = FindTargetArchives(1, 1);
                m_archive = new IdentifiableItem<string, IDataArchive>(archives.First().Key, archives.First().Value);

                // Create lookup from mappings for runtime use.
                m_lookup = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (IDataMapping mapping in Mappings.Where(m => m.Source != "*"))
                {
                    m_lookup.Add(mapping.Source, mapping.Target);
                }

                if (m_pollingInterval > 0)
                {
                    // Poll for new files.
                    m_fileTimer = new Timer(m_pollingInterval);
                    m_fileTimer.Elapsed += FileTimer_Elapsed;
                }
                else
                {
                    // Watch for new files.
                    m_fileWatcher = new FileSystemWatcher(m_filePath);
                    m_fileWatcher.Filter = m_fileSpec;
                    m_fileWatcher.Changed += FileWatcher_Changed;
                }

                m_initialized = true;
            }
        }

        public override void Start()
        {
            if (m_fileTimer != null)
                m_fileTimer.Start();

            if (m_fileWatcher != null)
                m_fileWatcher.EnableRaisingEvents = true;

            foreach (string file in Directory.GetFiles(m_filePath, m_fileSpec))
            {
                ProcessFile(file);
            }
        }

        public override void Stop()
        {
            if (m_fileTimer != null)
                m_fileTimer.Stop();

            if (m_fileWatcher != null)
                m_fileWatcher.EnableRaisingEvents = false;
        }

        protected override void OnArchiveAdded(IDataArchive archive)
        {
            if (string.Compare(m_archive.ID, archive.Name) == 0)
            {
                // Save reference to the added archive.
                m_archive.Item = archive;
                OnStatusUpdate(UpdateType.Information, "Saved reference to \"{0}\"", m_archive.ID);
            }
        }

        protected override void OnArchiveRemoved(IDataArchive archive)
        {
            if (string.Compare(m_archive.ID, archive.Name) == 0)
            {
                // Remove reference of the removed archive.
                m_archive = null;
                OnStatusUpdate(UpdateType.Information, "Removed reference to \"{0}\"", m_archive.ID);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        Stop();

                        if (m_fileTimer != null)
                        {
                            m_fileTimer.Elapsed -= FileTimer_Elapsed;
                            m_fileTimer.Dispose();
                        }

                        if (m_fileWatcher != null)
                        {
                            m_fileWatcher.Changed -= FileWatcher_Changed;
                            m_fileWatcher.Dispose();
                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        private void ProcessFile(string file)
        {
            // Check if file exists.
            if (!File.Exists(file))
            {
                OnStatusUpdate(UpdateType.Warning, "File {0} does not exist", file);
                return;
            }

            // Check if archive is available.
            if (m_archive.Item == null || m_archive.Item.State != DataArchiveState.Open)
            {
                OnStatusUpdate(UpdateType.Warning, "Archive is not available for writing");
                return;
            }

            string inKey;
            string outKey;
            TimeTag time;
            DataTable data = null;
            OleDbConnection connection = null;
            string name = Path.GetFileName(file);
            try
            {
                OnStatusUpdate(UpdateType.Information, "Processing {0}", name);

                if (!m_fileNameHasDateTime)
                {
                    // Use the date and time when file was created.
                    time = new TimeTag(File.GetLastAccessTimeUtc(file));
                }
                else
                {
                    // Use the date and time present in the file name.
                    string nameNoExtention = Path.GetFileNameWithoutExtension(name);
                    string fileDateTime = nameNoExtention.TruncateLeft(FileNameDateTimeFormat.Length);
                    time = new TimeTag(DateTime.ParseExact(fileDateTime, FileNameDateTimeFormat, CultureInfo.InvariantCulture));
                }

                connection = new OleDbConnection(string.Format(ConnectionStringTemplate, Path.GetDirectoryName(file)));
                connection.Open();
                data = connection.RetrieveData(string.Format("SELECT * FROM {0}", name));
                connection.Close();
                foreach (DataRow row in data.Rows)
                {
                    inKey = row[0].ToString();
                    if (m_lookup.TryGetValue(inKey, out outKey))
                    {
                        // Mapping exists for incoming data.
                        Data point = new Data(outKey)
                        {
                            Value = float.Parse(row[1].ToString()),
                            Time = time,
                            Quality = Quality.Good
                        };

                        m_archive.Item.WriteData(point);
                    }
                    else
                    {
                        // No mapping exists for incoming data.
                        OnStatusUpdate(UpdateType.Warning, "No mapping exists for {0}", inKey);
                    }
                }

                var calculated = from e in m_lookup
                                 where e.Key.Contains('+')
                                 select e;

                foreach (var entry in calculated)
                {
                    // Compute target value.
                    string[] sources = entry.Key.Split('+');
                    var select = from e in data.AsEnumerable()
                                 where sources.FirstOrDefault(source => source.Trim() == e[0].ToString()) != null
                                 select e;

                    Data point = new Data(entry.Value)
                    {
                        Value = (float)select.Sum(input => float.Parse(input[1].ToString())),
                        Time = time,
                        Quality = Quality.Good
                    };

                    m_archive.Item.WriteData(point);
                }

                if (!string.IsNullOrEmpty(m_processedFilesDirectory))
                    File.Copy(file, Path.Combine(m_processedFilesDirectory, name));

                if (m_deleteProcessedFiles)
                    File.Delete(file);

                OnStatusUpdate(UpdateType.Information, "Processed {0}", name);
            }
            catch (Exception ex)
            {
                if (connection != null)
                    connection.Dispose();

                if (data != null)
                    data.Dispose();

                OnExecutionException(string.Format("Error processing {0}", name), ex);
            }
        }

        private void FileTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (string file in Directory.GetFiles(m_filePath, m_fileSpec))
            {
                ProcessFile(file);
            }
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed ||
                e.ChangeType == WatcherChangeTypes.Created)
                ProcessFile(e.FullPath);
        }

        #endregion
    }
}
