//******************************************************************************************************
//  Logger.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Manages the collection and reporting of logging information in a system.
    /// </summary>
    public static class Logger
    {
        private static LoggerInternal s_logger;

        public static readonly LogSubscriptionConsole Console;
        public static readonly LogSubscriptionFileWriter FileWriter;

        private static readonly ConcurrentDictionary<int, int> RecursiveChecking = new ConcurrentDictionary<int, int>();
        private static readonly Dictionary<int, List<LogStackMessages>> ThreadStackDetails = new Dictionary<int, List<LogStackMessages>>();

        private static readonly LogPublisher Log;
        private static readonly LogEventPublisher EventFirstChanceException;
        private static readonly LogEventPublisher EventAppDomainException;

        static Logger()
        {
            //Initializes the empty object of StackTraceDetails
            LogStackTrace.Initialize();
            LogStackMessages.Initialize();

            s_logger = new LoggerInternal();

            Console = new LogSubscriptionConsole();
            FileWriter = new LogSubscriptionFileWriter(1000);

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
                AppDomain.CurrentDomain.ProcessExit += DomainUnload;
            else
                AppDomain.CurrentDomain.DomainUnload += DomainUnload;

            Log = CreatePublisher(typeof(Logger), MessageClass.Component);
            Log.InitialStackTrace = LogStackTrace.Empty;
            EventFirstChanceException = Log.RegisterEvent(MessageLevel.NA, MessageFlags.SystemHealth, "First Chance App Domain Exception", 30, MessageRate.PerSecond(30), 1000);
            EventAppDomainException = Log.RegisterEvent(MessageLevel.Critical, MessageFlags.SystemHealth, "Unhandled App Domain Exception");
        }

        static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            int threadid = Thread.CurrentThread.ManagedThreadId;

            if (RecursiveChecking.TryAdd(threadid, 1))
            {
                try
                {
                    EventFirstChanceException.Publish(null, null, e.Exception);
                }
                finally
                {
                    int value;
                    RecursiveChecking.TryRemove(threadid, out value);
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            int threadid = Thread.CurrentThread.ManagedThreadId;

            if (RecursiveChecking.TryAdd(threadid, 1))
            {
                try
                {
                    EventAppDomainException.Publish(null, null, e.ExceptionObject as Exception);
                }
                finally
                {
                    int value;
                    RecursiveChecking.TryRemove(threadid, out value);
                }
            }
        }

        private static void DomainUnload(object sender, EventArgs e)
        {
            try
            {
                s_logger.Dispose();
                Console.Verbose = VerboseLevel.None;
                FileWriter.Dispose();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Looks up the type of the log source
        /// </summary>
        /// <param name="type">the type</param>
        /// <param name="classification">the classification of the type of messages that this publisher will raise.</param>
        /// <returns></returns>
        public static LogPublisher CreatePublisher(Type type, MessageClass classification)
        {
            return new LogPublisher(s_logger.CreateType(type), classification);
        }

        /// <summary>
        /// Creates a <see cref="LogSubscriber"/>
        /// </summary>
        /// <returns></returns>
        public static LogSubscriber CreateSubscriber(VerboseLevel level = VerboseLevel.None)
        {
            var subscriber = new LogSubscriber(s_logger.CreateSubscriber());
            subscriber.SubscribeToAll(level);
            return subscriber;
        }

        /// <summary>
        /// Searches the current stack frame for all related messages that will be published with this message.
        /// </summary>
        /// <returns></returns>
        public static LogStackMessages GetStackMessages()
        {
            int threadid = Thread.CurrentThread.ManagedThreadId;

            List<LogStackMessages> stack;

            lock (ThreadStackDetails)
            {
                if (!ThreadStackDetails.TryGetValue(threadid, out stack))
                {
                    return LogStackMessages.Empty;
                }
            }

            return new LogStackMessages(stack);
        }


        /// <summary>
        /// Temporarily appends data to the thread's stack so the data can be propagated to any messages generated on this thread.
        /// Be sure to call Dispose on the returned object to remove this from the stack.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public static StackDetailsDisposal AppendStackDetails(LogStackMessages messages)
        {
            int threadid = Thread.CurrentThread.ManagedThreadId;

            List<LogStackMessages> stack;

            lock (ThreadStackDetails)
            {
                if (!ThreadStackDetails.TryGetValue(threadid, out stack))
                {
                    stack = new List<LogStackMessages>();
                    ThreadStackDetails.Add(threadid, stack);
                }
            }

            stack.Add(messages);
            return new StackDetailsDisposal(stack.Count);
        }

        public struct StackDetailsDisposal : IDisposable
        {
            public int Depth { get; private set; }

            public StackDetailsDisposal(int depth)
            {
                Depth = depth;
            }

            public void Dispose()
            {
                if (Depth == 0)
                    return;
                int threadid = Thread.CurrentThread.ManagedThreadId;

                if (Depth == 1)
                {
                    lock (ThreadStackDetails)
                    {
                        ThreadStackDetails.Remove(threadid);
                    }
                }
                else
                {
                    lock (ThreadStackDetails)
                    {
                        List<LogStackMessages> stack;
                        if (ThreadStackDetails.TryGetValue(threadid, out stack))
                        {
                            while (stack.Count >= Depth)
                            {
                                stack.RemoveAt(stack.Count - 1);
                            }
                        }
                    }
                }
                Depth = 0;
            }
        }
    }



}
