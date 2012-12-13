//******************************************************************************************************
//  ConvertArchiveFile.cs - Gbtc
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
//  ----------------------------------------------------------------------------------------------------
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Collections.KeyValue;
using openHistorian.IO.Unmanaged;
using openHistorian.Server.Database.Archive;

namespace openHistorian.Utility
{
    public static class ConvertArchiveFile
    {
        public static unsafe void ConvertVersion1File(string oldFileName, string newFileName, CompressionMethod compressionMethod)
        {

            if (!File.Exists(oldFileName))
                throw new ArgumentException("Old file does not exist", "oldFileName");
            if (File.Exists(newFileName))
                throw new ArgumentException("New file already exists", "newFileName");
            
            using (var bs0 = new BinaryStream())
            {
                var tree0 = new SortedTree256(bs0, 4096);
                var hist = new OldHistorianReader(oldFileName);
                Action<OldHistorianReader.Points> del = (x) =>
                {
                    tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
                };
                hist.Read(del);

                using (ArchiveFile file1 = ArchiveFile.CreateFile(newFileName, compressionMethod))
                using (var edit1 = file1.BeginEdit())
                {
                    var scan0 = tree0.GetDataRange();
                    scan0.SeekToKey(0, 0);
                    ulong key1, key2, value1, value2;
                    while (scan0.GetNextKey(out key1, out key2, out value1, out value2))
                    {
                        edit1.AddPoint(key1, key2, value1, value2);
                    }

                    edit1.Commit();
                }
            }
        }
    }
}
