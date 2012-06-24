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
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using openHistorian.V2.IO.Unmanaged;

//namespace openHistorian.V2.FileSystem
//{
//    [TestClass()]
//    public class FileAllocationTableTest
//    {
//        [TestMethod()]
//        public void Test()
//        {
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            DiskIo data = new DiskIo(new MemoryStream(), 0);
//            FileAllocationTable header = FileAllocationTable.CreateFileAllocationTable(data);

//            header.CreateNewFile(Guid.NewGuid());
//            header.CreateNewFile(Guid.NewGuid());
//            header.CreateNewFile(Guid.NewGuid());
//            header.WriteToFileSystem(data);

//            FileAllocationTable header2 = FileAllocationTable.OpenHeader(data);

//            CheckEqual(header2, header);
//            header = header2.CloneEditableCopy(false);
//            CheckEqual(header2, header);

//            Assert.IsTrue(true);
//            data.Dispose();
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            //verify they are the same;
//        }

//        private static void CheckEqual(FileAllocationTable RO, FileAllocationTable RW)
//        {
//            if (!RO.AreEqual(RW))
//                throw new Exception();
//        }

//#if DEBUG

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        /// <remarks>A debug function</remarks>
//        internal bool AreEqual(FileAllocationTable other)
//        {
//            if (other == null)
//                return false;
//            if (m_isTableCompromised != other.m_isTableCompromised) return false;
//            if (m_minimumReadVersion != other.m_minimumReadVersion) return false;
//            if (m_minimumWriteVersion != other.m_minimumWriteVersion) return false;
//            if (m_archiveId != other.m_archiveId) return false;
//            if (m_isOpenedForExclusiveEditing != other.m_isOpenedForExclusiveEditing) return false;
//            if (m_snapshotSequenceNumber != other.m_snapshotSequenceNumber) return false;
//            if (m_nextFileId != other.m_nextFileId) return false;
//            if (m_lastAllocatedBlock != other.m_lastAllocatedBlock) return false;
//            //compare files.
//            if (m_files == null)
//            {
//                if (other.m_files != null) return false;
//            }
//            else
//            {
//                if (other.m_files == null) return false;
//                if (m_files.Count != other.m_files.Count) return false;
//                for (int x = 0; x < m_files.Count; x++)
//                {
//                    FileMetaData file = m_files[x];
//                    FileMetaData fileOther = other.m_files[x];

//                    if (file == null)
//                    {
//                        if (fileOther != null) return false;
//                    }
//                    else
//                    {
//                        if (fileOther == null) return false;
//                        if (!file.AreEqual(fileOther)) return false;
//                    }
//                }
//            }
//            //compare unrecgonized meta data
//            if (m_unrecgonizedMetaDataTags == null)
//            {
//                if (other.m_unrecgonizedMetaDataTags != null) return false;
//            }
//            else
//            {
//                if (other.m_unrecgonizedMetaDataTags == null) return false;
//                if (m_unrecgonizedMetaDataTags.Count != other.m_unrecgonizedMetaDataTags.Count) return false;
//                for (int x = 0; x < m_unrecgonizedMetaDataTags.Count; x++)
//                {
//                    byte[] file = m_unrecgonizedMetaDataTags[x];
//                    byte[] fileOther = other.m_unrecgonizedMetaDataTags[x];

//                    if (file == null)
//                    {
//                        if (fileOther != null) return false;
//                    }
//                    else
//                    {
//                        if (fileOther == null) return false;

//                        if (file.Length != fileOther.Length) return false;

//                        if (!file.SequenceEqual(fileOther))
//                            return false;
//                    }
//                }
//            }

//            return true;
//        }

//#endif
//    }
//}
