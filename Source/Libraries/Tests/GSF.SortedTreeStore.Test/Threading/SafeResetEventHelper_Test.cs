//******************************************************************************************************
//  SafeResetEventHelper_Test.cs - Gbtc
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
//  9/13/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.Diagnostics;
using NUnit.Framework;

namespace GSF.Threading
{
    [TestFixture]
    public class SafeResetEventHelper_Test
    {

        [Test]
        public void Test()
        {
            Logger.Console.Verbose = VerboseLevel.All;
            bool state;
            SafeManualResetEvent wait = new SafeManualResetEvent(true);
            wait.WaitOne();
            wait.Reset();
            state = false;
            Run(() => { state = true; wait.Set(); }, 10);
            wait.WaitOne();
            if (!state)
                throw new Exception("Did not wait");
            wait.WaitOne();
            wait.Reset();
            wait.Set();
            wait.WaitOne();
            wait.Reset();
            Run(() => { state = true; wait.Dispose(); }, 10);
            wait.WaitOne();
            wait = wait;
            wait.Reset();
            wait.WaitOne();
            wait.Dispose();
            wait.Set();
            wait.WaitOne();
            wait.Reset();

        }

        [Test]
        public void Test2()
        {
            Logger.Console.Verbose = VerboseLevel.All;
            bool state;
            SafeManualResetEvent wait = new SafeManualResetEvent(false);
            state = false;
            Run(() => { state = true; wait.Set(); }, 10);
            wait.WaitOne();
            if (!state)
                throw new Exception("Did not wait");
            wait.Reset();
            wait.Dispose();
            wait.WaitOne();
            wait.Reset();

        }

        void Run(Action action, int timeInMilliseconds)
        {
            ThreadPool.QueueUserWorkItem(Run, Tuple.Create(action, timeInMilliseconds));
        }

        void Run(object state)
        {
            Tuple<Action, int> data = (Tuple<Action, int>)state;
            Thread.Sleep(data.Item2);
            data.Item1();
        }
    }
}
