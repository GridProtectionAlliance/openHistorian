////******************************************************************************************************
////  IsolatedQueueFileBacked_FileIO.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/4/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;
//using GSF.Text;

//namespace GSF.Collections
//{
//    public partial class IsolatedQueueFileBacked<T>
//    {
//        /// <summary>
//        /// Does the disk related IO functionality. Also reads existing files on a restore. 
//        /// </summary>
//        internal class FileIO
//        {
//            private readonly string m_filePrefix;
//            private readonly object m_syncRoot;
//            private readonly Queue<string> m_allFiles;
//            private long m_sizeOfAllFiles;
//            private readonly string m_path;
//            private const string Extension = ".dat";
//            private const string WorkingExtension = ".working";

//            public FileIO(string pathName, string filePrefix)
//            {
//                m_filePrefix = filePrefix;
//                m_syncRoot = new object();
//                m_allFiles = new Queue<string>();
//                m_path = pathName;

//                Func<string, bool> whereClause = s =>
//                {
//                    string name = Path.GetFileNameWithoutExtension(s).Substring(filePrefix.Length).Trim();
//                    long value;
//                    return long.TryParse(name, out value);
//                };

//                List<string> files = Directory.GetFiles(pathName, filePrefix + " *" + Extension).Where(whereClause).ToList();
//                files.Sort(new NaturalComparer());
//                files.ForEach(x => m_sizeOfAllFiles += new FileInfo(x).Length);
//                files.ForEach(x => m_allFiles.Enqueue(x));
//            }

//            /// <summary>
//            /// Creates a new archive file from the data in the queue.
//            /// </summary>
//            /// <param name="queue">The nodes to dump.</param>
//            /// <param name="appendToEnd">true to write the file as the next sequential file number,
//            /// false to force the number to be before all other numbers 
//            /// and thus be imported first next time it restarts</param>
//            public void DumpToDisk(IEnumerable<IsolatedNode<T>> queue, bool appendToEnd = true)
//            {
//                string file, fileTemp;
//                long fileSize;
//                GetFiles(out file, out fileTemp, appendToEnd);
//                using (FileStream fs = new FileStream(fileTemp, FileMode.CreateNew, FileAccess.Write))
//                {
//                    DumpToDisk(fs, queue);
//                    fileSize = fs.Length;
//                }
//                File.Move(fileTemp, file);
//                lock (m_syncRoot)
//                {
//                    m_allFiles.Enqueue(file);
//                    m_sizeOfAllFiles += fileSize;
//                }
//            }

//            internal void DumpToDisk(Stream stream, IEnumerable<IsolatedNode<T>> queue)
//            {
//                T item = default(T);
//                BinaryWriter wr = new BinaryWriter(stream);

//                wr.Write("IsolatedQueueFileBacked".ToCharArray());
//                wr.Write((byte)1);

//                foreach (IsolatedNode<T> node in queue)
//                {
//                    int count = node.Count;
//                    wr.Write(count);
//                    while (node.TryDequeue(out item))
//                    {
//                        count--;
//                        item.Save(wr);
//                    }
//                    if (count != 0)
//                        throw new Exception("The node was modified while being serialized.");
//                }
//                wr.Write(-1);
//            }

//            /// <summary>
//            /// Reads the next available block from the disk and adds it to the provided queue.
//            /// </summary>
//            /// <param name="queue">the queue to add new blocks to</param>
//            /// <param name="instance">A function to call that will construct an IsolatedNode.</param>
//            public void ReadFromDisk(ContinuousQueue<IsolatedNode<T>> queue, Func<IsolatedNode<T>> instance)
//            {
//                string file;
//                lock (m_syncRoot)
//                {
//                    file = m_allFiles.Dequeue();
//                    m_sizeOfAllFiles -= new FileInfo(file).Length;
//                }

//                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
//                {
//                    ReadFromDisk(fs, queue, instance);
//                }
//                File.Delete(file);
//            }

//            internal void ReadFromDisk(Stream stream, ContinuousQueue<IsolatedNode<T>> queue, Func<IsolatedNode<T>> instance)
//            {
//                T item = default(T);
//                BinaryReader rd = new BinaryReader(stream);

//                string header = new string(rd.ReadChars(23));
//                if (header != "IsolatedQueueFileBacked")
//                    throw new VersionNotFoundException("File type is not recgonized.");

//                byte version = rd.ReadByte();
//                if (version != 1)
//                    throw new VersionNotFoundException("Version is unknown");

//                IsolatedNode<T> node = instance();
//                node.Reset();
//                queue.Enqueue(node);

//                int count = rd.ReadInt32();
//                {
//                    while (count >= 0)
//                    {
//                        while (count > 0)
//                        {
//                            count--;
//                            item.Load(rd);
//                            if (node.IsHeadFull)
//                            {
//                                node = instance();
//                                node.Reset();
//                                queue.Enqueue(node);
//                            }
//                            node.Enqueue(item);
//                        }
//                        count = rd.ReadInt32();
//                    }
//                }
//                node.FlagAsFull();
//            }

//            /// <summary>
//            /// Gets the number of files that can be loaded. 
//            /// </summary>
//            public int FileCount
//            {
//                get
//                {
//                    return m_allFiles.Count;
//                }
//            }

//            /// <summary>
//            /// Gets the number of bytes in every file in this buffer.
//            /// </summary>
//            public long FileSizes
//            {
//                get
//                {
//                    return m_sizeOfAllFiles;
//                }
//            }

//            private void GetFiles(out string file, out string workingFile, bool appendToEnd)
//            {
//                long delta = 0;
//                TryAgain:
//                delta++;
//                long currentTime = DateTime.UtcNow.Ticks;

//                if (m_allFiles.Count > 0)
//                {
//                    if (appendToEnd)
//                    {
//                        string name = Path.GetFileNameWithoutExtension(m_allFiles.Last());
//                        name = name.Substring(m_filePrefix.Length).Trim();
//                        long value = long.Parse(name);
//                        if (value > currentTime)
//                            currentTime = value + delta;
//                    }
//                    else
//                    {
//                        string name = Path.GetFileNameWithoutExtension(m_allFiles.First());
//                        name = name.Substring(m_filePrefix.Length).Trim();
//                        long value = long.Parse(name);
//                        currentTime = value - delta;
//                    }
//                }

//                string fileName = m_filePrefix + " " + currentTime.ToString();

//                file = Path.Combine(m_path, fileName + Extension);
//                workingFile = Path.Combine(m_path, fileName + WorkingExtension);

//                if (File.Exists(file) || File.Exists(workingFile))
//                    goto TryAgain;
//            }
//        }
//    }
//}