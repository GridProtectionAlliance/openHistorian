//******************************************************************************************************
//  LogSubscriptionConsole.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// Creates a <see cref="LogSubscriber"/> that will write messages to the <see cref="Console"/>
    /// </summary>
    public class LogSubscriptionConsole 
    {
        private LogSubscriber m_subscriber;
        private VerboseLevel m_verbose;
        /// <summary>
        /// Creates a LogFileWriter that initially queues message
        /// </summary>
        public LogSubscriptionConsole()
        {
            m_verbose = VerboseLevel.None;
            m_subscriber = Logger.CreateSubscriber(m_verbose);
            m_subscriber.SubscribeToAll(m_verbose);
            m_subscriber.NewLogMessage += SubscriberNewLogMessage;
        }

        /// <summary>
        /// Gets or sets verbosity level for this <see cref="LogSubscriptionConsole"/>.
        /// To disable the console from receiving messages, set to <see cref="VerboseLevel.None"/>
        /// </summary>
        public VerboseLevel Verbose
        {
            get
            {
                return m_verbose;
            }
            set
            {
                m_verbose = value;
                m_subscriber.SubscribeToAll(m_verbose);
            }
        }

        private void SubscriberNewLogMessage(LogMessage logMessage)
        {
            string text = "---------------------------------------------------------" + Environment.NewLine + logMessage.GetMessage() + Environment.NewLine + "---------------------------------------------------------";
            System.Console.WriteLine(text);
        }
    }
}
