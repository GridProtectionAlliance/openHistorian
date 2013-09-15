//******************************************************************************************************
//  QueryFilterPointId_Universe.cs - Gbtc
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
    /// <summary>
    /// A class that is used to filter point results based on the PointID number.
    /// </summary>
    public abstract partial class QueryFilterPointId
    {
        /// <summary>
        /// A filter that returns true for every PointID
        /// </summary>
        private class Universe 
            : QueryFilterPointId
        {
            /// <summary>
            /// A reusable universal filter.
            /// </summary>
            public static Universe Instance
            {
                get;
                private set;
            }

            static Universe()
            {
                Instance = new Universe();
            }

            /// <summary>
            /// Always returns true
            /// </summary>
            /// <param name="pointID"></param>
            /// <returns></returns>
            public override bool ContainsPointID(ulong pointID)
            {
                return true;
            }
            
            /// <summary>
            /// Not Supported
            /// </summary>
            /// <param name="stream"></param>
            protected override void WriteToStream(BinaryStreamBase stream)
            {
                throw new NotSupportedException("Has no data that needs to be serialized to the stream.");
            }

            /// <summary>
            /// Not Supported
            /// </summary>
            protected override int Count
            {
                get
                {
                    throw new NotSupportedException("not valid for UniverseFilter");
                }
            }
        }
       
    }
}