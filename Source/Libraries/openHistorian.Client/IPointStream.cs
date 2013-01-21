//******************************************************************************************************
//  IPointStream.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************


using GSF;

namespace openHistorian
{
    /// <summary>
    /// Provides a point stream that has no data in it. Obtain an instance of this class
    /// by calling the static member <see cref="Instance"/>.
    /// </summary>
    public class NullPointStream : IPointStream
    {
        public readonly static IPointStream Instance = new NullPointStream();
        
        private NullPointStream()
        {
        }

        public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
        {
            key1 = 0;
            key2 = 0;
            value1 = 0;
            value2 = 0;
            return false;
        }

        public void Cancel()
        {
        }
    }

    /// <summary>
    /// Represents a fundamental way to stream points.
    /// </summary>
    public interface IPointStream : IStream256
    {
        /// <summary>
        /// Prematurely stops the execution of the stream.
        /// Once canceled, no more points will be returned.
        /// </summary>
        void Cancel();
    }
}
