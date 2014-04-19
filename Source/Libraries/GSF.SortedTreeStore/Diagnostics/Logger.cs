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
    /// A log message delegate
    /// </summary>
    /// <param name="logMessage"></param>
    public delegate void LogEventHandler(LogMessage logMessage);

    /// <summary>
    /// A level of verbose for certain error messages.
    /// </summary>
    [Flags]
    public enum VerboseLevel : int
    {
        None = 0,
        Debug = 1,
        Information = 2,
        Warning = 4,
        Error = 8,
        Fatal = 16,
        All = -1
    }


    /// <summary>
    /// Manages the collection and reporting of logging information in a system.
    /// </summary>
    public class Logger
    {
        public readonly static Logger Default = new Logger();

        object m_syncRoot;
        WeakList<LogReporter> m_logReportList;
        WeakList<LogHandler> m_logHandlerList;
        VerboseLevel m_consoleLevel;
        public Logger()
        {
            m_consoleLevel = VerboseLevel.None;
            m_syncRoot = new object();
            m_logReportList = new WeakList<LogReporter>();
            m_logHandlerList = new WeakList<LogHandler>();
        }

        /// <summary>
        /// Registers components that can raise log messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        /// <param name="classification"></param>
        /// <param name="getDetails"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public LogReporter Register(object sender, string name, string classification, Func<string> getDetails = null, LogSource parent = null)
        {
            LogReporter reporter = new LogReporter(this, sender, name, classification, getDetails, parent);
            lock (m_syncRoot)
            {
                m_logReportList.Add(reporter);
            }
            RefreshVerbose();
            return reporter;
        }

        /// <summary>
        /// Indicates that messages will automatically be reported to the console.
        /// </summary>
        /// <param name="level"></param>
        public void ReportToConsole(VerboseLevel level)
        {
            m_consoleLevel = level;
            RefreshVerbose();
        }

        /// <summary>
        /// Registers a callback that will handle system events.
        /// </summary>
        /// <returns></returns>
        public LogHandler Handle()
        {
            var handler = new LogHandler(this);
            lock (m_syncRoot)
            {
                m_logHandlerList.Add(handler);
            }
            return handler;
        }

        internal void RefreshVerbose()
        {
            VerboseLevel level = m_consoleLevel;

            lock (m_syncRoot)
            {
                foreach (var handler in m_logHandlerList)
                {
                    level |= handler.Verbose;
                }

                foreach (var reporter in m_logReportList)
                {
                    reporter.Verbose = level;
                }
            }
        }

        internal void RaiseMessage(LogMessage message)
        {
            lock (m_syncRoot)
            {
                if (m_consoleLevel != VerboseLevel.None)
                {
                    if ((message.Level & m_consoleLevel) > 0)
                    {
                        try
                        {
                            System.Console.WriteLine("---------------------------------------------------------");
                            System.Console.WriteLine(message.GetMessage(true));
                            System.Console.WriteLine("---------------------------------------------------------");
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                foreach (var handler in m_logHandlerList)
                {
                    handler.ProcessMessage(message);
                }
            }
        }

    }
}
