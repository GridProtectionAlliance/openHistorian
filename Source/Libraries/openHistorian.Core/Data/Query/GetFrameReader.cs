//******************************************************************************************************
//  GetFrameMethods.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  8/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{

    public class FrameReader
        : IDisposable
    {
        readonly PointStream m_stream;

        public FrameReader(PointStream stream)
        {
            m_stream = stream;
            Frame = new SortedList<ulong, HistorianValueStruct>();
            m_stream.Read();
        }

        public DateTime FrameTime;

        public SortedList<ulong, HistorianValueStruct> Frame;

        public bool Read()
        {
            if (!m_stream.IsValid)
                return false;

            Frame.Clear();
            Frame.Add(m_stream.CurrentKey.PointID, m_stream.CurrentValue.ToStruct());
            FrameTime = m_stream.CurrentKey.TimestampAsDate;

            while (true)
            {
                if (!m_stream.Read())
                {
                    Dispose();
                    return true; //End of stream
                }

                if (m_stream.CurrentKey.TimestampAsDate == FrameTime)
                {
                    //try
                    //{
                        Frame.Add(m_stream.CurrentKey.PointID, m_stream.CurrentValue.ToStruct());
                    //}
                    //catch (Exception ex)
                    //{
                    //    ex = ex;
                    //}
                }
                else
                {
                    return true;
                }
            }
        }

        public void Dispose()
        {
            m_stream.Dispose();
        }
    }

    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static partial class GetFrameReaderMethods
    {

        /// <summary>
        /// Gets concentrated frames from the provided stream
        /// </summary>
        /// <param name="stream">the database to use</param>
        /// <returns></returns>
        public static FrameReader GetFrameReader(this PointStream stream)
        {
            return new FrameReader(stream);
        }

    }
}