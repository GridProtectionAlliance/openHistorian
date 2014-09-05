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
    /// Manages the collection and reporting of logging information in a system.
    /// </summary>
    public class Logger : ILogSourceDetails
    {
        /// <summary>
        /// Gets the default system logger.
        /// </summary>
        public readonly static Logger Default = new Logger();

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
        public readonly LogPublisher UniversalPublisher;

        object m_syncRoot;
        WeakList<LogPublisher> m_logReportList;
        WeakList<LogSubscriber> m_logHandlerList;
        VerboseLevel m_consoleLevel;


        /// <summary>
        /// Creates a <see cref="Logger"/>
        /// </summary>
        public Logger()
        {
            m_consoleLevel = VerboseLevel.None;
            m_syncRoot = new object();
            m_logReportList = new WeakList<LogPublisher>();
            m_logHandlerList = new WeakList<LogSubscriber>();
            UniversalPublisher = Register(this);
        }

        /// <summary>
        /// Registers components that can raise log messages.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public LogPublisher Register(object source, LogPublisherDetails parent = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            LogPublisher publisher = new LogPublisher(this, source, parent);
            lock (m_syncRoot)
            {
                m_logReportList.Add(publisher);
            }
            RefreshVerboseFilter();
            return publisher;
        }

        /// <summary>
        /// Indicates that messages will automatically be reported to the console.
        /// </summary>
        /// <param name="level"></param>
        public void ReportToConsole(VerboseLevel level)
        {
            m_consoleLevel = level;
            RefreshVerboseFilter();
        }

        /// <summary>
        /// Registers a callback that will handle system events.
        /// </summary>
        /// <returns></returns>
        public LogSubscriber CreateHandler()
        {
            var handler = new LogSubscriber(this);
            lock (m_syncRoot)
            {
                m_logHandlerList.Add(handler);
            }
            return handler;
        }

        /// <summary>
        /// Refreshes the verbose level filter. 
        /// </summary>
        internal void RefreshVerboseFilter()
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

        string ILogSourceDetails.GetSourceDetails()
        {
            return "Universal Source";
        }
    }
}
