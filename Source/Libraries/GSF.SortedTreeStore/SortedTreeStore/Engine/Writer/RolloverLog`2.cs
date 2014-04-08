//******************************************************************************************************
//  RolloverLog`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  ----------------------------------------------------------------------------------------------------
//  3/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// Logs the rollover process so that it can be properly finished in the event of a power outage or process crash
    /// </summary>
    /// <typeparam name="TKey">The key</typeparam>
    /// <typeparam name="TValue">The value</typeparam>
    public class RolloverLog<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        /// <summary>
        /// The verification status of the rollover process.
        /// </summary>
        public enum VerificationInformation
        {
            /// <summary>
            /// Indicates that the rolled over file was never actually created
            /// </summary>
            NotStarted,
            /// <summary>
            /// Indicates that the file was being rolled over, but the final point was never committed to the disk.
            /// </summary>
            NotFinishedDestination,
            /// <summary>
            /// Indicates that all data was successfully rolled over, however, the source files were never deleted.
            /// </summary>
            NotDeletedSourceFiles,
            /// <summary>
            /// Indicates that the source files have been modified.
            /// </summary>
            Corrupt,
            /// <summary>
            /// Indicates that the rollover was successful, and now the log file can be deleted.
            /// </summary>
            RolloverComplete
        }

        class FileDetails
        {
            public Guid FileId;
            public uint SnapshotSequenceNumber;

            public FileDetails()
            {

            }
            public FileDetails(BinaryReader reader)
            {
                switch (reader.ReadByte())
                {
                    case 1:
                        FileId = new Guid(reader.ReadBytes(16));
                        SnapshotSequenceNumber = reader.ReadUInt32();
                        return;
                    default:
                        throw new Exception("Unknown Verison");
                }
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write((byte)1);
                writer.Write(FileId.ToByteArray());
                writer.Write(SnapshotSequenceNumber);
            }
        }

        Guid m_keyGuid;
        Guid m_valueGuid;
        List<FileDetails> m_sourceFiles = new List<FileDetails>();
        Guid m_destinationFile;
        TKey m_startKey = new TKey();
        TKey m_endKey = new TKey();
        string m_savedFileName;

        static readonly System.Text.Encoding Unicode;
        static readonly byte[] Header;
        static RolloverLog()
        {
            Unicode = System.Text.Encoding.Unicode;
            Header = Unicode.GetBytes("Historian 2.0 Rollover Log");
        }

        /// <summary>
        /// Creates a rollover log
        /// </summary>
        /// <param name="sourceFiles">All of the source files that will be combined into a destination file.</param>
        /// <param name="destinationFile"></param>
        /// <param name="list">the archive list so additional metadata can be looked up </param>
        public RolloverLog(List<Guid> sourceFiles, Guid destinationFile, ArchiveList<TKey, TValue> list)
        {
            using (var resources = list.CreateNewClientResources())
            {
                resources.UpdateSnapshot();

                foreach (var fileId in sourceFiles)
                {
                    var table = resources.TryGetFile(fileId);
                    if (table == null)
                        throw new Exception("File not found");

                    var file = new FileDetails();
                    file.FileId = table.SortedTreeTable.BaseFile.Snapshot.Header.ArchiveId;
                    file.SnapshotSequenceNumber = table.SortedTreeTable.BaseFile.Snapshot.Header.SnapshotSequenceNumber;

                    m_sourceFiles.Add(file);
                }
                m_destinationFile = destinationFile;
            }
        }

        /// <summary>
        /// Opens a rollover log to check if the rollover was complete.
        /// </summary>
        /// <param name="fileName">the name of the log file</param>
        public RolloverLog(string fileName)
        {
            m_savedFileName = fileName;
            byte[] data = File.ReadAllBytes(fileName);
            if (data.Length < 20 + 1 + Header.Length)
                throw new Exception("File Corrupt, Not enough length");
            for (int x = 0; x < Header.Length; x++)
            {
                if (data[x] != Header[x])
                    throw new Exception("Corrupt Header Code");
            }

            byte[] hash = new byte[20];
            data.CopyTo(hash, hash.Length - 20);
            using (var sha = new SHA1Managed())
            {
                var checksum = sha.ComputeHash(data, 0, data.Length - 20);
                if (!hash.SequenceEqual(checksum))
                    throw new Exception("Invalid Checksum");
            }

            var stream = new MemoryStream(data);
            var reader = new BinaryReader(stream, Unicode);
            var headerCode = reader.ReadBytes(Header.Length);
            if (!headerCode.SequenceEqual(Header))
                throw new Exception("Corrupt Header Code");

            switch (reader.ReadByte())
            {
                case 1:
                    m_keyGuid = new Guid(reader.ReadBytes(16));
                    m_valueGuid = new Guid(reader.ReadBytes(16));

                    if (m_keyGuid != new TKey().GenericTypeGuid)
                        throw new Exception("Key type does not match");

                    if (m_valueGuid != new TValue().GenericTypeGuid)
                        throw new Exception("Value type does not match");

                    m_startKey.Read(stream);
                    m_endKey.Read(stream);
                    int count = reader.ReadInt32();
                    while (count > 0)
                    {
                        count--;
                        m_sourceFiles.Add(new FileDetails(reader));
                    }
                    m_destinationFile = new Guid(reader.ReadBytes(16));
                    return;


                default:
                    throw new Exception("Version not recgonized.");
            }

        }

        /// <summary>
        /// Writes the rollover log file to the disk
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            m_savedFileName = filename;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream, Unicode);
            writer.Write(Header);
            writer.Write((byte)1);
            writer.Write(m_keyGuid.ToByteArray());
            writer.Write(m_valueGuid.ToByteArray());
            m_startKey.Write(stream);
            m_endKey.Write(stream);
            writer.Write(m_sourceFiles.Count);
            foreach (var file in m_sourceFiles)
            {
                file.Save(writer);
            }
            writer.Write(m_destinationFile.ToByteArray());

            using (var sha = new SHA1Managed())
            {
                writer.Write(sha.ComputeHash(stream.ToArray()));
            }

            File.WriteAllBytes(filename, stream.ToArray());
        }

        /// <summary>
        /// Gets the verification status of the log file. Used to decide next steps.
        /// </summary>
        /// <param name="list">an archive list that can be used to verify the rollover status</param>
        /// <returns></returns>
        public VerificationInformation Verify(ArchiveList<TKey, TValue> list)
        {
            using (var resources = list.CreateNewClientResources())
            {
                resources.UpdateSnapshot();

                var destination = resources.TryGetFile(m_destinationFile);
                if (destination != null)
                {
                    //The destination was created
                    if (destination.FirstKey.IsEqualTo(m_startKey) && destination.LastKey.IsEqualTo(m_endKey))
                    {
                        //All data was properly copied
                        foreach (var file in m_sourceFiles)
                        {
                            if (resources.TryGetFile(file.FileId) != null)
                                return VerificationInformation.NotDeletedSourceFiles;
                        }
                        return VerificationInformation.RolloverComplete;
                    }
                    return VerificationInformation.NotFinishedDestination;
                }
                //Destination was never created
                foreach (var file in m_sourceFiles)
                {
                    var table = resources.TryGetFile(file.FileId);
                    if (table == null)
                        return VerificationInformation.Corrupt;
                    if (table.SortedTreeTable.BaseFile.Snapshot.Header.SnapshotSequenceNumber != file.SnapshotSequenceNumber)
                        return VerificationInformation.Corrupt;
                }
                return VerificationInformation.NotStarted;
            }
        }

        /// <summary>
        /// Rollbacks any partially committed data.
        /// </summary>
        /// <param name="list"></param>
        public void Rollback(ArchiveList<TKey, TValue> list)
        {
            switch (Verify(list))
            {
                case VerificationInformation.Corrupt:
                    throw new Exception("Corrupt");
                case VerificationInformation.NotDeletedSourceFiles:
                    DeleteSources(list);
                    DeleteLog();
                    return;
                case VerificationInformation.NotStarted:
                    DeleteDestination(list);
                    DeleteLog();
                    return;
                case VerificationInformation.NotFinishedDestination:
                    DeleteDestination(list);
                    DeleteLog();
                    return;
                case VerificationInformation.RolloverComplete:
                    DeleteLog();
                    return;
                default:
                    throw new Exception("Unknown Enum");
            }
        }

        /// <summary>
        /// Deletes the log file from the server
        /// </summary>
        public void DeleteLog()
        {
            File.Delete(m_savedFileName);
        }

        void DeleteDestination(ArchiveList<TKey, TValue> list)
        {
            using (var edit = list.AcquireEditLock())
            {
                edit.TryRemoveAndDelete(m_destinationFile);
            }
        }

        void DeleteSources(ArchiveList<TKey, TValue> list)
        {
            using (var edit = list.AcquireEditLock())
            {
                foreach (var source in m_sourceFiles)
                {
                    if (edit.Contains(source.FileId))
                        edit.TryRemoveAndDelete(source.FileId);
                }
            }
        }
    }
}
