//******************************************************************************************************
//  LogSubscriber.cs - Gbtc
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
    /// Subscribes to log events.
    /// </summary>
    public abstract class LogSubscriber
    {
        /// <summary>
        /// Event handler for the logs that are raised
        /// </summary>
        public event LogEventHandler Log;

        VerboseLevel m_verbose;

        /// <summary>
        /// Gets/Sets the verbose level of this subscriber
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                if (m_verbose != value)
                {
                    m_verbose = value;
                    OnVerboseChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when the verbose level changes
        /// </summary>
        protected abstract void OnVerboseChanged();
        /// <summary>
        /// Subscribes to the supplied <see cref="source"/>
        /// </summary>
        /// <param name="source"></param>
        public abstract void Subscribe(LogSource source);
        /// <summary>
        /// Unsubscribes from all instances of <see cref="source"/>
        /// </summary>
        /// <param name="source"></param>
        public abstract void Unsubscribe(LogSource source);

        /// <summary>
        /// Subscribes to the supplied <see cref="LogType"/>
        /// </summary>
        /// <param name="type"></param>
        public abstract void Subscribe(LogType type);
        /// <summary>
        /// Unsubscribes from all instances of <see cref="type"/>
        /// </summary>
        /// <param name="type"></param>
        public abstract void Unsubscribe(LogType type);

        /// <summary>
        /// Ignores all messages that are raised by this type.
        /// </summary>
        /// <param name="type">the type or namespace</param>
        public abstract void AddIgnored(LogType type);
        /// <summary>
        /// Removes a previously ignored type.
        /// </summary>
        /// <param name="type">the type or namespace</param>
        public abstract void RemoveIgnored(LogType type);


        /// <summary>
        /// Raises the <see cref="Log"/> event.
        /// </summary>
        /// <param name="logMessage">the message to raise.</param>
        protected void OnLog(LogMessage logMessage)
        {
            if (logMessage == null)
                return;
            if ((logMessage.Level & Verbose) == 0)
                return;

            try
            {
                if (Log != null)
                    Log(logMessage);
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

    }
}