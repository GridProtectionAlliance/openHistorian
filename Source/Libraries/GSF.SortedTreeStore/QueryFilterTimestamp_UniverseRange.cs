//******************************************************************************************************
//  QueryFilterTimestamp_UniverseRange.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO;

namespace openHistorian
{
    public abstract partial class QueryFilterTimestamp
    {
        /// <summary>
        /// Creates a universe range filter.
        /// </summary>
        private class UniverseRange 
            : QueryFilterTimestamp
        {
            private bool m_isEndReached;


            /// <summary>
            /// Resets the iterative nature of the filter. 
            /// </summary>
            /// <remarks>
            /// Since a time filter is a set of date ranges, this will reset the frame so a
            /// call to <see cref="GetNextWindow"/> will return the first window of the sequence.
            /// </remarks>
            public override void Reset()
            {
                m_isEndReached = false;
            }

            /// <summary>
            /// Gets the next search window.
            /// </summary>
            /// <param name="startOfWindow">the start of the window to search</param>
            /// <param name="endOfWindow">the end of the window to search</param>
            /// <returns>true if window exists, false if finished.</returns>
            public override bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow)
            {
                if (m_isEndReached)
                {
                    startOfWindow = 0;
                    endOfWindow = 0;
                    return false;
                }
                startOfWindow = ulong.MinValue;
                endOfWindow = ulong.MaxValue;
                m_isEndReached = true;
                return true;
            }

            /// <summary>
            /// Serializes the filter to a stream
            /// </summary>
            /// <param name="stream">the stream to write to</param>
            protected override void WriteToStream(BinaryStreamBase stream)
            {
                throw new NotSupportedException("Cannot be serialized.");
            }


            /// <summary>
            /// Gets the first time that might be accessed by this filter.
            /// </summary>
            public override ulong FirstTime
            {
                get
                {
                    return ulong.MinValue;
                }
            }

            /// <summary>
            /// Gets the last time that might be accessed by this filter.
            /// </summary>
            public override ulong LastTime
            {
                get
                {
                    return ulong.MaxValue;
                }
            }
        }
    }
}