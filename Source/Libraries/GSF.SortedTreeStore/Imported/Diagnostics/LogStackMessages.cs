//******************************************************************************************************
//  LogStackMessages.cs - Gbtc
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Immutable List of stack messages
    /// </summary>
    public class LogStackMessages 
        : IEquatable<LogStackMessages>
    {
        private readonly string[] m_attributes;

        private readonly string[] m_values;
        /// <summary>
        /// Gets the hashcode for this class
        /// </summary>
        private readonly long m_hashCode;

        private LogStackMessages()
        {
            m_attributes = EmptyArray<string>.Empty;
            m_values = EmptyArray<string>.Empty;
            m_hashCode = ComputeHashCode();
        }

        public LogStackMessages(List<LogStackMessages> details)
        {
            int cnt = 0;
            for (int x = 0; x < details.Count; x++)
            {
                cnt += details[x].m_attributes.Length;
            }

            m_attributes = new string[cnt];
            m_values = new string[cnt];

            cnt = 0;
            for (int x = 0; x < details.Count; x++)
            {
                var item = details[x];
                for (int y = 0; y < item.m_attributes.Length; y++)
                {
                    m_attributes[cnt] = item.m_attributes[y];
                    m_values[cnt] = item.m_values[y];
                    cnt++;
                }
            }
            m_hashCode = ComputeHashCode();
        }

        public LogStackMessages(Stream stream)
        {
            int version = stream.ReadNextByte();
            if (version == 0)
            {
                int cnt = stream.ReadInt32();
                m_attributes = new string[cnt];
                m_values = new string[cnt];
                for (int x = 0; x < cnt; x++)
                {
                    m_attributes[x] = stream.ReadString();
                    m_values[x] = stream.ReadString();
                }
            }
            else
            {
                throw new VersionNotFoundException();
            }
            m_hashCode = ComputeHashCode();
        }

        private LogStackMessages(LogStackMessages oldMessage, string key, string value)
        {
            int cnt = oldMessage.m_attributes.Length + 1;
            m_attributes = new string[cnt];
            m_values = new string[cnt];

            oldMessage.m_attributes.CopyTo(m_attributes, 0);
            oldMessage.m_values.CopyTo(m_values, 0);

            m_attributes[cnt - 1] = key;
            m_values[cnt - 1] = value;
            m_hashCode = ComputeHashCode();
        }

        private int ComputeHashCode()
        {
            int hashSum = m_attributes.Length;
            for (int x = 0; x < m_attributes.Length; x++)
            {
                hashSum ^= m_attributes[x].GetHashCode();
                hashSum ^= m_values[x].GetHashCode();
            }
            return hashSum;
        }

        public LogStackMessages ConcatenateWith(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return new LogStackMessages(this, key, value);
        }

        public void Save(Stream stream)
        {
            stream.WriteByte(0);
            stream.Write(m_attributes.Length);
            for (int x = 0; x < m_attributes.Length; x++)
            {
                stream.Write(m_attributes[x]);
                stream.Write(m_values[x]);
            }
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (int)m_hashCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Equals(obj as LogStackMessages);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(LogStackMessages other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if ((object)other == null
                || m_hashCode != other.m_hashCode
                || m_attributes.Length != other.m_attributes.Length)
                return false;
            for (int x = 0; x < m_attributes.Length; x++)
            {
                if (m_attributes[x] != other.m_attributes[x])
                    return false;
                if (m_values[x] != other.m_values[x])
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            if (m_attributes.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            for (int x = 0; x < m_attributes.Length; x++)
            {
                sb.Append(m_attributes[x]);
                sb.Append('=');
                sb.Append(m_values[x]);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static readonly LogStackMessages Empty;

        static LogStackMessages()
        {
            Empty = new LogStackMessages();
        }

        internal static void Initialize()
        {

        }

    }
}
