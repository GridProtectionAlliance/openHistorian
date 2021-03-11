//******************************************************************************************************
//  ResourceQueueTest.cs - Gbtc
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.Collections.Test
{
    [TestFixture()]
    public class ResourceQueueTest
    {
        [Test()]
        public void Test()
        {
            MemoryPoolTest.TestMemoryLeak();
            int x = 0;
            ResourceQueue<string> queue = new ResourceQueue<string>(() => (x++).ToString(), 3, 4);

            x = 10;

            Assert.AreEqual(queue.Dequeue(), "0");
            Assert.AreEqual(queue.Dequeue(), "1");
            Assert.AreEqual(queue.Dequeue(), "2");
            Assert.AreEqual(queue.Dequeue(), "10");
            Assert.AreEqual(queue.Dequeue(), "11");

            queue.Enqueue("0");
            queue.Enqueue("1");
            Assert.AreEqual(queue.Dequeue(), "0");
            queue.Enqueue("3");
            Assert.AreEqual(queue.Dequeue(), "1");
            Assert.AreEqual(queue.Dequeue(), "3");
            Assert.AreEqual(queue.Dequeue(), "12");

            queue.Enqueue("1");
            queue.Enqueue("2");
            queue.Enqueue("3");
            queue.Enqueue("4");
            queue.Enqueue("5");
            Assert.AreEqual(queue.Dequeue(), "1");
            Assert.AreEqual(queue.Dequeue(), "2");
            Assert.AreEqual(queue.Dequeue(), "3");
            Assert.AreEqual(queue.Dequeue(), "4");
            Assert.AreEqual(queue.Dequeue(), "13");
            MemoryPoolTest.TestMemoryLeak();
        }
    }
}