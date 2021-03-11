//******************************************************************************************************
//  SrpUserCredentials.cs - Gbtc
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

using System.Collections.Generic;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    /// <remarks>
    /// It is safe to store the user's credential on the server. This is a zero knowledge 
    /// password proof, meaning if this database is compromised, a brute force attack
    /// is the only way to reveal the password.
    /// </remarks>
    public class SrpUserCredentials
    {
        private readonly Dictionary<string, SrpUserCredential> m_users = new Dictionary<string, SrpUserCredential>();

        /// <summary>
        /// Looks up the username from the database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public SrpUserCredential Lookup(string username)
        {
            lock (m_users)
            {
                return m_users[username];
            }
        }

        /// <summary>
        /// Adds the specified user to the credentials database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="strength"></param>
        /// <param name="saltSize"></param>
        /// <param name="iterations"></param>
        public void AddUser(string username, string password, SrpStrength strength = SrpStrength.Bits1024, int saltSize = 32, int iterations = 4000)
        {
            SrpUserCredential user = new SrpUserCredential(username, password, strength, saltSize, iterations);
            lock (m_users)
            {
                m_users.Add(username, user);
            }
        }

        /// <summary>
        /// Adds the specified user to the credential database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="verifier"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="iterations"></param>
        /// <param name="strength"></param>
        public void AddUser(string username, byte[] verifier, byte[] passwordSalt, int iterations, SrpStrength strength)
        {
            SrpUserCredential user = new SrpUserCredential(username, passwordSalt, verifier, iterations, strength);
            lock (m_users)
            {
                m_users.Add(username, user);
            }
        }

    }
}


