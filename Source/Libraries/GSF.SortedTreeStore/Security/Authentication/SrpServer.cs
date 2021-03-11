//******************************************************************************************************
//  SrpServer.cs - Gbtc
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

using System.IO;
using System.Text;
using GSF.IO;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class SrpServer
    {
        /// <summary>
        /// Contains the user credentials database
        /// </summary>
        public readonly SrpUserCredentials Users;

        private static readonly UTF8Encoding UTF8 = new UTF8Encoding(true);

        /// <summary>
        /// 
        /// </summary>
        public SrpServer()
        {
            Users = new SrpUserCredentials();
        }

        /// <summary>
        /// Requests that the provided stream be authenticated 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="additionalChallenge">Additional data to include in the challenge. If using SSL certificates, 
        /// adding the thumbprint to the challenge will allow detecting man in the middle attacks.</param>
        /// <returns></returns>
        public SrpServerSession AuthenticateAsServer(Stream stream, byte[] additionalChallenge = null)
        {
            if (additionalChallenge is null)
                additionalChallenge = new byte[] { };

            // Header
            //  C => S
            //  int16   usernameLength (max 1024 characters)
            //  byte[]  usernameBytes

            int len = stream.ReadInt16();
            if (len < 0 || len > 1024)
                return null;

            byte[] usernameBytes = stream.ReadBytes(len);
            string username = UTF8.GetString(usernameBytes);
            SrpUserCredential user = Users.Lookup(username);
            SrpServerSession session = new SrpServerSession(user);
            if (session.TryAuthenticate(stream, additionalChallenge))
            {
                return session;
            }
            return null;
        }

    }
}


