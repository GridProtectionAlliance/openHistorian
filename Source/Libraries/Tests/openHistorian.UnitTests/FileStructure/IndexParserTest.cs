//******************************************************************************************************
//  IndexParserTest.cs - Gbtc
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
//  1/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using NUnit.Framework;
using openHistorian.IO.Unmanaged;

namespace openHistorian.FileStructure.Test
{
    [TestFixture()]
    public class IndexParserTest
    {
        [Test()]
        public void Test()
        {
            int blockSize = 4096;
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
            IndexMapper map = new IndexMapper(blockSize);
            DiskIo stream = new DiskIo(blockSize, new MemoryStream(), 0);
            SubFileMetaData node = new SubFileMetaData(1, Guid.NewGuid(), AccessMode.ReadWrite);
            IndexParser parse = new IndexParser(blockSize, 1, stream, node);
            parse.SetPosition(14312);
            Assert.IsTrue(true);
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        }
    }
}
