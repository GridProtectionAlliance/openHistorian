//******************************************************************************************************
//  SubFileMetaDataTest.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  11/23/2011 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace openHistorian.V2.FileStructure
{
    [TestClass()]
    public class SubFileMetaDataTest
    {
        [TestMethod()]
        public void Test()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
            Random rand = new Random();
            int fileIdNumber = rand.Next(int.MaxValue);
            Guid fileExtension = Guid.NewGuid();
            int fileFlags = rand.Next(int.MaxValue);
            int dataBlock1 = rand.Next(int.MaxValue);
            int singleRedirect = rand.Next(int.MaxValue);
            int doubleRedirect = rand.Next(int.MaxValue);
            int tripleRedirect = rand.Next(int.MaxValue);

            SubFileMetaData node = new SubFileMetaData(fileIdNumber, fileExtension, AccessMode.ReadWrite);
            node.FileFlags = fileFlags;
            node.DirectBlock = dataBlock1;
            node.SingleIndirectBlock = singleRedirect;
            node.DoubleIndirectBlock = doubleRedirect;
            node.TripleIndirectBlock = tripleRedirect;
            SubFileMetaData node2 = SaveItem(node);

            if (node2.FileIdNumber != fileIdNumber) throw new Exception();
            if (node2.FileExtension != fileExtension) throw new Exception();
            if (node2.FileFlags != fileFlags) throw new Exception();
            if (node2.DirectBlock != dataBlock1) throw new Exception();
            if (node2.SingleIndirectBlock != singleRedirect) throw new Exception();
            if (node2.DoubleIndirectBlock != doubleRedirect) throw new Exception();
            if (node2.TripleIndirectBlock != tripleRedirect) throw new Exception();
            Assert.IsTrue(true);

            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        }
      
        private static SubFileMetaData SaveItem(SubFileMetaData node)
        {
            //Serialize the header
            MemoryStream stream = new MemoryStream();
            node.Save(new BinaryWriter(stream));

            stream.Position = 0;
            //load the header
            SubFileMetaData node2 = new SubFileMetaData(new BinaryReader(stream), AccessMode.ReadOnly);

            CheckEqual(node2, node);

            SubFileMetaData node3 = node2.CloneEditable();

            CheckEqual(node2, node3);
            return node3;

        }

        static internal void CheckEqual(SubFileMetaData RO, SubFileMetaData RW)
        {
            if (!AreEqual(RO, RW)) throw new Exception();
        }

        /// <summary>
        /// Determines if the two objects are equal in value.
        /// </summary>
        /// <param name="a">the object to compare this class to</param>
        /// <returns></returns>
        /// <remarks>A debug function</remarks>
        static internal bool AreEqual(SubFileMetaData a, SubFileMetaData b)
        {
            if (a == null || b == null)
                return false;

            if (b.FileIdNumber != a.FileIdNumber) return false;
            if (b.FileExtension != a.FileExtension) return false;
            if (b.FileFlags != a.FileFlags) return false;
            if (b.DirectBlock != a.DirectBlock) return false;
            if (b.SingleIndirectBlock != a.SingleIndirectBlock) return false;
            if (b.DoubleIndirectBlock != a.DoubleIndirectBlock) return false;
            if (b.TripleIndirectBlock != a.TripleIndirectBlock) return false;
            return true;
        }

    }
}
