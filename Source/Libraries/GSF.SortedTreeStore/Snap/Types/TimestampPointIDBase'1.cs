//******************************************************************************************************
//  TimestampPointIDBase'1.cs - Gbtc
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
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF.Snap.Types
{
    /// <summary>
    /// Base implementation of a historian key. 
    /// These are the required functions that are 
    /// necessary for the historian engine to operate
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class TimestampPointIDBase<TKey>
        : TimestampBase<TKey>
        where TKey : SnapTypeBase<TKey>, new()
    {
        /// <summary>
        /// The id number of the point.
        /// </summary>
        public ulong PointID;

        
    }
}