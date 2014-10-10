//******************************************************************************************************
//  SequentialSortedTreeWriter_Test.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.Annotations;
using GSF.IO.FileStructure.Media;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Collection;
using GSF.SortedTreeStore.Filters;
using NUnit.Framework;
using openHistorian.Collections;
using openHistorian.SortedTreeStore.Types.CustomCompression.Ts;

namespace GSF.SortedTreeStore.Tree.Specialized
{
    [TestFixture]
    public class SequentialSortedTreeWriter_Test
    {
        [Test]
        public void Test()
        {
            for (int x = 1; x < 10000; x+=10)
            {
                Test(x);
                System.Console.WriteLine(x);
            }

        }

        public void Test(int pointCount)
        {
            SortedPointBuffer<HistorianKey, HistorianValue> points = new SortedPointBuffer<HistorianKey, HistorianValue>(pointCount, true);
            var r = new Random(1);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            for (int x = 0; x < pointCount; x++)
            {
                key.PointID = (ulong)r.Next();
                key.Timestamp = (ulong)r.Next();
                value.Value1 = key.PointID;
                points.TryEnqueue(key, value);
            }

            points.IsReadingMode = true;

            using (var bs = new BinaryStream(true))
            {
                //var tree = new SequentialSortedTreeWriter<HistorianKey, HistorianValue>(bs, 256, SortedTree.FixedSizeNode);
                var tree = new SequentialSortedTreeWriter<HistorianKey, HistorianValue>(bs, 512, CreateTsCombinedEncoding.TypeGuid);
                tree.Build(points);

                var sts = SortedTree<HistorianKey, HistorianValue>.Open(bs);
                r = new Random(1);

                for (int x = 0; x < pointCount; x++)
                {
                    key.PointID = (ulong)r.Next();
                    key.Timestamp = (ulong)r.Next();
                    sts.Get(key, value);
                    if (value.Value1 != key.PointID)
                        throw new Exception();
                }

            }
        }

    }
}
