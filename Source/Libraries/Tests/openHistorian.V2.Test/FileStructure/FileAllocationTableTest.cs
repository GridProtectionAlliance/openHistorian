//******************************************************************************************************
//  FileAllocationTableTest.cs - Gbtc
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
//  12/3/2011 - Steven E. Chisholm
//       Generated original version of source code.
//     
//******************************************************************************************************
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.FileStructure
{
    [TestClass()]
    public class FileAllocationTableTest
    {
        [TestMethod()]
        public void Test()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
            using (DiskIo data = new DiskIo(new MemoryStream(), 0))
            {
                FileHeaderBlock header = new FileHeaderBlock(data, OpenMode.Create, AccessMode.ReadWrite);

                header.CreateNewFile(Guid.NewGuid());
                header.CreateNewFile(Guid.NewGuid());
                header.CreateNewFile(Guid.NewGuid());
                header.WriteToFileSystem(data);

                FileHeaderBlock header2 = new FileHeaderBlock(data, OpenMode.Open, AccessMode.ReadOnly);

                CheckEqual(header2, header);
                header = header2.CloneEditableCopy();
                FileHeaderBlock_Accessor.AttachShadow(header).m_snapshotSequenceNumber--;
                CheckEqual(header2, header);

            }
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
            //verify they are the same;
        }

        private static void CheckEqual(FileHeaderBlock RO, FileHeaderBlock RW)
        {
            if (!AreEqual(FileHeaderBlock_Accessor.AttachShadow(RW), FileHeaderBlock_Accessor.AttachShadow(RO)))
                throw new Exception();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <remarks>A debug function</remarks>
        static internal bool AreEqual(FileHeaderBlock_Accessor other, FileHeaderBlock_Accessor self)
        {
            if (other == null)
                return false;
            if (self.m_minimumReadVersion != other.m_minimumReadVersion) return false;
            if (self.m_minimumWriteVersion != other.m_minimumWriteVersion) return false;
            if (self.m_archiveId != other.m_archiveId) return false;
            if (self.m_snapshotSequenceNumber != other.m_snapshotSequenceNumber) return false;
            if (self.m_nextFileId != other.m_nextFileId) return false;
            if (self.m_lastAllocatedBlock != other.m_lastAllocatedBlock) return false;
            
            //compare files.
            if (self.m_files == null)
            {
                if (other.m_files != null) return false;
            }
            else
            {
                if (other.m_files == null) return false;
                if (self.m_files.Count != other.m_files.Count) return false;
                for (int x = 0; x < self.m_files.Count; x++)
                {
                    SubFileMetaData subFile = self.m_files[x];
                    SubFileMetaData subFileOther = other.m_files[x];

                    if (subFile == null)
                    {
                        if (subFileOther != null) return false;
                    }
                    else
                    {
                        if (subFileOther == null) return false;
                        if (!FileMetaDataTest.AreEqual(subFile,subFileOther)) return false;
                    }
                }
            }
            //compare unrecgonized meta data
            if (self.m_unrecgonizedMetaDataTags == null)
            {
                if (other.m_unrecgonizedMetaDataTags != null) return false;
            }
            else
            {
                if (other.m_unrecgonizedMetaDataTags == null) return false;
                if (self.m_unrecgonizedMetaDataTags.Count != other.m_unrecgonizedMetaDataTags.Count) return false;
                for (int x = 0; x < self.m_unrecgonizedMetaDataTags.Count; x++)
                {
                    byte[] file = self.m_unrecgonizedMetaDataTags[x];
                    byte[] fileOther = other.m_unrecgonizedMetaDataTags[x];

                    if (file == null)
                    {
                        if (fileOther != null) return false;
                    }
                    else
                    {
                        if (fileOther == null) return false;

                        if (file.Length != fileOther.Length) return false;

                        if (!file.SequenceEqual(fileOther))
                            return false;
                    }
                }
            }

            return true;
        }

    }
}
