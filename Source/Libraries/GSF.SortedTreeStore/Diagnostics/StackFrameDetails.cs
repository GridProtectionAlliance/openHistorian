//******************************************************************************************************
//  StackFrameDetails.cs - Gbtc
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
//  11/17/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Data;
using System.Diagnostics;
using System.IO;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Provides information about the specific stack frame.
    /// </summary>
    public class StackFrameDetails
    {
        /// <summary>
        /// The full name of the assembly.
        /// </summary>
        public readonly string AssemblyName;
        /// <summary>
        /// The name of the module
        /// </summary>
        public readonly string ModuleName;
        /// <summary>
        /// The name of the method's type.
        /// </summary>
        public readonly string ClassName;
        /// <summary>
        /// The name of the method
        /// </summary>
        public readonly string MethodName;
        /// <summary>
        /// The file name if debug symbols were compiled with the assembly
        /// </summary>
        public readonly string FileName;
        /// <summary>
        /// The line number of the data.
        /// </summary>
        public readonly int LineNumber;

        public StackFrameDetails(StackFrame frame)
        {
            var method = frame.GetMethod();
            MethodName = method.Name;
            LineNumber = frame.GetFileLineNumber();
            FileName = frame.GetFileName();
            ClassName = method.DeclaringType.FullName;
            var module = method.Module;
            ModuleName = module.Name;
            var assembly = module.Assembly;
            AssemblyName = assembly.FullName;
        }

        public StackFrameDetails(Stream stream)
        {
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    AssemblyName = stream.ReadString();
                    ModuleName = stream.ReadString();
                    ClassName = stream.ReadString();
                    MethodName = stream.ReadString();
                    FileName = stream.ReadString();
                    LineNumber = stream.ReadInt32();
                    break;
                default:
                    throw new VersionNotFoundException("Unknown StackTraceDetails Version");
            }
        }

        public void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(AssemblyName);
            stream.Write(ModuleName);
            stream.Write(ClassName);
            stream.Write(MethodName);
            stream.Write(FileName);
            stream.Write(LineNumber);
        }
    }
}
