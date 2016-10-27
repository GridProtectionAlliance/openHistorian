//******************************************************************************************************
//  LogStackFrame.cs - Gbtc
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
using System.Security;
using System.Text;
using GSF.IO;
using GSF.Reflection;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Provides information about the specific stack frame.
    /// </summary>
    public class LogStackFrame
        : IEquatable<LogStackFrame>
    {
        /// <summary>
        /// The name of the method's type.
        /// </summary>
        /// <remarks>
        /// Example: System.Collections.Generic.List`1+Enumerable
        /// </remarks>
        public readonly string ClassName;
        /// <summary>
        /// The name of the method
        /// </summary>
        /// <remarks>
        /// Example: MethodName&lt;TKey,TValue&gt;(String key, String value)
        /// </remarks>
        public readonly string MethodName;

        /// <summary>
        /// The file name if debug symbols were compiled with the assembly
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Gets the offset position in the IL Code.
        /// </summary>
        public readonly int NativeOffset;

        /// <summary>
        /// The line number of the data.
        /// </summary>
        public readonly int LineNumber;

        /// <summary>
        /// The column number of the execution point.
        /// </summary>
        public readonly int ColumnNumber;

        /// <summary>
        /// Creates a <see cref="LogStackFrame"/> from a <see cref="StackFrame"/>
        /// </summary>
        /// <param name="frame"></param>
        public LogStackFrame(StackFrame frame)
        {
            if (frame == null)
            {
                ClassName = string.Empty;
                MethodName = string.Empty;
                FileName = string.Empty;
                NativeOffset = 0;
                LineNumber = 0;
                ColumnNumber = 0;
                return;
            }

            ClassName = frame.GetMethod().GetFriendlyClassName();
            MethodName = frame.GetMethod().GetFriendlyMethodName();
            try
            {
                FileName = frame.GetFileName() ?? string.Empty;
            }
            catch (SecurityException)
            {
                FileName = string.Empty;
            }
            NativeOffset = frame.GetNativeOffset();
            LineNumber = frame.GetFileLineNumber();
            ColumnNumber = frame.GetFileColumnNumber();
        }

        /// <summary>
        /// Creates a <see cref="LogStackFrame"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">the stream to load from</param>
        public LogStackFrame(Stream stream)
        {
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    ClassName = stream.ReadString();
                    MethodName = stream.ReadString();
                    FileName = stream.ReadString();
                    NativeOffset = stream.ReadInt32();
                    LineNumber = stream.ReadInt32();
                    ColumnNumber = stream.ReadInt32();
                    break;
                default:
                    throw new VersionNotFoundException("Unknown StackTraceDetails Version");
            }
        }

        /// <summary>
        /// Saves this class to a <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">the stream to write to.</param>
        public void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(ClassName);
            stream.Write(MethodName);
            stream.Write(FileName);
            stream.Write(NativeOffset);
            stream.Write(LineNumber);
            stream.Write(ColumnNumber);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Appends the details of this stack frame to the provided <see cref="stringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">where to append this class information</param>
        /// <remarks>
        /// Does not append a <see cref="Environment.NewLine"/> to the end of the line.
        /// </remarks>
        public void ToString(StringBuilder stringBuilder)
        {
            if (ClassName.Length > 0)
            {
                stringBuilder.Append(ClassName);
                stringBuilder.Append('.');
            }
            stringBuilder.Append(MethodName);
            if (FileName.Length == 0)
            {
                stringBuilder.Append(" IL offset: ");
                stringBuilder.Append(NativeOffset);
            }
            else
            {
                stringBuilder.Append(" in ");
                stringBuilder.Append(FileName);
                stringBuilder.Append(": line ");
                stringBuilder.Append(LineNumber);
                stringBuilder.Append(':');
                stringBuilder.Append(ColumnNumber);
            }
        }

        /// <summary>
        /// Gets the hash code data from this frame
        /// </summary>
        internal int ComputeHashCode()
        {
            return ClassName.GetHashCode() ^ MethodName.GetHashCode() ^ FileName.GetHashCode() ^ NativeOffset.GetHashCode() ^ LineNumber.GetHashCode() ^ ColumnNumber.GetHashCode();
        }

        /// <summary>
        /// Gets of the two classes are equal
        /// </summary>
        /// <param name="other">the class to compare.</param>
        /// <returns></returns>
        public bool Equals(LogStackFrame other)
        {
            return (object)other != null &&
                NativeOffset == other.NativeOffset &&
                LineNumber == other.LineNumber &&
                ColumnNumber == other.ColumnNumber &&
                ClassName == other.ClassName &&
                MethodName == other.MethodName &&
                FileName == other.FileName;
        }

    }
}
