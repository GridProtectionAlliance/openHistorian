////******************************************************************************************************
////  FileAllocationTableTest.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
////  12/3/2011 - Steven E. Chisholm
////       Generated original version of source code.
////     
////******************************************************************************************************
//using System;
//using System.IO;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace openHistorian.V2.FileSystem
//{
//    [TestClass()]
//    public class FileAllocationTableTest
//    {
//        [TestMethod()]
//        public void Test()
//        {
//            Assert.AreEqual(openHistorian.V2.UnmanagedMemory.BufferPool.AllocatedBytes, 0L);
//            DiskIoEnhanced data = new DiskIoEnhanced();
//            FileAllocationTable header = FileAllocationTable.CreateFileAllocationTable(data);

//            header.CreateNewFile(Guid.NewGuid());
//            header.CreateNewFile(Guid.NewGuid());
//            header.CreateNewFile(Guid.NewGuid());
//            header.WriteToFileSystem(data);

//            FileAllocationTable header2 = FileAllocationTable.OpenHeader(data);

//            CheckEqual(header2, header);
//            header = header2.CreateEditableCopy(false);
//            CheckEqual(header2, header);
         
//            Assert.IsTrue(true);
//            data.Dispose();
//            Assert.AreEqual(openHistorian.V2.UnmanagedMemory.BufferPool.AllocatedBytes, 0L);
//            //verify they are the same;
//        }

//        private static void CheckEqual(FileAllocationTable RO, FileAllocationTable RW)
//        {
//            if (!RO.AreEqual(RW))
//                throw new Exception();
//        }

//    }
//}
