//******************************************************************************************************
//  Logger.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  4/11/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using NUnit.Framework;

namespace GSF.Diagnostics.Test
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void Test1()
        {
            var sd1 = new SourceDetails("This is source 1");
            var sd2 = new SourceDetails("This is source 2");

            Logger log = new Logger();
            log.ReportToConsole(VerboseLevel.All);

            var reporter = log.Register(sd1);
            reporter.Publish(VerboseLevel.Fatal, "Name", "Message", "Details", new Exception("This is an error"));

            var reporter2 = reporter.LogPublisherDetails.Register(sd2);
            reporter2.Publish(VerboseLevel.Fatal, "Name2", "Message2");

            var reporter3 = reporter2.LogPublisherDetails.Register(this);
            sd2 = null;
            GC.Collect();

            reporter3.Publish(VerboseLevel.Debug, "Missing SD2");

            if (sd1.GetSourceDetails().GetHashCode() == 0)
                throw new Exception();

        }

        [Test]
        public void Test2()
        {
            var sd1 = new SourceDetails("This is source 1");
            var sd2 = new SourceDetails("This is source 2");

            Logger log = new Logger();
            log.ReportToConsole(VerboseLevel.All);
            var handler = log.CreateHandler();
            handler.Log += handler_Log;
            handler.Verbose = VerboseLevel.Fatal | VerboseLevel.Debug;

            var reporter = log.Register(sd1);
            reporter.Publish(VerboseLevel.Fatal, "Name", "Message", "Details", new Exception("This is an error"));

            var reporter2 = reporter.LogPublisherDetails.Register(sd2);
            reporter2.Publish(VerboseLevel.Information, "Name2", "Message2");

            var reporter3 = reporter2.LogPublisherDetails.Register(this);
            sd2 = null;
            GC.Collect();

            reporter3.Publish(VerboseLevel.Debug, "Missing SD2");

            if (sd1.GetSourceDetails().GetHashCode() == 0)
                throw new Exception();
            if (handler.GetHashCode() != handler.GetHashCode())
                throw new Exception();

        }

        void handler_Log(LogMessage logMessage)
        {
            System.Console.WriteLine("Begin Handle**********");
            System.Console.WriteLine(logMessage.GetMessage(false));
            System.Console.WriteLine("End Handle**********");
        }

        private class SourceDetails : ILogSourceDetails
        {
            private string m_details;

            public SourceDetails(string message)
            {
                m_details = message;
            }

            public string GetSourceDetails()
            {
                return m_details;
            }
        }

    }
}
