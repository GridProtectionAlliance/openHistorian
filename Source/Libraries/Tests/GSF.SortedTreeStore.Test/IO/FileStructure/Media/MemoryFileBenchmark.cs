//******************************************************************************************************
//  MemoryFileBenchmark.cs - Gbtc
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
//  12/2/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF.IO.Unmanaged;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Media.Test
{
    [TestFixture]
    internal class MemoryFileBenchmark
    {
        [Test]
        public void Test1()
        {
            MemoryPoolTest.TestMemoryLeak();
            MemoryPoolFile file = new MemoryPoolFile(Globals.MemoryPool);

            BinaryStreamIoSessionBase session = file.CreateIoSession();

            BlockArguments blockArguments = new BlockArguments();
            blockArguments.IsWriting = true;
            blockArguments.Position = 10000000;
            session.GetBlock(blockArguments);


            System.Console.WriteLine("Get Block\t" + StepTimer.Time(10, () =>
            {
                blockArguments.Position = 100000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 200000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 300000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 400000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 500000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 600000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 700000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 800000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 900000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 1000000;
                session.GetBlock(blockArguments);
            }));
            file.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }
    }
}