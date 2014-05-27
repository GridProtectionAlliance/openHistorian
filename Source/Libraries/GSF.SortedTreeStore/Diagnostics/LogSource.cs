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
using System.Collections;
using System.Text;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Identifies the source of a log message.
    /// </summary>
    public class LogSource
    {
        /// <summary>
        /// A custom log message
        /// </summary>
        private class CustomSourceDetails : ILogSourceDetails
        {
            private string m_detailMessage;
            public CustomSourceDetails(string detailMessage)
            {
                m_detailMessage = detailMessage;
            }
            public string GetSourceDetails()
            {
                return m_detailMessage;
            }
        }

        /// <summary>
        /// A link to the log's parent so a stack trace can be computed. 
        /// <see cref="ParentNull"/> if the parent supplied was null.
        /// </summary>
        public readonly LogSource Parent;
        /// <summary>
        /// The name of the source type.
        /// </summary>
        public readonly string TypeName;

        private readonly WeakReference m_source;

        /// <summary>
        /// Gets the <see cref="Logger"/> that this source belongs to.
        /// </summary>
        internal readonly Logger Logger;

        /// <summary>
        /// The object reference. <see cref="SourceCollected"/> if the source
        /// has been collected.
        /// </summary>
        public object Source
        {
            get
            {
                return m_source.Target ?? SourceCollected;
            }
        }

        /// <summary>
        /// Gets additional metadata about the source. 
        /// </summary>
        /// <returns></returns>
        public string GetDetails()
        {
            ILogSourceDetails details = Source as ILogSourceDetails;
            if ((object)details == null)
                return string.Empty;
            try
            {
                return details.GetSourceDetails();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates a <see cref="LogSource"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <param name="logger"></param>
        public LogSource(object source, LogSource parent, Logger logger)
        {
            if ((object)logger == null)
                logger = Logger.Default;

            if (source == null)
                throw new ArgumentNullException("source");
            if (parent == null)
                parent = ParentNull;

            // In the special case where the sender is ParentNullDetails, 
            // then create a circular reference so Parent is never null.
            if ((object)source == ParentNullDetails)
                parent = this;

            Logger = logger;
            Parent = parent;
            m_source = new WeakReference(source);
            TypeName = source.GetType().FullName;

        }

        /// <summary>
        /// Gets a string representation of this source
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        public void AppendString(StringBuilder sb, bool stackTrace)
        {
            sb.Append("   at: ");
            sb.AppendLine(TypeName);
            if (stackTrace)
            {
                string details = GetDetails();
                if (details != string.Empty)
                {
                    sb.Append("      // ");
                    sb.AppendLine(details);
                }
                if (Parent != ParentNull)
                {
                    Parent.AppendString(sb, true);
                }
            }
           
        }

        #region [ Static ]

        /// <summary>
        /// Strong references to these source details, as LogSource only maintains weak references.
        /// </summary>
        private readonly static CustomSourceDetails ParentNullDetails = new CustomSourceDetails("Parent is null");
        private readonly static CustomSourceDetails SourceCollectedDetails = new CustomSourceDetails("Source was garbage collected");

        /// <summary>
        /// Represents a source that had no parents
        /// </summary>
        public readonly static LogSource ParentNull = new LogSource(ParentNullDetails, null, null);

        /// <summary>
        /// Represents a source who was Garbage Collected
        /// </summary>
        public readonly static LogSource SourceCollected = new LogSource(SourceCollectedDetails, null, null);

        #endregion
    }

    /// <summary>
    /// Extention methods for <see cref="LogSource"/>
    /// </summary>
    public static class LogSourceExtensions
    {
        /// <summary>
        /// Registers the provided source using this <see cref="LogSource"/> as the parent.
        /// </summary>
        /// <param name="parent">The parent. If Null, uses <see cref="Logger.Default"/> to register a new item</param>
        /// <param name="source">The source object</param>
        /// <returns>A <see cref="LogReporter"/>that the source can use to raise logs events.</returns>
        public static LogReporter Register(this LogSource parent, object source)
        {
            if ((object)parent != null)
                return parent.Logger.Register(source, parent);
            return Logger.Default.Register(source, null);
        }
    }
}