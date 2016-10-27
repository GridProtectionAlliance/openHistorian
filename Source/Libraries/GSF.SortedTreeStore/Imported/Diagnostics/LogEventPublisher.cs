//******************************************************************************************************
//  LogEventPublisher.cs - Gbtc
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

// ReSharper disable InconsistentlySynchronizedField

namespace GSF.Diagnostics
{
    public sealed class LogEventPublisher
    {
        private LogPublisher m_publisher;
        private LogEventPublisherInternal m_internalPublisher;

        internal LogEventPublisher(LogPublisher publisher, LogEventPublisherInternal internalPublisher)
        {
            m_publisher = publisher;
            m_internalPublisher = internalPublisher;
        }

        /// <summary>
        /// Gets if this publisher has any subscribers to it. This does not have to be checked as messages won't route if there are not subscribers. This is mainly
        /// used to skip the code that would generate the data for the <see cref="LogMessage"/>.
        /// </summary>
        public bool HasSubscribers => m_internalPublisher.HasSubscribers;

        /// <summary>
        /// Raises a log message with the provided data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="details">A long text field with the details of the message.</param>
        /// <param name="exception">An exception object if one is provided.</param>
        public void Publish(string message = null, string details = null, Exception exception = null)
        {
            m_internalPublisher.Publish(message, details, exception, m_publisher.InitialStackMessages, m_publisher.InitialStackTrace);
        }
    }
}