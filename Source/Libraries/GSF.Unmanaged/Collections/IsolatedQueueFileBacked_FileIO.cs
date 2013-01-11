//******************************************************************************************************
//  IsolatedQueueFileBacked_FileIO.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/4/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace openHistorian.Collections
{
    public partial class IsolatedQueueFileBacked<T>
    {
        /// <summary>
        /// Does the disk related IO functionality. Also reads existing files on a restore. 
        /// </summary>
        internal class FileIO
        {
            object m_syncRoot;
            Queue<string> m_allFiles;
            long m_sizeOfAllFiles;
            string m_path;
            const string Extension = ".dat";
            const string WorkingExtension = ".working";

            public FileIO(string pathName)
            {
                m_syncRoot = new object();
                m_allFiles = new Queue<string>();
                m_path = pathName;

                var files = Directory.GetFiles(pathName, "*" + Extension).ToList();
                files.Sort();
                files.ForEach(x => m_sizeOfAllFiles += new FileInfo(x).Length);
                files.ForEach(x => m_allFiles.Enqueue(x));
            }

            /// <summary>
            /// Creates a new archive file from the data in the queue.
            /// </summary>
            /// <param name="queue">The nodes to dump.</param>
            public void DumpToDisk(IEnumerable<IsolatedNode<T>> queue)
            {
                string file, fileTemp;
                GetFiles(out file, out fileTemp);
                using (var fs = new FileStream(fileTemp, FileMode.CreateNew, FileAccess.Write))
                {
                    DumpToDisk(fs, queue);
                }
                File.Move(fileTemp, file);
                lock (m_syncRoot)
                {
                    m_allFiles.Enqueue(file);
                    m_sizeOfAllFiles += new FileInfo(file).Length;
                }
            }

            internal void DumpToDisk(Stream stream, IEnumerable<IsolatedNode<T>> queue)
            {
                T item = default(T);
                var wr = new BinaryWriter(stream);

                wr.Write("IsolatedQueueFileBacked".ToCharArray());
                wr.Write((byte)1);

                foreach (var node in queue)
                {
                    wr.Write(node.Count);
                    while (node.TryDequeue(out item))
                    {
                        item.Save(wr);
                    }
                }
                wr.Write(-1);
            }

            /// <summary>
            /// Reads the next available block from the disk and adds it to the provided queue.
            /// </summary>
            /// <param name="queue">the queue to add new blocks to</param>
            /// <param name="instance">A function to call that will construct an IsolatedNode.</param>
            public void ReadFromDisk(ContinuousQueue<IsolatedNode<T>> queue, Func<IsolatedNode<T>> instance)
            {
                string file;
                lock (m_syncRoot)
                {
                    file = m_allFiles.Dequeue();
                    m_sizeOfAllFiles -= new FileInfo(file).Length;
                }

                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    ReadFromDisk(fs, queue, instance);
                }
                File.Delete(file);
            }

            internal void ReadFromDisk(Stream stream, ContinuousQueue<IsolatedNode<T>> queue, Func<IsolatedNode<T>> instance)
            {
                T item = default(T);
                var rd = new BinaryReader(stream);

                string header = new string(rd.ReadChars(23));
                if (header != "IsolatedQueueFileBacked")
                    throw new VersionNotFoundException("File type is not recgonized.");

                byte version = rd.ReadByte();
                if (version != 1)
                    throw new VersionNotFoundException("Version is unknown");

                IsolatedNode<T> node = instance();
                node.Reset();
                queue.Enqueue(node);

                int count = rd.ReadInt32();
                {
                    while (count >= 0)
                    {
                        while (count > 0)
                        {
                            count--;
                            item.Load(rd);
                            if (node.IsHeadFull)
                            {
                                node = instance();
                                node.Reset();
                                queue.Enqueue(node);
                            }
                            node.Enqueue(item);
                        }
                        count = rd.ReadInt32();
                    }
                }
                node.FlagAsFull();
            }

            /// <summary>
            /// Gets the number of files that can be loaded. 
            /// </summary>
            public int FileCount
            {
                get
                {
                    return m_allFiles.Count;
                }
            }

            /// <summary>
            /// Gets the number of bytes in every file in this buffer.
            /// </summary>
            public long FileSizes
            {
                get
                {
                    return m_sizeOfAllFiles;
                }
            }

            void GetFiles(out string file, out string workingFile)
            {
            TryAgain:
                string fileName = DateTime.UtcNow.Ticks.ToString();
                file = Path.Combine(m_path, fileName + Extension);
                workingFile = Path.Combine(m_path, fileName + WorkingExtension);

                if (File.Exists(file) || File.Exists(workingFile))
                    goto TryAgain;
            }
        }
    }
}
