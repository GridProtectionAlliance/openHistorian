//******************************************************************************************************
//  IStream256.cs - Gbtc
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
//  6/23/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF
{
    /// <summary>
    /// A 256 bit stream that can be parsed only once.
    /// </summary>
    public interface IStream256
    {
        /// <summary>
        /// Advances to the next entry
        /// </summary>
        /// <param name="key1">an output parameter to store the first key</param>
        /// <param name="key2">an output parameter to store the second key</param>
        /// <param name="value1">an output parameter to store the first value</param>
        /// <param name="value2">an output parameter to store the second value</param>
        /// <returns>
        /// Returns true if the next value is valid. Returns false if the end of the stream has been encountered.
        /// </returns>
        bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2);
    }
}
