//******************************************************************************************************
//  LogSource.cs - Gbtc
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
using System.Text;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Identifies the source of a log message.
    /// </summary>
    public class LogSource
    {
        /// <summary>
        /// Represents a source that had no parents
        /// </summary>
        public readonly static LogSource Root = new LogSource("Null", "Logger", "Source is null");

        /// <summary>
        /// Represents a source whose parent was Garbage Collected
        /// </summary>
        public readonly static LogSource Orphan = new LogSource("Orphan", "Logger", "Source was garbage collected");

        /// <summary>
        /// A callback to get additional details
        /// </summary>
        Func<string> m_getDetails;

        WeakReference m_parent;

        /// <summary>
        /// The object reference
        /// </summary>
        public object Reference { get; private set; }

        /// <summary>
        /// The friendly name of the sender.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The classification of the sender.
        /// </summary>
        public string Classification { get; private set; }

        /// <summary>
        /// A link to the log's parent so a stack trace can be computed.
        /// </summary>
        public LogSource Parent
        {
            get
            {
                LogSource parent = m_parent.Target as LogSource;
                if (parent == null)
                    return Orphan;
                return parent;
            }
        }

        /// <summary>
        /// Gets additional metadata about the source. 
        /// </summary>
        /// <returns></returns>
        public string GetDetails()
        {
            if (m_getDetails == null)
                return string.Empty;
            try
            {
                var details = m_getDetails();
                if (details == null)
                    return string.Empty;
                return details;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private LogSource(string name, string classification, string details)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (classification == null)
                throw new ArgumentNullException("classification");

            Reference = this;
            Name = name;
            Classification = classification;

            m_parent = new WeakReference(this);
            m_getDetails = () => details;
        }

        /// <summary>
        /// Creates a <see cref="LogSource"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        /// <param name="classification"></param>
        /// <param name="parent"></param>
        /// <param name="getDetails"></param>
        public LogSource(object sender, string name, string classification, Func<string> getDetails, LogSource parent)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            if (name == null)
                throw new ArgumentNullException("name");
            if (classification == null)
                throw new ArgumentNullException("classification");
            if (parent == null)
                parent = Root;

            m_parent = new WeakReference(parent);
            Reference = sender;
            Name = name;
            Classification = classification;
            m_getDetails = getDetails;
        }

        /// <summary>
        /// Gets a string representation of this source
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        public string GetString(bool stackTrace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Classification + Name);
            string details = GetDetails();
            if (details != string.Empty)
                sb.AppendLine(details);
            if (stackTrace && this != Root && this != Orphan)
            {
                sb.AppendLine("Parent");
                sb.AppendLine(Parent.GetString(true));
            }
            return sb.ToString();
        }
    }
}