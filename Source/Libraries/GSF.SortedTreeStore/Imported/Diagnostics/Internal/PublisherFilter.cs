//******************************************************************************************************
//  PublisherFilter.cs - Gbtc
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Describes a kind of publisher. This can be All, or a subset based on Assembly (dll) or Type (namespace/type)
    /// </summary>
    internal class PublisherFilter
    {
        /// <summary>
        /// The general classification of the subscription.
        /// </summary>
        private enum FilterType
        {
            /// <summary>
            /// This will subscribe to anything, regardless of matching assembly or type. 
            /// </summary>
            Universal = 0,
            /// <summary>
            /// This subscription matches the name of the assembly.
            /// </summary>
            Assembly = 1,
            /// <summary>
            /// This subscription matches the full type name.
            /// </summary>
            Type = 2,
        }

        /// <summary>
        /// The type of the filter.
        /// </summary>
        private readonly FilterType m_filterType;

        /// <summary>
        /// The text associated with the filter;
        /// </summary>
        private readonly string m_text;

        /// <summary>
        /// Filter is an expression, this means the name contains a wildcard (* or ?)
        /// </summary>
        private readonly bool m_isExpression;

        /// <summary>
        /// A <see cref="Regex"/> for matching publishers if the publisher is an expression.
        /// </summary>
        private readonly Regex m_regexMatch;

        /// <summary>
        /// Creates a either Universal or LogSource Topic
        /// </summary>
        private PublisherFilter(FilterType filterType)
        {
            if (filterType == FilterType.Universal)
            {
                m_text = "Universal";
            }
            else
            {
                throw new InvalidEnumArgumentException(nameof(filterType), (int)filterType, typeof(FilterType));
            }
            m_filterType = filterType;
            m_isExpression = false;
            m_regexMatch = null;
        }

        /// <summary>
        /// Creates an Assembly, Type.
        /// </summary>
        private PublisherFilter(FilterType filterType, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text), "cannot be null or whitespace");

            if (filterType != FilterType.Assembly && filterType != FilterType.Type)
                throw new InvalidEnumArgumentException(nameof(filterType), (int)filterType, typeof(FilterType));

            m_filterType = filterType;
            m_text = text;
            m_isExpression = text.Contains('*') || text.Contains('?');
            m_regexMatch = null;

            if (m_isExpression)
            {
                var regexMatchString = "^" + Regex.Escape(text).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                m_regexMatch = new Regex(regexMatchString, RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// Gets if this subscription contains the publisher.
        /// </summary>
        /// <param name="publisher">the publisher to check</param>
        /// <returns></returns>
        public bool ContainsPublisher(LogPublisherInternal publisher)
        {
            if (publisher == null)
                throw new ArgumentNullException(nameof(publisher));

            switch (m_filterType)
            {
                case FilterType.Universal:
                    return true;
                case FilterType.Assembly:
                    if (m_isExpression)
                        return m_regexMatch.IsMatch(publisher.AssemblyFullName);
                    return publisher.AssemblyFullName.Equals(m_text, StringComparison.OrdinalIgnoreCase);
                case FilterType.Type:
                    if (m_isExpression)
                        return m_regexMatch.IsMatch(publisher.TypeFullName);
                    return publisher.TypeFullName.Equals(m_text, StringComparison.OrdinalIgnoreCase);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool ContainsTheSameLogSearchCriteria(PublisherFilter other)
        {
            return m_filterType == other.m_filterType && m_text == other.m_text;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            switch (m_filterType)
            {
                case FilterType.Universal:
                    return "Universal";
                case FilterType.Type:
                    return "Type:" + m_text;
                case FilterType.Assembly:
                    return "Assembly:" + m_text;
            }
            return m_text;
        }

        /// <summary>
        /// Creates a type topic on a specified type.
        /// </summary>
        public static PublisherFilter CreateType(Type type)
        {
            if ((object)type == null)
                throw new ArgumentNullException(nameof(type));

            //Create and add if not exists
            string name = type.AssemblyQualifiedName;
            name = TrimAfterFullName(name);
            return CreateType(name);
        }

        /// <summary>
        /// Creates a topic from the specified list
        /// </summary>
        /// <returns></returns>
        public static PublisherFilter CreateType(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Cannot be null or whitespace");
            if (name != name.Trim())
                throw new ArgumentException("Names cannot begin or end with whitespace characters: " + name, nameof(name));

            return new PublisherFilter(FilterType.Type, name);
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

        #region [ Assembly ]

        /// <summary>
        /// Creates a topic from the specified assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static PublisherFilter CreateAssembly(Assembly assembly)
        {
            if ((object)assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            string path = assembly.Location;
            string name = Path.GetFileName(path);
            return CreateAssembly(name);
        }

        /// <summary>
        /// Creates a topic from the specified assembly file name.
        /// </summary>
        /// <param name="name">the assembly file name (.dll or .exe) </param>
        /// <returns></returns>
        public static PublisherFilter CreateAssembly(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Cannot be null or whitespace");
            if (name != name.Trim())
                throw new ArgumentException("Names cannot begin or end with whitespace characters: " + name, nameof(name));

            return new PublisherFilter(FilterType.Assembly, name);
        }

        #endregion

        /// <summary>
        /// Creates a universal topic.
        /// </summary>
        /// <returns></returns>
        public static PublisherFilter CreateUniversal()
        {
            return new PublisherFilter(FilterType.Universal);
        }

    }
}