//******************************************************************************************************
//  DiskIOMemoryStream.cs - Gbtc
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
//  1/1/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace openHistorian.Core.StorageSystem.File
{
    internal class DiskIOMemoryStream : DiskIOBase
    {
        protected List<byte[]> m_dataBytes;

        public DiskIOMemoryStream()
        {
            m_dataBytes = new List<byte[]>();
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        protected override long SetFileLength(long requestedSize)
        {
             while (FileSize < requestedSize)
            {
                m_dataBytes.Add(null);
            }
            while (FileSize > requestedSize)
            {
                m_dataBytes.RemoveAt(m_dataBytes.Count - 1);
            }
            return FileSize;
        }

        public override long FileSize
        {
            get { return m_dataBytes.Count * ArchiveConstants.BlockSize; }
        }

        protected override void WriteDataBlock(uint blockIndex, byte[] data)
        {
            while (blockIndex >= m_dataBytes.Count)
            {
                m_dataBytes.Add(null);
            }
            byte[] localPage = m_dataBytes[(int)blockIndex];
            if (localPage == null)
            {
                localPage = new byte[ArchiveConstants.BlockSize];
                m_dataBytes[(int)blockIndex] = localPage;
            }
            Array.Copy(data, 0, localPage, 0, data.Length);
        }

        protected override IOReadState ReadBlock(uint blockIndex, byte[] data)
        {
            if (blockIndex > m_dataBytes.Count)
                return IOReadState.ReadPastThenEndOfTheFile;

            byte[] localPage = m_dataBytes[(int)blockIndex];
            if (localPage == null)
                return IOReadState.ChecksumInvalidBecausePageIsNull;

            Array.Copy(localPage, 0, data, 0, data.Length);
            return IOReadState.Valid;
        }

  
    }
}
