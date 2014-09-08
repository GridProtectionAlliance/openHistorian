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
using System.ComponentModel;
using System.Net.Mime;
using System.Threading;
using GSF.Collections;

namespace GSF.Diagnostics
{
    /// <summary>
    /// A log message delegate
    /// </summary>
    /// <param name="logMessage"></param>
    public delegate void LogEventHandler(LogMessage logMessage);

    /// <summary>
    /// Manages the collection and reporting of logging information in a system.
    /// </summary>
    public static class Logger
    {
        private static WeakList<LogSource> s_allPublishers;
        private static WeakList<LogSubscriber> s_allSubscribers;
        private static object s_syncRoot;
        private static int s_initialized;
        private static bool s_enabled;

        /// <summary>
        /// Due to inter static dependencies, we must initialize 
        /// <see cref="Logger"/>, <see cref="LogType"/>, and <see cref="LogSource"/>
        /// in a controlled manner.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static void Initialize()
        {
            if (s_initialized != 0)
                return;
            if (Interlocked.CompareExchange(ref s_initialized, 1, 0) == 0)
            {
                s_enabled = true;

                AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnProcessExit;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
                
                s_syncRoot = new object();
                s_allPublishers = new WeakList<LogSource>();
                s_allSubscribers = new WeakList<LogSubscriber>();

                LogType.Initialize();
                LogSource.Initialize();

                s_consoleSubscriber = new LogSubscriber();
                s_consoleSubscriber.Verbose = VerboseLevel.None;
                s_consoleSubscriber.Subscribe(LogType.Root);
                s_consoleSubscriber.Subscribe(LogSource.Root);
                s_consoleSubscriber.Log += ConsoleSubscriberOnLog;
                UniversalSource = new LogSource(new object(), null, null);
            }

        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            s_enabled = false;
        }

        private static void ConsoleSubscriberOnLog(LogMessage logMessage)
        {
            System.Console.WriteLine("---------------------------------------------------------");
            string text = logMessage.GetMessage(true);
            System.Console.WriteLine(text);
            System.Console.WriteLine("---------------------------------------------------------");
        }

        /// <summary>
        /// Allows general logging if source data cannot be provided.
        /// </summary>
        /// <remarks>
        /// For certain classes, the overhead of creating log messages
        /// may not be desired. However, there are still occurance when
        /// logging a message is still desired. Therefore this class exists
        /// 
        /// An example use case is when exception code is generated at a very low level,
        /// but these details would like to be bubbled to the message log.
        /// </remarks>
        public static LogSource UniversalSource { get; private set; }

        private static LogSubscriber s_consoleSubscriber;

        /// <summary>
        /// Creates a <see cref="Logger"/>
        /// </summary>
        static Logger()
        {
            Initialize();
        }

        static internal void Register(LogSource source)
        {
            s_allPublishers.Add(source);
        }

        static internal void Register(LogSubscriber subscriber)
        {
            s_allSubscribers.Add(subscriber);
        }

        static internal void UnRegister(LogSource source)
        {
            s_allPublishers.Remove(source);
        }

        static internal void UnRegister(LogSubscriber subscriber)
        {
            s_allSubscribers.Remove(subscriber);
        }

        internal static void RaiseLogMessage(LogMessage logMessage)
        {
            if (!s_enabled)
                return;

            lock (s_syncRoot)
            {
                foreach (var sub in s_allSubscribers)
                    sub.BeginLogMessage();

                logMessage.Source.ProcessMessage(logMessage);
                logMessage.Source.LogType.ProcessMessage(logMessage);

                foreach (var sub in s_allSubscribers)
                    sub.EndLogMessage();
            }
        }

        internal static void RefreshVerbose()
        {
            if (!s_enabled)
                return;

            lock (s_syncRoot)
            {
                LogType.RefreshVerbose();
                foreach (var publisher in s_allPublishers)
                    publisher.BeginRecalculateVerbose();
                foreach (var publisher in s_allPublishers)
                    publisher.EndRecalculateVerbose();
            }
        }

        /// <summary>
        /// Indicates that messages will automatically be reported to the console.
        /// </summary>
        /// <param name="level"></param>
        public static void ReportToConsole(VerboseLevel level)
        {
            s_consoleSubscriber.Verbose = level;
        }

    }
}
