//******************************************************************************************************
//  RolloverPendingAction.cs - Gbtc
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
    public class RolloverPendingAction<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
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
        List<FileDetails> m_sourceFile = new List<FileDetails>();
        Guid m_destinationFileId;
        TKey m_startKey = new TKey();
        TKey m_endKey = new TKey();
        string m_savedFileName;

        static readonly System.Text.Encoding Unicode;
        static readonly byte[] Header;
        static RolloverPendingAction()
        {
            Unicode = System.Text.Encoding.Unicode;
            Header = Unicode.GetBytes("Historian 2.0 Rollover Log");
        }

        public RolloverPendingAction(List<Guid> sourceFiles, Guid destinationFileId, ArchiveList<TKey, TValue> list)
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

                    m_sourceFile.Add(file);
                }
                m_destinationFileId = destinationFileId;
            }

        }

        public RolloverPendingAction(string fileName)
        {
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
                        m_sourceFile.Add(new FileDetails(reader));
                    }
                    m_destinationFileId = new Guid(reader.ReadBytes(16));
                    return;


                default:
                    throw new Exception("Version not recgonized.");
            }

        }

        public void Save(string filename)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream, Unicode);
            writer.Write(Header);
            writer.Write((byte)1);
            writer.Write(m_keyGuid.ToByteArray());
            writer.Write(m_valueGuid.ToByteArray());
            m_startKey.Write(stream);
            m_endKey.Write(stream);
            writer.Write(m_sourceFile.Count);
            foreach (var file in m_sourceFile)
            {
                file.Save(writer);
            }
            writer.Write(m_destinationFileId.ToByteArray());

            using (var sha = new SHA1Managed())
            {
                writer.Write(sha.ComputeHash(stream.ToArray()));
            }

            File.WriteAllBytes(filename, stream.ToArray());
        }

        public enum VerificationInformation
        {
            NotStarted,
            NotFinishedDestination,
            NotDeletedSourceFiles,
            Corrupt,
            RolloverComplete
        }

        public VerificationInformation Verify(ArchiveList<TKey, TValue> list)
        {
            using (var resources = list.CreateNewClientResources())
            {
                resources.UpdateSnapshot();

                var destination = resources.TryGetFile(m_destinationFileId);
                if (destination != null)
                {
                    //The destination was created
                    if (destination.FirstKey.IsEqualTo(m_startKey) && destination.LastKey.IsEqualTo(m_endKey))
                    {
                        //All data was properly copied
                        foreach (var file in m_sourceFile)
                        {
                            if (resources.TryGetFile(file.FileId) != null)
                                return VerificationInformation.NotDeletedSourceFiles;
                        }
                        return VerificationInformation.RolloverComplete;
                    }
                    return VerificationInformation.NotFinishedDestination;
                }
                //Destination was never created
                foreach (var file in m_sourceFile)
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

        public void Rollover(ArchiveList<TKey, TValue> list)
        {
            switch (Verify(list))
            {
                case VerificationInformation.Corrupt:
                    throw new Exception("Corrupt");
                case VerificationInformation.NotDeletedSourceFiles:
                    DeleteSources(list);
                    return;
                case VerificationInformation.NotStarted:
                    StartRollover(list);
                    DeleteSources(list);
                    return;
                case VerificationInformation.NotFinishedDestination:
                    DeleteDestination(list);
                    StartRollover(list);
                    DeleteSources(list);
                    return;
                case VerificationInformation.RolloverComplete:
                    File.Delete(m_savedFileName);
                    return;
            }
        }

        void DeleteDestination(ArchiveList<TKey, TValue> list)
        {

        }
        void DeleteSources(ArchiveList<TKey, TValue> list)
        {

        }
        void StartRollover(ArchiveList<TKey, TValue> list)
        {

        }


    }
}
