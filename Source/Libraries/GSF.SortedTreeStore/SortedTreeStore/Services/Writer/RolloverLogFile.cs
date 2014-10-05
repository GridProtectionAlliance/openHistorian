//******************************************************************************************************
//  RolloverLogFile.cs - Gbtc
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
//  03/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.Diagnostics;
using GSF.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// Logs the rollover process so that it can be properly finished in the event of a power outage or process crash
    /// </summary>
    public class RolloverLogFile
    {

        private static readonly LogType Log = Logger.LookupType(typeof(RolloverLogFile));

        /// <summary>
        /// Gets if the file is valid and not corrupt.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// Gets all of the source files.
        /// </summary>
        public readonly List<Guid> SourceFiles = new List<Guid>();
        /// <summary>
        /// Gets the destination file
        /// </summary>
        public readonly Guid DestinationFile;

        /// <summary>
        /// Gets the filename of this log file. String.Empty if not currently associated with a file.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Creates a new rollover log
        /// </summary>
        /// <param name="fileName">the name of the file to save</param>
        /// <param name="sourceFiles">the source files in the rollover process</param>
        /// <param name="destinationFile">the destination file in the rollover process.</param>
        public RolloverLogFile(string fileName, List<Guid> sourceFiles, Guid destinationFile)
        {
            SourceFiles = sourceFiles;
            DestinationFile = destinationFile;
            IsValid = true;
            FileName = fileName;

            var stream = new MemoryStream();
            stream.Write(Header);
            stream.Write((byte)1);
            stream.Write(SourceFiles.Count);
            foreach (var file in SourceFiles)
            {
                stream.Write(file);
            }
            stream.Write(destinationFile);
            using (var sha = new SHA1Managed())
            {
                stream.Write(sha.ComputeHash(stream.ToArray()));
            }
            File.WriteAllBytes(fileName, stream.ToArray());
        }

        /// <summary>
        /// Resumes a rollover log
        /// </summary>
        /// <param name="fileName">the name of the log file to load.</param>
        public RolloverLogFile(string fileName)
        {
            FileName = fileName;
            SourceFiles.Clear();
            IsValid = false;

            try
            {
                byte[] data = File.ReadAllBytes(fileName);
                if (data.Length < Header.Length + 1 + 20) //Header + Version + SHA1
                {
                    Log.Publish(VerboseLevel.Warning, "Failed to load file.", "Expected file length is not long enough", fileName);
                    return;
                }
                for (int x = 0; x < Header.Length; x++)
                {
                    if (data[x] != Header[x])
                    {
                        Log.Publish(VerboseLevel.Warning, "Failed to load file.", "Incorrect File Header", fileName);
                        return;
                    }
                }

                byte[] hash = new byte[20];
                Array.Copy(data, data.Length - 20, hash, 0, 20);
                using (var sha = new SHA1Managed())
                {
                    var checksum = sha.ComputeHash(data, 0, data.Length - 20);
                    if (!hash.SequenceEqual(checksum))
                    {
                        Log.Publish(VerboseLevel.Warning, "Failed to load file.", "Hashsum failed.", fileName);
                        return;
                    }
                }

                var stream = new MemoryStream(data);

                stream.Position = Header.Length;

                int version = stream.ReadNextByte();
                switch (version)
                {
                    case 1:
                        int count = stream.ReadInt32();
                        while (count > 0)
                        {
                            count--;
                            SourceFiles.Add(stream.ReadGuid());
                        }
                        DestinationFile = stream.ReadGuid();
                        IsValid = true;
                        return;
                    default:
                        Log.Publish(VerboseLevel.Warning, "Failed to load file.", "Version Not Recgonized.", fileName);
                        SourceFiles.Clear();
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Warning, "Failed to load file.", "Unexpected Error", fileName, ex);
                SourceFiles.Clear();
                return;
            }
        }

        /// <summary>
        /// Recovers this rollover during an application crash.
        /// </summary>
        /// <param name="list"></param>
        public void Recover(ArchiveList list)
        {
            using (var edit = list.AcquireEditLock())
            {
                //If the destination file exists, the rollover is complete. Therefore remove any source file.
                if (edit.Contains(DestinationFile))
                {
                    foreach (var source in SourceFiles)
                    {
                        if (edit.Contains(source))
                            edit.TryRemoveAndDelete(source);
                    }
                }
                //Otherwise, delete the destination file (which is allow the ~d2 cleanup to occur).
            }
            Delete();
        }

        /// <summary>
        /// Deletes the file associated with this ArchiveLog
        /// </summary>
        public void Delete()
        {
            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Error, "Could not delete file: " + FileName, null, null, ex);
            }
        }

        static readonly byte[] Header;
        static RolloverLogFile()
        {
            Header = System.Text.Encoding.UTF8.GetBytes("Historian 2.0 Rollover Log");
        }
    }
}
