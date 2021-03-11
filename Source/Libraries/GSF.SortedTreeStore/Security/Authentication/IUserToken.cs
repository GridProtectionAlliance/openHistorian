//******************************************************************************************************
//  IUserToken.cs - Gbtc
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
//  08/29/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.IO;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// An interface for token data that is associated with a user. 
    /// </summary>
    public interface IUserToken
    {
        /// <summary>
        /// Saves the token to a stream
        /// </summary>
        /// <param name="stream">the stream to save to</param>
        void Save(Stream stream);
        /// <summary>
        /// Loads the token from a stream
        /// </summary>
        /// <param name="stream">the stream to load from</param>
        void Load(Stream stream);
    }

    /// <summary>
    /// An empty token that does not contain any data.
    /// </summary>
    public struct NullToken
        : IUserToken
    {
        public void Save(Stream stream)
        {
        }
        public void Load(Stream stream)
        {
        }
    }
}
