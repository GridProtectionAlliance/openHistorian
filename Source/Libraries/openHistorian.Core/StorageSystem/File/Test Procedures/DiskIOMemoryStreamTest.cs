//******************************************************************************************************
//  DiskIOMemoryStreamTest.cs - Gbtc
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
    internal class DiskIOMemoryStreamTest : DiskIOMemoryStream
    {
        internal static void Test()
        {
            TestAllReadStates();
        }
        static void TestAllReadStates()
        {
            DiskIOMemoryStreamTest stream = new DiskIOMemoryStreamTest();
            for (int x = 0; x < 2; x++) 
            {
                DiskIOTest.TestAllReadStatesExceptInvalid(stream);
                TestChecksumInvalid(stream);
            }
        }
        static void TestChecksumInvalid(DiskIOMemoryStreamTest stream)
        {
            IOReadState readState;
            int seed = (int)DateTime.Now.Ticks;
            byte[] buffer = DiskIOTest.GenerateRandomDataBlock(seed);
            uint currentBlock = (uint)(stream.FileSize / ArchiveConstants.BlockSize);

            stream.WriteBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);

            byte[] internalBlock = stream.m_dataBytes[(int)currentBlock];

            internalBlock[0] = (byte)((int)internalBlock[0] + 1);

            readState = stream.ReadBlock(currentBlock, BlockType.FileAllocationTable, 1, 2, 3, buffer);
            if (readState != IOReadState.ChecksumInvalid)
                throw new Exception();
        }
    }
}
