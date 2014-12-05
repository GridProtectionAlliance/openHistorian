//******************************************************************************************************
//  StackTraceDetails.cs - Gbtc
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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using GSF.Immutable;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Provides stack trace data that can be serialized to a stream.
    /// </summary>
    public class StackTraceDetails
        : ImmutableObjectAutoBase<StackTraceDetails>
    {
        /// <summary>
        /// Gets the stack frame data
        /// </summary>
        public readonly ImmutableList<StackFrameDetails> Frames;

        /// <summary>
        /// Creates a new stack trace.
        /// </summary>
        public StackTraceDetails()
        {
            var trace = new StackTrace(true);
            Frames = new ImmutableList<StackFrameDetails>();
            var frames = trace.GetFrames();
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    Frames.Add(new StackFrameDetails(frame));
                }
            }
            IsReadOnly = true;
        }

        /// <summary>
        /// Loads stack trace information from the supplied <see cref="stream"/>
        /// </summary>
        /// <param name="stream">where to load the stack trace information</param>
        public StackTraceDetails(Stream stream)
        {
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    int count = stream.ReadInt32();
                    Frames = new ImmutableList<StackFrameDetails>(count);
                    while (count > 0)
                    {
                        count--;
                        Frames.Add(new StackFrameDetails(stream));
                    }
                    break;
                default:
                    throw new VersionNotFoundException("Unknown StackTraceDetails Version");
            }
            IsReadOnly = true;
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

    }
}
