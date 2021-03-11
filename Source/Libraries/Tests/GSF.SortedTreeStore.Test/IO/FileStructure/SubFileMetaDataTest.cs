//******************************************************************************************************
//  SubFileMetaDataTest.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
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
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    [TestFixture()]
    public class SubFileMetaDataTest
    {
        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            Random rand = new Random();
            ushort fileIdNumber = (ushort)rand.Next(int.MaxValue);
            SubFileName fileName = SubFileName.CreateRandom();
            int dataBlock1 = rand.Next(int.MaxValue);
            int singleRedirect = rand.Next(int.MaxValue);
            int doubleRedirect = rand.Next(int.MaxValue);
            int tripleRedirect = rand.Next(int.MaxValue);
            int quadrupleRedirect = rand.Next(int.MaxValue);

            SubFileHeader node = new SubFileHeader(fileIdNumber, fileName, isImmutable: false, isSimplified:false);
            node.DirectBlock = (uint)dataBlock1;
            node.SingleIndirectBlock = (uint)singleRedirect;
            node.DoubleIndirectBlock = (uint)doubleRedirect;
            node.TripleIndirectBlock = (uint)tripleRedirect;
            node.QuadrupleIndirectBlock = (uint)quadrupleRedirect;
            SubFileHeader node2 = SaveItem(node);

            if (node2.FileIdNumber != fileIdNumber) throw new Exception();
            if (node2.FileName != fileName) throw new Exception();
            if (node2.DirectBlock != dataBlock1) throw new Exception();
            if (node2.SingleIndirectBlock != singleRedirect) throw new Exception();
            if (node2.DoubleIndirectBlock != doubleRedirect) throw new Exception();
            if (node2.TripleIndirectBlock != tripleRedirect) throw new Exception();
            if (node2.QuadrupleIndirectBlock != quadrupleRedirect) throw new Exception();
            Assert.IsTrue(true);

            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

        private static SubFileHeader SaveItem(SubFileHeader node)
        {
            //Serialize the header
            MemoryStream stream = new MemoryStream();
            node.Save(new BinaryWriter(stream));

            stream.Position = 0;
            //load the header
            SubFileHeader node2 = new SubFileHeader(new BinaryReader(stream), isImmutable: true, isSimplified: false);

            CheckEqual(node2, node);

            SubFileHeader node3 = node2.CloneEditable();

            CheckEqual(node2, node3);
            return node3;
        }

        internal static void CheckEqual(SubFileHeader RO, SubFileHeader RW)
        {
            if (!AreEqual(RO, RW)) throw new Exception();
        }

        /// <summary>
        /// Determines if the two objects are equal in value.
        /// </summary>
        /// <param name="a">the object to compare this class to</param>
        /// <returns></returns>
        /// <remarks>A debug function</remarks>
        internal static bool AreEqual(SubFileHeader a, SubFileHeader b)
        {
            if (a is null || b is null)
                return false;

            if (b.FileIdNumber != a.FileIdNumber) return false;
            if (b.FileName != a.FileName) return false;
            if (b.DirectBlock != a.DirectBlock) return false;
            if (b.SingleIndirectBlock != a.SingleIndirectBlock) return false;
            if (b.DoubleIndirectBlock != a.DoubleIndirectBlock) return false;
            if (b.TripleIndirectBlock != a.TripleIndirectBlock) return false;
            if (b.QuadrupleIndirectBlock != a.QuadrupleIndirectBlock) return false;
            return true;
        }
    }
}