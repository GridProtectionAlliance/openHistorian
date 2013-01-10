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
using System.Text;

namespace openHistorian.Collections
{
    public partial class IsolatedQueueFileBacked<T>
    {
        internal class FileIO
        {
            Queue<string> m_allFiles;
            string m_path;
            const string Extension = ".dat";
            const string WorkingExtension = ".working";

            public FileIO(string pathName)
            {
                m_allFiles = new Queue<string>();
                m_path = pathName;

                var files = Directory.GetFiles(pathName, "*" + Extension).ToList();
                files.Sort();
                files.ForEach(x => m_allFiles.Enqueue(x));
            }

            /// <summary>
            /// Creates a new archive file from the data in the queue.
            /// </summary>
            /// <param name="queue">The queue to keep.</param>
            /// <param name="countRemaining">The number of items to keep in the queue</param>
            public void DumpToDisk(ContinuousQueue<IsolatedNode<T>> queue, int countRemaining)
            {
                string file, fileTemp;
                GetFiles(out file, out fileTemp);
                using (var fs = new FileStream(fileTemp, FileMode.CreateNew, FileAccess.Write))
                {
                    DumpToDisk(fs, queue, countRemaining);
                }
                File.Move(fileTemp, file);
                m_allFiles.Enqueue(file);
            }

            internal void DumpToDisk(Stream stream, ContinuousQueue<IsolatedNode<T>> queue, int countRemaining)
            {
                T item = default(T);
                var wr = new BinaryWriter(stream);

                wr.Write("IsolatedQueueFileBacked".ToCharArray());
                wr.Write((byte)1);

                while (queue.Count > countRemaining)
                {
                    var node = queue.Dequeue();
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
                var file = m_allFiles.Dequeue();

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
