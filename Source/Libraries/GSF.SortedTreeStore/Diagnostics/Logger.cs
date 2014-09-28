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
using GSF.Collections;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Manages the collection and reporting of logging information in a system.
    /// </summary>
    public static partial class Logger
    {
        private static bool s_enabled;
        private static object s_syncRoot;
        private static WeakList<InternalSource> s_allSources;
        private static WeakList<InternalSubscriber> s_allSubscribers;
        private static InternalSubscriber s_consoleSubscriber;

        /// <summary>
        /// Gets the root of all source definitions of log messages.
        /// </summary>
        private static readonly InternalSource InternalRootSource;

        /// <summary>
        /// Gets the root of all type definitions of log messages.
        /// </summary>
        private static readonly InternalType InternalRootType;

        /// <summary>
        /// Gets the root of all source definitions of log messages.
        /// </summary>
        public static LogSource RootSource
        {
            get
            {
                return InternalRootSource;
            }
        }

        /// <summary>
        /// Gets the root of all type definitions of log messages.
        /// </summary>
        public static LogType RootType
        {
            get
            {
                return InternalRootType;
            }
        }

        /// <summary>
        /// Gets the console subscriber
        /// </summary>
        public static LogSubscriber ConsoleSubscriber
        {
            get
            {
                return s_consoleSubscriber;
            }       
        }

        static Logger()
        {
            s_enabled = true;

            //When the appdomain unloads, raising messages can be dangerous.
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnProcessExit;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

            s_syncRoot = new object();
            s_allSources = new WeakList<InternalSource>();
            s_allSubscribers = new WeakList<InternalSubscriber>();

            InternalRootType = new InternalType();
            InternalRootSource = new InternalSource(InternalRootType);

            s_consoleSubscriber = new InternalSubscriber();
            s_consoleSubscriber.Verbose = VerboseLevel.None;
            s_consoleSubscriber.Subscribe(RootType);
            s_consoleSubscriber.Subscribe(RootSource);
            s_consoleSubscriber.Log += ConsoleSubscriberOnLog;
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            s_enabled = false;
        }

        private static void Register(InternalSource source)
        {
            s_allSources.Add(source);
        }

        private static void UnRegister(InternalSource source)
        {
            s_allSources.Remove(source);
        }

        private static void Register(InternalSubscriber subscriber)
        {
            s_allSubscribers.Add(subscriber);
        }

        private static void UnRegister(InternalSubscriber subscriber)
        {
            s_allSubscribers.Remove(subscriber);
        }

        private static void RaiseLogMessage(InternalMessage logMessage)
        {
            if (!s_enabled)
                return;

            lock (s_syncRoot)
            {
                if (!s_enabled)
                    return;

                foreach (var sub in s_allSubscribers)
                    sub.BeginLogMessage();

                logMessage.Source.ProcessMessage(logMessage);
                logMessage.Type.ProcessMessage(logMessage);

                foreach (var sub in s_allSubscribers)
                    sub.EndLogMessage();
            }
        }

        private static void RefreshVerbose()
        {
            if (!s_enabled)
                return;

            lock (s_syncRoot)
            {
                InternalRootType.CalculateVerbose(VerboseLevel.None);
                foreach (var publisher in s_allSources)
                    publisher.BeginRecalculateVerbose();
                foreach (var publisher in s_allSources)
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

        private static void ConsoleSubscriberOnLog(LogMessage logMessage)
        {
            System.Console.WriteLine("---------------------------------------------------------");
            string text = logMessage.GetMessage(true);
            System.Console.WriteLine(text);
            System.Console.WriteLine("---------------------------------------------------------");
        }

        /// <summary>
        /// Looks up the type of the log source
        /// </summary>
        /// <param name="type">the type</param>
        /// <returns></returns>
        public static LogType LookupType(Type type)
        {
            string name = type.AssemblyQualifiedName;
            return LookupType(name);
        }

        /// <summary>
        /// Looks up the type of the log source
        /// </summary>
        /// <param name="name">the string name of the type</param>
        /// <returns></returns>
        public static LogType LookupType(string name)
        {
            name = TrimAfterFullName(name);

            string[] parts = name.Split('.', '+');

            var current = InternalRootType;
            foreach (var s in parts)
            {
                current = current.GetOrAddNode(s);
            }
            return current;
        }

        /// <summary>
        /// A source to report log details.
        /// </summary>
        /// <param name="source">The source object of the message. Cannot be null</param>
        /// <param name="parent">The parent source object. May be null.</param>
        /// <param name="logType">The type of the log. If null, the type of <see cref="source"/> is looked up.</param>
        public static LogSource CreateSource(object source, LogSource parent = null, LogType logType = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (parent == null)
                parent = InternalRootSource;

            if (logType == null)
                logType = LookupType(source.GetType());

            InternalSource p = parent as InternalSource;
            InternalType t = logType as InternalType;

            if (p == null)
                throw new InvalidCastException("Custom implementations of LogSource is not permitted");

            if (t == null)
                throw new InvalidCastException("Custom implementations of LogType is not permitted");

            return new InternalSource(source, p, t);
        }

        /// <summary>
        /// Creates a <see cref="LogSubscriber"/>
        /// </summary>
        /// <returns></returns>
        public static LogSubscriber CreateSubscriber()
        {
            return new InternalSubscriber();
        }

        /// <summary>
        /// Trims the unused information after the namespace.class+subclass details.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string TrimAfterFullName(string name)
        {
            int newLength = name.Length;
            int indexOfBracket = name.IndexOf('[');
            int indexOfComma = name.IndexOf(',');

            if (indexOfBracket >= 0)
                newLength = Math.Min(indexOfBracket, newLength);
            if (indexOfComma >= 0)
                newLength = Math.Min(indexOfComma, newLength);
            name = name.Substring(0, newLength).Trim();
            return name;
        }

    }
}
