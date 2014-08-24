//******************************************************************************************************
//  ScramServer.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  8/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.IO;
using System.Linq;
using GSF.IO;

namespace GSF.Security
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class ScramServer
    {
        /// <summary>
        /// Contains the user credentials database
        /// </summary>
        public readonly ScramUserCredentials Users;
        private NonceSequencer m_nonce = new NonceSequencer(16);

        /// <summary>
        /// 
        /// </summary>
        public ScramServer()
        {
            Users = new ScramUserCredentials();
        }

        /// <summary>
        /// Requests that the provided stream be authenticated 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ScramServerSession AuthenticateAsServer(Stream stream)
        {
            byte[] usernameBytes = stream.ReadBytes();
            byte[] clientNonce = stream.ReadBytes();
            string userName = Scram.Utf8.GetString(usernameBytes);

            var user = Users.Lookup(userName);

            byte[] serverNonce = m_nonce.Next();
            stream.WriteWithLength(serverNonce);
            stream.WriteWithLength(user.Salt);
            stream.Write(user.Iterations);
            stream.Flush();

            byte[] authMessage = Scram.ComputeAuthMessage(serverNonce, clientNonce, user.Salt, usernameBytes, user.Iterations);
            byte[] clientSignature = user.ComputeClientSignature(authMessage);
            byte[] serverSignature = user.ComputeServerSignature(authMessage);
            byte[] clientProof = stream.ReadBytes();

            byte[] clientKeyVerify = Scram.XOR(clientProof, clientSignature);
            byte[] storedKeyVerify = Scram.ComputeStoredKey(clientKeyVerify);

            if (storedKeyVerify.SequenceEqual(user.StoredKey))
            {
                //Client holds the password
                //Send ServerSignature
                stream.WriteWithLength(serverSignature);
                stream.Flush();
                return new ScramServerSession(user.UserName);
            }
            return null;
        }

       

    }
}


