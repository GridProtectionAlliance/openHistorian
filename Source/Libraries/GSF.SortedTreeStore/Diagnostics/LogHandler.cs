//******************************************************************************************************
//  LogHandler.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// A handler of log events.
    /// </summary>
    public class LogHandler
    {
        /// <summary>
        /// Event handler for the logs that are raised
        /// </summary>
        public event LogEventHandler Log;
        /// <summary>
        /// The main logger
        /// </summary>
        public Logger Logger;

        VerboseLevel m_verbose;

        internal LogHandler(Logger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Gets if this handler will receive messages of this type
        /// </summary>
        public bool ReportDebug { get; private set; }
        /// <summary>
        /// Gets if this handler will receive messages of this type
        /// </summary>
        public bool ReportInfo { get; private set; }
        /// <summary>
        /// Gets if this handler will receive messages of this type
        /// </summary>
        public bool ReportWarning { get; private set; }
        /// <summary>
        /// Gets if this handler will receive messages of this type
        /// </summary>
        public bool ReportError { get; private set; }
        /// <summary>
        /// Gets if this handler will receive messages of this type
        /// </summary>
        public bool ReportFatal { get; private set; }

        /// <summary>
        /// Gets/Sets the verbose level of this log handler
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                ReportDebug = (value & VerboseLevel.Debug) != 0;
                ReportInfo = (value & VerboseLevel.Information) != 0;
                ReportWarning = (value & VerboseLevel.Warning) != 0;
                ReportError = (value & VerboseLevel.Error) != 0;
                ReportFatal = (value & VerboseLevel.Fatal) != 0;
                m_verbose = value;
                Logger.RefreshVerbose();
            }
        }

        /// <summary>
        /// Messages to raise
        /// </summary>
        /// <param name="log"></param>
        internal void ProcessMessage(LogMessage log)
        {
            if (log.Level == VerboseLevel.Debug && !ReportDebug)
                return;
            if (log.Level == VerboseLevel.Information && !ReportInfo)
                return;
            if (log.Level == VerboseLevel.Warning && !ReportWarning)
                return;
            if (log.Level == VerboseLevel.Error && !ReportError)
                return;
            if (log.Level == VerboseLevel.Fatal && !ReportFatal)
                return;

            try
            {
                if (Log != null)
                    Log(log);
            }
            catch (Exception)
            {
            }
        }
    }
}