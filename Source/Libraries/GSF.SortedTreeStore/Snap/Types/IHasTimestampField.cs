//******************************************************************************************************
//  IHasTimestampField.cs - Gbtc
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
//  03/26/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace GSF.Snap.Types
{
    /// <summary>
    /// A point that has a timestamp field that may be extracted.
    /// </summary>
    public interface IHasTimestampField
    {
        /// <summary>
        /// Attempts to get the timestamp field of a point. This function might fail if the datetime field
        /// is not able to be converted.
        /// </summary>
        /// <param name="timestamp">an output field of the timestamp</param>
        /// <returns>True if a timestamp could be parsed. False otherwise.</returns>
        bool TryGetDateTime(out DateTime timestamp);
    }
}
