//******************************************************************************************************
//  ConvertArchiveFile.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.IO;
using openHistorian.Collections;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;

namespace openHistorian.Utility
{
    public static class ConvertArchiveFile
    {
        public static unsafe void ConvertVersion1File(string oldFileName, string newFileName, Guid compressionMethod, long max = long.MaxValue)
        {
            //Guid compressionMethod = CreateFixedSizeNode.TypeGuid;

            if (!File.Exists(oldFileName))
                throw new ArgumentException("Old file does not exist", "oldFileName");
            if (File.Exists(newFileName))
                throw new ArgumentException("New file already exists", "newFileName");

            using (var bs0 = new BinaryStream())
            {
                var key = new HistorianKey();
                var value = new HistorianValue();
                var tree0 = SortedTree<HistorianKey, HistorianValue>.Create(bs0, 4096, SortedTree.FixedSizeNode);
                var hist = new OldHistorianReader(oldFileName);

                float count = 0;
                Func<OldHistorianReader.Points, bool> del = (x) =>
                {
                    //if (*(uint*)&x.Value == 0)
                    //    return true;
                    //if (*(uint*)&x.Value > (1<<30))
                    //    return true;

                    count++;
                    if (count > max)
                        return false;
                    key.Timestamp = (ulong)x.Time.Ticks;
                    key.PointID = (ulong)x.PointID;
                    value.Value3 = x.flags;
                    value.Value1 = *(uint*)&x.Value;
                    if (!tree0.TryAdd(key, value))
                        count--;
                    return true;
                };
                hist.Read(del);

                using (var file1 = SortedTreeFile.CreateFile(newFileName))
                using (var table = file1.OpenOrCreateTable<HistorianKey, HistorianValue>(compressionMethod))
                using (var edit1 = table.BeginEdit())
                {
                    var scan0 = tree0.CreateTreeScanner();
                    scan0.SeekToStart();
                    while (scan0.Read(key, value))
                    {
                        edit1.AddPoint(key, value);
                    }

                    var range = edit1.GetRange();
                    range.SeekToStart();
                    while (range.Read(key, value))
                        ;

                    edit1.Commit();

                }
            }
        }
    }
}

