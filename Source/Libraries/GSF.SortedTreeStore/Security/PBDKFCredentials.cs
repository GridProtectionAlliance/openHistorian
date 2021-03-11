//******************************************************************************************************
//  PBDKFCredentials.cs - Gbtc
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
//  8/26/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Text;

namespace GSF.Security
{
    /// <summary>
    /// Computes the password credentials. 
    /// Optimized so duplicate calls will not recompute the password unless necessary.
    /// </summary>
    internal class PBDKFCredentials
    {
        //Origional Username and Passwords
        /// <summary>
        /// The UTF8 encoded normalized username.
        /// </summary>
        public byte[] UsernameBytes;
        private readonly byte[] m_passwordBytes;

        //Salted Password, base on PBKDF2
        private HashMethod m_hashMethod;
        private int m_iterations;
        private byte[] m_salt;
        
        /// <summary>
        /// The password value 
        /// </summary>
        public byte[] SaltedPassword;

        public PBDKFCredentials(string username, string password)
        {
            UsernameBytes = Encoding.UTF8.GetBytes(username.Normalize(NormalizationForm.FormKC));
            m_passwordBytes = Encoding.UTF8.GetBytes(password.Normalize(NormalizationForm.FormKC));
        }

        /// <summary>
        /// Updates the <see cref="SaltedPassword"/>. Returns False if the password value did not change.
        /// </summary>
        /// <param name="hashMethod"></param>
        /// <param name="salt"></param>
        /// <param name="iterations"></param>
        /// <returns>Returns False if the password value did not change.</returns>
        public bool TryUpdate(HashMethod hashMethod, byte[] salt, int iterations)
        {
            bool hasChanged = false;

            if (m_salt is null || !salt.SecureEquals(m_salt))
            {
                hasChanged = true;
                m_salt = salt;
            }

            if (m_hashMethod != hashMethod)
            {
                hasChanged = true;
                m_hashMethod = hashMethod;
            }

            if (iterations != m_iterations)
            {
                hasChanged = true;
                m_iterations = iterations;
            }

            if (hasChanged)
            {
                switch (hashMethod)
                {
                    case HashMethod.Sha1:
                        SaltedPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA1, m_passwordBytes, m_salt, m_iterations, 20);
                        break;
                    case HashMethod.Sha256:
                        SaltedPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA256, m_passwordBytes, m_salt, m_iterations, 32);
                        break;
                    case HashMethod.Sha384:
                        SaltedPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA384, m_passwordBytes, m_salt, m_iterations, 48);
                        break;
                    case HashMethod.Sha512:
                        SaltedPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA512, m_passwordBytes, m_salt, m_iterations, 64);
                        break;
                    default:
                        throw new Exception("Invalid Hash Method");
                }
                return true;
            }
            return false;
        }
    }
}
