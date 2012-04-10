//******************************************************************************************************
//  HadoopReplicationProvider.cs - Gbtc
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
//  11/12/2009 - Pinal C. Patel
//       Generated original version of source code.
//  11/18/2009 - Pinal C. Patel
//       Localized the use of replication log file to ReplicateArchive() method.
//  12/04/2009 - Pinal C. Patel
//       Added more information to replication log file that can be useful for troubleshooting.
//  12/22/2009 - Pinal C. Patel
//       Added try...catch block to the code that performs FTP file delete when an exception is 
//       encountered in ReplicateArchive() method.
//  12/23/2009 - Pinal C. Patel
//       Added HashRequestAttempts and HashRequestWaitTime properties for better control over HDFS file 
//       hash request process.
//  12/24/2009 - Pinal C. Patel
//       Added error handling around the hash request process due to the flaky nature of FTP server when
//       dealing with large files.
//  03/03/2010 - Pinal C. Patel
//       Modified to include files in sub directories of the root directory to be included in the 
//       replication process.
//  05/20/2010 - Pinal C. Patel
//       Added the option to allow for deletion of the original files after being replicated.
//  06/10/2010 - Pinal C. Patel
//       Added the option to allow multiple archive locations to be specified delimited by ';' in 
//       ArchiveLocation for convenience.
//       Added extensive debug messages to enhance the debugging experience of the adapter.
//  09/13/2010 - Pinal C. Patel
//       Modified the replication process to attempt deletion of the original file again if deletion was 
//       unsuccessful during previous attempt.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using TVA;
using TVA.Collections;
using TVA.Configuration;
using TVA.Historian.Replication;
using TVA.IO;
using TVA.IO.Checksums;
using TVA.Net.Ftp;
using TVA.Units;

namespace Hadoop.Replication
{
    /// <summary>
    /// Represents a provider of replication for the <see cref="TVA.Historian.IArchive"/> to Hadoop using FTP channel.
    /// </summary>
    public class HadoopReplicationProvider : ReplicationProviderBase
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the default value for the <see cref="BytesPerCrc32"/> property.
        /// </summary>
        public const int DefaultBytesPerCrc32 = 512;

        /// <summary>
        /// Specifies the default value for the <see cref="HdfsBlockSize"/> property.
        /// </summary>
        public const int DefaultHdfsBlockSize = 64;

        /// <summary>
        /// Specifies the default value for the <see cref="ApplyBufferPadding"/> property.
        /// </summary>
        public const bool DefaultApplyBufferPadding = true;

        /// <summary>
        /// Specifies the default value for the <see cref="HashRequestAttempts"/> property.
        /// </summary>
        public const int DefaultHashRequestAttempts = 3;

        /// <summary>
        /// Specifies the default value for the <see cref="HashRequestWaitTime"/> property.
        /// </summary>
        public const int DefaultHashRequestWaitTime = 3000;

        /// <summary>
        /// Specifies the default value for the <see cref="DeleteOriginalFiles"/> property.
        /// </summary>
        public const bool DefaultDeleteOriginalFiles = false;

        /// <summary>
        /// Length to be used for the <see cref="FilePath.TrimFileName"/> method.
        /// </summary>
        private const int FilePathTrimLength = 30;

        /// <summary>
        /// Name of the file where replication history information is to be serialized.
        /// </summary>
        private const string ReplicationLogFile = "HadoopReplicationLog.xml";

        // Fields
        private int m_bytesPerCrc32;
        private int m_hdfsBlockSize;
        private bool m_applyBufferPadding;
        private int m_hashRequestAttempts;
        private int m_hashRequestWaitTime;
        private bool m_deleteOriginalFiles;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopReplicationProvider"/> class.
        /// </summary>
        public HadoopReplicationProvider()
            : base()
        {
            m_bytesPerCrc32 = DefaultBytesPerCrc32;
            m_hdfsBlockSize = DefaultHdfsBlockSize;
            m_applyBufferPadding = DefaultApplyBufferPadding;
            m_hashRequestAttempts = DefaultHashRequestAttempts;
            m_hashRequestWaitTime = DefaultHashRequestWaitTime;
            m_deleteOriginalFiles = DefaultDeleteOriginalFiles;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the number of bytes at which HDFS is configured to compute a CRC32.
        /// </summary>
        /// <exception cref="ArgumentException">Value being assigned is zero or negative.</exception>
        public int BytesPerCrc32
        {
            get
            {
                return m_bytesPerCrc32;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be at least 1.");

                m_bytesPerCrc32 = value;
            }
        }

        /// <summary>
        /// Gets or sets the size, in MB, of the data blocks for HDFS where the file resides.
        /// </summary>
        /// <exception cref="ArgumentException">Value being assigned is zero or negative.</exception>
        public int HdfsBlockSize
        {
            get
            {
                return m_hdfsBlockSize;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Value must be at least 1.");

                m_hdfsBlockSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the buffer used for computing file hash is to be padded with null bytes for replicating HDFS hashing bug.
        /// </summary>
        public bool ApplyBufferPadding
        {
            get
            {
                return m_applyBufferPadding;
            }
            set
            {
                m_applyBufferPadding = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of requests to be made to the FTP server for HDFS file hash.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is negative or zero.</exception>
        public int HashRequestAttempts
        {
            get
            {
                return m_hashRequestAttempts;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive");

                m_hashRequestAttempts = value;
            }
        }

        /// <summary>
        /// Gets or set the time, in milliseconds, to wait between requests to the FTP server for HDFS file hash.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is negative or zero.</exception>
        public int HashRequestWaitTime
        {
            get
            {
                return m_hashRequestWaitTime;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be positive");

                m_hashRequestWaitTime = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the original files are to be deleted after being replicated successfully.
        /// </summary>
        public bool DeleteOriginalFiles
        {
            get
            {
                return m_deleteOriginalFiles;
            }
            set
            {
                m_deleteOriginalFiles = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Saves <see cref="HadoopReplicationProvider"/> settings to the config file if the <see cref="TVA.Adapters.Adapter.PersistSettings"/> property is set to true.
        /// </summary>
        public override void SaveSettings()
        {
            base.SaveSettings();
            if (PersistSettings)
            {
                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings["BytesPerCrc32", true].Update(m_bytesPerCrc32);
                settings["HdfsBlockSize", true].Update(m_hdfsBlockSize);
                settings["ApplyBufferPadding", true].Update(m_applyBufferPadding);
                settings["HashRequestAttempts", true].Update(m_hashRequestAttempts);
                settings["HashRequestWaitTime", true].Update(m_hashRequestWaitTime);
                settings["DeleteOriginalFiles", true].Update(m_deleteOriginalFiles);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved <see cref="HadoopReplicationProvider"/> settings from the config file if the <see cref="TVA.Adapters.Adapter.PersistSettings"/> property is set to true.
        /// </summary>  
        public override void LoadSettings()
        {
            base.LoadSettings();
            if (PersistSettings)
            {
                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("BytesPerCrc32", m_bytesPerCrc32, "Number of bytes at which HDFS is configured to compute a CRC32.");
                settings.Add("HdfsBlockSize", m_hdfsBlockSize, "Size (in MB) of the data blocks for HDFS where the file resides.");
                settings.Add("ApplyBufferPadding", m_applyBufferPadding, "True if the buffer used for computing file hash is to be padded with null bytes for replicating HDFS hashing bug, otherwise False.");
                settings.Add("HashRequestAttempts", m_hashRequestAttempts, "Maximum number of requests to be made to the FTP server for HDFS file hash.");
                settings.Add("HashRequestWaitTime", m_hashRequestWaitTime, "Time (in milliseconds) to wait between requests to the FTP server for HDFS file hash.");
                settings.Add("DeleteOriginalFiles", m_deleteOriginalFiles, "True if the original files are to be deleted after being replicated successfully; otherwise False.");
                BytesPerCrc32 = settings["BytesPerCrc32"].ValueAs(m_bytesPerCrc32);
                HdfsBlockSize = settings["HdfsBlockSize"].ValueAs(m_hdfsBlockSize);
                ApplyBufferPadding = settings["ApplyBufferPadding"].ValueAs(m_applyBufferPadding);
                HashRequestAttempts = settings["HashRequestAttempts"].ValueAs(m_hashRequestAttempts);
                HashRequestWaitTime = settings["HashRequestWaitTime"].ValueAs(m_hashRequestWaitTime);
                DeleteOriginalFiles = settings["DeleteOriginalFiles"].ValueAs(m_deleteOriginalFiles);
            }
        }

        /// <summary>
        /// Replicates the <see cref="TVA.Historian.IArchive"/>.
        /// </summary>
        protected override void ReplicateArchive()
        {
            WriteTrace("Archive replication started");

            // Parse FTP client information.
            Uri replicaUri = new Uri(ReplicaLocation);
            string[] credentials = replicaUri.UserInfo.Split(':');

            // Ensure credentials are supplied.
            if (credentials.Length != 2)
                throw new ArgumentException("FTP credentials are missing in ReplicaLocation.");

            // Create FTP client for uploading.
            FtpClient ftpClient = new FtpClient();
            ftpClient.Server = replicaUri.Host;
            ftpClient.Port = replicaUri.Port;
            ftpClient.FileTransferProgress += FtpClient_FileTransferProgress;

            // Initialize the replication log.
            WriteTrace("Initializing archive replication log");
            DataTable replicationLog = new DataTable("ReplicationRecord");
            replicationLog.Columns.Add("DateTime");
            replicationLog.Columns.Add("FileName");
            replicationLog.Columns.Add("FileHash");
            replicationLog.Columns.Add("FileSync");
            replicationLog.Columns.Add("HashingTime");
            replicationLog.Columns.Add("TransferTime");
            replicationLog.Columns.Add("TransferRate");
            replicationLog.Columns.Add("ServerRequests");
            replicationLog.Columns.Add("ServerResponse");
            if (File.Exists(FilePath.GetAbsolutePath(ReplicationLogFile)))
                replicationLog.ReadXml(FilePath.GetAbsolutePath(ReplicationLogFile));
            WriteTrace("Archive replication log initialized");

            try
            {
                // Connect FTP client to server.
                WriteTrace("Connecting to ftp://{0}:{1}", ftpClient.Server, ftpClient.Port);
                ftpClient.Connect(credentials[0], credentials[1]);
                WriteTrace("Connection successful");
                WriteTrace("Changing current directory to '{0}'", replicaUri.AbsolutePath);
                ftpClient.SetCurrentDirectory(replicaUri.LocalPath);
                WriteTrace("Current directory changed to '{0}'", ftpClient.CurrentDirectory.FullPath);

                // Process all archive location(s).
                foreach (string folder in ArchiveLocation.Split(';'))
                {
                    if (!string.IsNullOrEmpty(folder))
                        ReplicateToHadoop(ftpClient, folder.Trim(), replicationLog);
                }
            }
            finally
            {
                ftpClient.Dispose();
                replicationLog.WriteXml(FilePath.GetAbsolutePath(ReplicationLogFile));
            }

            WriteTrace("Archive replication complete");
        }

        private void ReplicateToHadoop(FtpClient ftpClient, string replicationFolder, DataTable replicationLog)
        {
            WriteTrace("Replicating folder '{0}'", FilePath.TrimFileName(replicationFolder, FilePathTrimLength));

            // Create list of files to be replicated.
            List<string> files = new List<string>(Directory.GetFiles(replicationFolder, "*_to_*.d", SearchOption.AllDirectories));
            files.Sort();

            // Process all the files in the list.
            WriteTrace("Found {0} files in folder", files.Count);
            foreach (string file in files)
            {
                // Initialize local variables.
                int requests = int.MinValue;
                bool uploading = false;
                double hashingStartTime = double.MinValue;
                double hashingTotalTime = double.MinValue;
                double transferStartTime = double.MinValue;
                double transferTotalTime = double.MinValue;
                string justFileName = FilePath.GetFileName(file);
                DataRow record = null;
                DataRow[] filter = replicationLog.Select(string.Format("FileName ='{0}'", justFileName));
                FileInfo fileInfo = new FileInfo(file);

                WriteTrace("Replicating file '{0}' of size {1:0,0} KB", FilePath.TrimFileName(file, FilePathTrimLength), Convert.ToInt32(fileInfo.Length / 1024D));
                try
                {
                    // Continue to "ping" FTP server so that it knows we are alive and well
                    ftpClient.ControlChannel.Command("NOOP");

                    // Compute HDFS file hash.
                    WriteTrace("Hashing file");
                    hashingStartTime = Common.SystemTimer;
                    byte[] localHash = ComputeHdfsFileHash(file, m_bytesPerCrc32, m_hdfsBlockSize, m_applyBufferPadding);
                    hashingTotalTime = Common.SystemTimer - hashingStartTime;
                    WriteTrace("File hashed in {0} seconds", Convert.ToInt32(hashingTotalTime));

                    // Check if file is to be uploaded.
                    if (filter.Length == 0 ||
                        filter[0]["FileSync"].ToString() == "Fail" ||
                        localHash.CompareTo(ByteEncoding.Hexadecimal.GetBytes(filter[0]["FileHash"].ToString())) != 0)
                    {
                        // Upload file to HDFS since:
                        // 1) File has not been replicated previously.
                        // OR
                        // 2) File has been replicated in the past, but its content has changed since then.
                        uploading = true;
                        WriteTrace("Uploading file");
                        transferStartTime = Common.SystemTimer;
                        ftpClient.CurrentDirectory.PutFile(file);
                        transferTotalTime = Common.SystemTimer - transferStartTime;
                        WriteTrace("File uploaded in {0} seconds", Convert.ToInt32(transferTotalTime));

                        // Request file hash from HDFS.
                        for (requests = 1; requests <= m_hashRequestAttempts; requests++)
                        {
                            try
                            {
                                // Wait before request.
                                WriteTrace("Waiting {0} seconds before HDFS hash request", m_hashRequestWaitTime / 1000);
                                Thread.Sleep(m_hashRequestWaitTime);
                                // Request file hash.
                                WriteTrace("Requesting HDFS hash (Attempt {0})", requests);
                                ftpClient.ControlChannel.Command(string.Format("HDFSCHKSM {0}{1}", ftpClient.CurrentDirectory.FullPath, justFileName));
                                WriteTrace("Hash request response - {0}", ftpClient.ControlChannel.LastResponse.Message.RemoveCrLfs());
                                // Exit when successful.
                                if (ftpClient.ControlChannel.LastResponse.Code == 200)
                                    break;
                            }
                            catch (Exception ex)
                            {
                                // Try again - Apache MINA FTP server acts funny with updoad & hash check of large files.
                                WriteTrace("Hash request error - {0}", ex.Message);
                            }
                        }

                        // Initialize replication log entry.
                        if (filter.Length > 0)
                        {
                            record = filter[0];
                        }
                        else
                        {
                            record = replicationLog.NewRow();
                            replicationLog.Rows.Add(record);
                        }

                        // Update replication log entry.
                        record["DateTime"] = DateTime.UtcNow;
                        record["FileName"] = justFileName;
                        record["FileHash"] = ByteEncoding.Hexadecimal.GetString(localHash);
                        record["HashingTime"] = hashingTotalTime.ToString("0.000");
                        record["TransferTime"] = transferTotalTime.ToString("0.000");
                        record["TransferRate"] = ((new FileInfo(file).Length / SI2.Kilo) / transferTotalTime).ToString("0.00");
                        record["ServerRequests"] = requests < m_hashRequestAttempts ? requests : m_hashRequestAttempts;
                        record["ServerResponse"] = ftpClient.ControlChannel.LastResponse.Message.RemoveCrLfs();

                        // Compare local and HDFS hash.
                        if (ftpClient.ControlChannel.LastResponse.Code == 200 &&
                            localHash.CompareTo(ByteEncoding.Hexadecimal.GetBytes(ftpClient.ControlChannel.LastResponse.Message.RemoveCrLfs().Split(':')[1])) == 0)
                        {
                            // File uploaded and hashes match.
                            record["FileSync"] = "Pass";
                            WriteTrace("Replication successful");

                            // Deleted original file after replication.
                            if (m_deleteOriginalFiles)
                                DeleteOriginalFile(file, fileInfo);

                            // Notify about the successful replication.
                            OnReplicationProgress(new ProcessProgress<int>("ReplicateArchive", justFileName, 1, 1));
                        }
                        else
                        {
                            // Hashes are different - possible causes:
                            // 1) Local file got modified after hash was computed locally.
                            // OR
                            // 2) Local and remote hashing algorithms are not the same.
                            record["FileSync"] = "Fail";
                            WriteTrace("Replication unsuccessful");
                            throw new InvalidDataException("File hash mismatch");
                        }

                        // Write replication entry to the log file.
                        WriteTrace("Updating replication log file");
                        replicationLog.WriteXml(FilePath.GetAbsolutePath(ReplicationLogFile));
                        WriteTrace("Replication log file updated");
                    }
                    else
                    {
                        WriteTrace("Replication skipped - file content unchanged");

                        // Deleted original file if skipped previously.
                        if (m_deleteOriginalFiles)
                            DeleteOriginalFile(file, fileInfo);
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace("Replication error - {0}", ex.Message);

                    // Delete file from FTP site if an exception is encountered when processing the file.
                    try
                    {
                        if (uploading && ftpClient.IsConnected)
                        {
                            WriteTrace("Deleting partial upload");
                            ftpClient.CurrentDirectory.RemoveFile(justFileName);
                            WriteTrace("Partial upload deleted");
                        }
                    }
                    catch (Exception exDelete)
                    {
                        WriteTrace("Delete error - {0}", exDelete.Message);
                    }

                    if (ex is ThreadAbortException)
                        // Re-throw the encountered exception.
                        throw;
                    else
                        // Notify about the encountered exception.
                        OnReplicationException(ex);
                }
            }

            WriteTrace("Folder '{0}' replicated", FilePath.TrimFileName(replicationFolder, FilePathTrimLength));
        }

        private static void DeleteOriginalFile(string file, FileInfo fileInfo)
        {
            try
            {
                WriteTrace("Deleting original file");
                if (fileInfo.IsReadOnly)
                    fileInfo.IsReadOnly = false;
                File.Delete(file);
                WriteTrace("Original file deleted");
            }
            catch (Exception ex)
            {
                WriteTrace("File delete error - {0}", ex.Message);
            }
        }

        private void FtpClient_FileTransferProgress(object sender, EventArgs<ProcessProgress<long>, TransferDirection> e)
        {
            // Show transfer progress at 10% increaments.
            long percentComplete = (e.Argument1.Complete * 100) / e.Argument1.Total;
            if ((percentComplete % 10) == 0)
                WriteTrace("{0}ed {1}%", e.Argument2, percentComplete);
        }

        #endregion

        #region [ Static ]

        // Static Methods

        /// <summary>
        /// Computes a MD5 hash of the file content using the algorithm used by HDFS.
        /// </summary>
        /// <param name="fileName">Name of the file for which the hash is to be computed.</param>
        /// <param name="bytesPerCrc32">Number of bytes at which HDFS is configured to compute a CRC32.</param>
        /// <param name="hdfsBlockSize">Size (in MB) of the data blocks for HDFS where the file resides.</param>
        /// <param name="applyBufferPadding">true if the buffer used for computing file hash is to be padded with null bytes for replicating HDFS hashing bug, otherwise false.</param>
        /// <returns>An <see cref="Array"/> of <see cref="byte"/>s containing the file hash.</returns>
        public static byte[] ComputeHdfsFileHash(string fileName, int bytesPerCrc32, int hdfsBlockSize, bool applyBufferPadding)
        {
            int bytesRead = 0;
            int blockCount = 0;
            byte[] blockCRC = null;
            byte[] blockMD5 = null;
            byte[] fileHash = null;
            FileStream fileStream = null;
            List<byte> blockCRCs = new List<byte>();
            List<byte> blockMD5s = new List<byte>();
            byte[] readBuffer = new byte[bytesPerCrc32];
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();

            try
            {
                WriteTrace("Computing HDFS file hash for '{0}'", FilePath.TrimFileName(fileName, FilePathTrimLength));

                // Open file whose hash is to be computed.
                fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // Calculate the number of HDFS blocks used by the file on HDFS.
                blockCount = (int)Math.Ceiling((double)fileStream.Length / (double)(hdfsBlockSize * SI2.Mega));

                // For each HDFS block used by the file on HDFS:
                // 1) Compute CRC32s at every "bytesPerCrc32" bytes (default is 512 bytes) and store it in a buffer.
                // 2) Compute block MD5 hash by computing a MD5 hash of the buffer that contains the block CRC32s.
                for (int i = 1; i <= blockCount; i++)
                {
                    // Clear existing data from CRC32 buffer.
                    blockCRCs.Clear();
                    while ((bytesRead = fileStream.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        // Read "bytesPerCrc32" bytes from the file and compute CRC32.
                        blockCRC = EndianOrder.BigEndian.GetBytes(readBuffer.Crc32Checksum(0, bytesRead));
                        // Add big-endian byte array of the computed CRC32 to CRC32 buffer.
                        blockCRCs.AddRange(blockCRC);
                        // Stop reading and compute block MD5 hash when a HDFS block data has been processed.
                        if (fileStream.Position >= i * hdfsBlockSize * SI2.Mega)
                            break;
                    }

                    // Compute block MD5 hash from the buffer containg block CRC32s.
                    blockMD5 = hasher.ComputeHash(blockCRCs.ToArray());

                    // Store the computed block MD5 hash in a buffer that will be used for computing the file hash.
                    if (applyBufferPadding)
                    {
                        // Apply padding - this replicates a bug in HDFS file hashing algorithm.
                        if (blockMD5s.Count == 0)
                            // Initialize the buffer to 32 bytes.
                            blockMD5s.AddRange(new byte[32]);
                        else if (blockMD5s.Count < blockMD5.Length * i)
                            // Extend the buffer twice its current size.
                            blockMD5s.AddRange(new byte[(blockMD5s.Count * 2) - blockMD5s.Count]);

                        blockMD5s.UpdateRange((i - 1) * blockMD5.Length, blockMD5);
                    }
                    else
                    {
                        // Don't apply padding - this will compute the correct HDFS file hash as per its design.
                        blockMD5s.AddRange(blockMD5);
                    }

                    WriteTrace("Block {0} MD5 - {1}", i, ByteEncoding.Hexadecimal.GetString(blockMD5));
                }

                // Compute the final file hash from the buffer that contains block MD5 hashes.
                fileHash = hasher.ComputeHash(blockMD5s.ToArray());
                WriteTrace("HDFS file hash - {0}", ByteEncoding.Hexadecimal.GetString(fileHash));
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }

            return fileHash;
        }

        private static void WriteTrace(string message, params object[] args)
        {
            Trace.WriteLine(DateTime.Now + ": " + string.Format(message, args), typeof(HadoopReplicationProvider).Name);
        }

        #endregion
    }
}
