//******************************************************************************************************
//  LogStackTrace.cs - Gbtc
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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using GSF.Immutable;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Provides stack trace data that can be serialized to a stream.
    /// </summary>
    public class LogStackTrace
        : ImmutableObjectAutoBase<LogStackTrace>, IEquatable<LogStackTrace>
    {
        /// <summary>
        /// Gets the stack frame data
        /// </summary>
        public readonly ImmutableList<LogStackFrame> Frames;

        /// <summary>
        /// Gets the hashcode for this class
        /// </summary>
        private readonly int m_hashCode;

        /// <summary>
        /// Creates the <see cref="LogStackTrace.Empty"/> object
        /// </summary>
        private LogStackTrace()
        {
            Frames = new ImmutableList<LogStackFrame>();
            IsReadOnly = true;
        }

        /// <summary>
        /// Creates a stack trace object
        /// </summary>
        /// <param name="lookupFileInfo">gets if the file paths need to be looked up.</param>
        /// <param name="skipCount">the number of frames to skip.</param>
        /// <param name="maxStackFrames">the maximum number of frames in the stack trace.</param>
        public LogStackTrace(bool lookupFileInfo = true, int skipCount = 0, int maxStackFrames = int.MaxValue)
        {
            var trace = new StackTrace(skipCount + 1, lookupFileInfo);
            Frames = new ImmutableList<LogStackFrame>();
            var frames = trace.GetFrames();
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    if (maxStackFrames <= 0)
                        break;
                    maxStackFrames--;
                    Frames.Add(new LogStackFrame(frame));
                }
            }
            m_hashCode = ComputeHashCode();
            IsReadOnly = true;
        }

        /// <summary>
        /// Loads stack trace information from the supplied <see cref="stream"/>
        /// </summary>
        /// <param name="stream">where to load the stack trace information</param>
        public LogStackTrace(Stream stream)
        {
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    int count = stream.ReadInt32();
                    Frames = new ImmutableList<LogStackFrame>(count);
                    while (count > 0)
                    {
                        count--;
                        Frames.Add(new LogStackFrame(stream));
                    }
                    break;
                default:
                    throw new VersionNotFoundException("Unknown StackTraceDetails Version");
            }
            m_hashCode = ComputeHashCode();
            IsReadOnly = true;
        }

        private int ComputeHashCode()
        {
            int code = Frames.Count;
            foreach (var frame in Frames)
            {
                code ^= frame.ComputeHashCode();
            }
            return code;
        }

        /// <summary>
        /// Saves stack trace information to the supplied <see cref="stream"/>
        /// </summary>
        /// <param name="stream">where to save the stack trace information</param>
        public void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(Frames.Count);
            foreach (var frame in Frames)
            {
                frame.Save(stream);
            }
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
            if (Frames.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var frame in Frames)
            {
                sb.Append("   at ");
                frame.ToString(sb);
                sb.AppendLine();
            }
            return sb.ToString();
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
            return Equals(obj as LogStackTrace);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(LogStackTrace other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if ((object)other == null
                || m_hashCode != other.m_hashCode
                || Frames.Count != other.Frames.Count)
                return false;
            for (int x = 0; x < Frames.Count; x++)
            {
                if (!Frames[x].Equals(other.Frames[x]))
                    return false;
            }
            return true;
        }

        #region [ Static ]

        /// <summary>
        /// An empty stack trace.
        /// </summary>
        public static readonly LogStackTrace Empty;

        static LogStackTrace()
        {
            Empty = new LogStackTrace();
        }

        public static void Initialize()
        {

        }

        #endregion
    }
}
