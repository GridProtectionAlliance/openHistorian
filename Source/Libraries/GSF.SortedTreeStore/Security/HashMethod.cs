//******************************************************************************************************
//  HashMethod.cs - Gbtc
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
//  8/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

namespace GSF.Security
{
    /// <summary>
    /// The hash/hmac method that will be used for authentication protocols.
    /// </summary>
    public enum HashMethod : byte
    {
        /// <summary>
        /// Uses Hash and HMAC Sha1
        /// </summary>
        Sha1 = 0,
        /// <summary>
        /// Uses Hash and HMAC Sha2-256
        /// </summary>
        Sha256 = 1,
        /// <summary>
        /// Uses Hash and HMAC Sha2-384
        /// </summary>
        Sha384 = 2,
        /// <summary>
        /// Uses Hash and HMAC Sha2-512
        /// </summary>
        Sha512 = 3
    }
}
