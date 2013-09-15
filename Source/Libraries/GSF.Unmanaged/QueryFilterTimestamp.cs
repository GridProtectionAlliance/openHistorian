//******************************************************************************************************
//  QueryFilterTimestamp.cs - Gbtc
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
using System.Data;
using GSF.IO;

namespace openHistorian
{
    /// <summary>
    /// A class that is used to filter point results based on the Timestamp
    /// </summary>
    public abstract partial class QueryFilterTimestamp
    {

        /// <summary>
        /// Creates a filter that is a universe filter that will not filter any points.
        /// </summary>
        /// <returns></returns>
        public static QueryFilterTimestamp CreateAllKeysValid()
        {
            return new UniverseRange();
        }
        
        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <returns></returns>
        public static QueryFilterTimestamp CreateFromRange(DateTime firstTime, DateTime lastTime)
        {
            return new FixedRange((ulong)firstTime.Ticks, (ulong)lastTime.Ticks);
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <returns></returns>
        public static QueryFilterTimestamp CreateFromRange(ulong firstTime, ulong lastTime)
        {
            return new FixedRange(firstTime, lastTime);
        }

        /// <summary>
        /// Creates a filter over a set of date ranges (Similiar to downsampled queries)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive if contained in the intervals)</param>
        /// <param name="mainInterval">the smallest interval that is exact</param>
        /// <param name="subInterval">the interval that will be parsed. Possible to be rounded</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="QueryFilterTimestamp"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static QueryFilterTimestamp CreateFromIntervalData(ulong firstTime, ulong lastTime, ulong mainInterval, ulong subInterval, ulong tolerance)
        {
            return new IntervalRanges(firstTime, lastTime, mainInterval, subInterval, tolerance);
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <param name="mainInterval">the smallest interval that is exact</param>
        /// <param name="subInterval">the interval that will be parsed. Possible to be rounded</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="QueryFilterTimestamp"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               MainInterval = 0.1 seconds. SubInterval = 0.0333333 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static QueryFilterTimestamp CreateFromIntervalData(DateTime firstTime, DateTime lastTime, TimeSpan mainInterval, TimeSpan subInterval, TimeSpan tolerance)
        {
            return new IntervalRanges((ulong)firstTime.Ticks, (ulong)lastTime.Ticks, (ulong)mainInterval.Ticks, (ulong)subInterval.Ticks, (ulong)tolerance.Ticks);
        }
        
        /// <summary>
        /// Loads a <see cref="QueryFilterTimestamp"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns></returns>
        public static QueryFilterTimestamp CreateFromStream(BinaryStreamBase stream)
        {
            byte version = stream.ReadByte();
            switch (version)
            {
                case 0:
                    return new UniverseRange();
                case 1:
                    return new FixedRange(stream);
                case 2:
                    return new IntervalRanges(stream);
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
        }

        /// <summary>
        /// Resets the iterative nature of the filter. 
        /// </summary>
        /// <remarks>
        /// Since a time filter is a set of date ranges, this will reset the frame so a
        /// call to <see cref="GetNextWindow"/> will return the first window of the sequence.
        /// </remarks>
        public abstract void Reset();

        /// <summary>
        /// Gets the next search window.
        /// </summary>
        /// <param name="startOfWindow">the start of the window to search</param>
        /// <param name="endOfWindow">the end of the window to search</param>
        /// <returns>true if window exists, false if finished.</returns>
        public abstract bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow);


        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public void Save(BinaryStreamBase stream)
        {
            if (this is UniverseRange)
            {
                stream.Write((byte)0); //No data stored
            }
            else if (this is FixedRange)
            {
                stream.Write((byte)1); //stored as start/stop
                WriteToStream(stream);
            }
            else if (this is IntervalRanges)
            {
                stream.Write((byte)2); //Stored with interval data
                WriteToStream(stream);
            }
            else
            {
                throw new NotSupportedException("The provided inherited class cannot be serialized");
            }
        }

        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        protected abstract void WriteToStream(BinaryStreamBase stream);

        /// <summary>
        /// Gets the first time that might be accessed by this filter.
        /// </summary>
        public abstract ulong FirstTime { get; }
        /// <summary>
        /// Gets the last time that might be accessed by this filter.
        /// </summary>
        public abstract ulong LastTime { get; }
    }
}