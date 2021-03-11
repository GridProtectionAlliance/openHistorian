//******************************************************************************************************
//  SrpStrength.cs - Gbtc
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
//  7/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Specifies the bit strength of the SRP algorithm.
    /// </summary>
    public enum SrpStrength
        : int
    {
        /// <summary>
        /// Bit strength takes 1x (approximately 20ms on a 3.4Ghz PC) to secure the channel.
        /// </summary>
        Bits1024 = 1024,
        /// <summary>
        /// Bit strength takes 3x to secure channel.
        /// </summary>
        Bits1536 = 1536,
        /// <summary>
        /// Bit strength takes 6.5x to secure channel.
        /// </summary>
        Bits2048 = 2048,
        /// <summary>
        /// Bit strength takes 21x to secure channel.
        /// </summary>
        Bits3072 = 3072,
        /// <summary>
        /// Bit strength takes 47x to secure channel.
        /// </summary>
        Bits4096 = 4096,
        /// <summary>
        /// Bit strength takes 154x to secure channel.
        /// </summary>
        Bits6144 = 6144,
        /// <summary>
        /// Bit strength takes 366x to secure channel.
        /// </summary>
        Bits8192 = 8192
    }
}