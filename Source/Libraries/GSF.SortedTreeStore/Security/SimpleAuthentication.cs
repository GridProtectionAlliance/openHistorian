//******************************************************************************************************
//  SimpleAuthentication.cs - Gbtc
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
//  7/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GSF.Net;
using Org.BouncyCastle.Crypto.Digests;

namespace GSF.Security
{
    /// <summary>
    /// Provides simple password based authentication that uses SCRAM (RFC 5802).
    /// </summary>
    public class SimpleAuthentication
    {
        private Dictionary<string, UserCredentials> m_users = new Dictionary<string, UserCredentials>();

        public class UserCredentials
        {
            public string UserName;
            public int Iterations;
            public byte[] Salt;
            public byte[] StoredKey;
            public byte[] ServerKey;
        }

        static UTF8Encoding UTF8 = new UTF8Encoding(true);
        private static byte[] StringClientKey = UTF8.GetBytes("Client Key");
        private static byte[] StringServerKey = UTF8.GetBytes("Server Key");

        private const int HashSize = 64;

        /// <summary>
        /// The number of bytes in the salt. Recommends 16 or more.
        /// </summary>
        public int DefaultSaltSize { get; set; }
        /// <summary>
        /// The number of iterations to make. Recommends 1000 or more.
        /// </summary>
        public int DefaultIterations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SimpleAuthentication()
        {
            DefaultSaltSize = 32;
            DefaultIterations = 4000;
        }

        public void AddUser(string userName, string password)
        {
            userName = userName.Normalize(NormalizationForm.FormKC);
            password = password.Normalize(NormalizationForm.FormKC);

            var user = new UserCredentials();
            user.UserName = userName;
            user.Iterations = DefaultIterations;
            user.Salt = GetNonce(DefaultSaltSize);

            byte[] saltedPassword = GenerateSaltedPassword(password, user.Salt, user.Iterations);
            byte[] clientKey = ComputeClientKey(saltedPassword);
            user.StoredKey = ComputeStoredKey(clientKey);
            user.ServerKey = ComputeServerKey(saltedPassword);

            m_users.Add(userName, user);
        }

        public string AuthenticateAsServer(NetworkBinaryStream stream)
        {
            byte[] usernameBytes = stream.ReadBytes();
            byte[] clientNonce = stream.ReadBytes();
            string userName = UTF8.GetString(usernameBytes);
            UserCredentials user = m_users[userName];

            byte[] serverNonce = GetNonce();
            stream.WriteWithLength(serverNonce);
            stream.WriteWithLength(user.Salt);
            stream.Write(user.Iterations);
            stream.Flush();

            byte[] authMessage = ComputeAuthMessage(serverNonce, clientNonce, user.Salt, usernameBytes, user.Iterations);
            byte[] clientSignature = ComputeHMAC(user.StoredKey, authMessage);
            byte[] clientProof = stream.ReadBytes();

            byte[] clientKeyVerify = XOR(clientProof, clientSignature);
            byte[] storedKeyVerify = ComputeStoredKey(clientKeyVerify);

            if (storedKeyVerify.SequenceEqual(user.StoredKey))
            {
                //Client holds the password
                //Send ServerSignature

                byte[] serverSignature = ComputeHMAC(user.ServerKey, authMessage);
                stream.WriteWithLength(serverSignature);
                stream.Flush();
                return user.UserName;
            }
            return string.Empty;
        }

        public bool AuthenticateAsClient(NetworkBinaryStream stream, string username, string password)
        {
            byte[] clientNonce = GetNonce();
            byte[] usernameBytes = UTF8.GetBytes(username.Normalize(NormalizationForm.FormKC));

            stream.WriteWithLength(usernameBytes);
            stream.WriteWithLength(clientNonce);
            stream.Flush();

            byte[] serverNonce = stream.ReadBytes();
            byte[] salt = stream.ReadBytes();
            int iterations = stream.ReadInt32();

            byte[] saltedPassword = GenerateSaltedPassword(password, salt, iterations);
            byte[] clientKey = ComputeClientKey(saltedPassword);
            byte[] storedKey = ComputeStoredKey(clientKey);
            byte[] authMessage = ComputeAuthMessage(serverNonce, clientNonce, salt, usernameBytes, iterations);
            byte[] clientSignature = ComputeHMAC(storedKey, authMessage);
            byte[] clientProof = XOR(clientKey, clientSignature);
            byte[] serverKey = ComputeServerKey(saltedPassword);
            byte[] serverSignature = ComputeHMAC(serverKey, authMessage);

            stream.WriteWithLength(clientProof);
            stream.Flush();

            byte[] serverSignatureVerify = stream.ReadBytes();
            return (serverSignature.SequenceEqual(serverSignatureVerify));
        }


        private byte[] XOR(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                throw new Exception();
            byte[] rv = new byte[a.Length];
            for (int x = 0; x < a.Length; x++)
            {
                rv[x] = (byte)(a[x] ^ b[x]);
            }
            return rv;
        }

        

        private byte[] ComputeAuthMessage(byte[] serverNonce, byte[] clientNonce, byte[] salt, byte[] username, int iterations)
        {
            byte[] data = new byte[serverNonce.Length + clientNonce.Length + salt.Length + username.Length + 4];
            serverNonce.CopyTo(data, 0);
            clientNonce.CopyTo(data, serverNonce.Length);
            salt.CopyTo(data, serverNonce.Length + clientNonce.Length);
            username.CopyTo(data, serverNonce.Length + clientNonce.Length + salt.Length);
            data[data.Length - 4] = (byte)(iterations);
            data[data.Length - 3] = (byte)(iterations >> 8);
            data[data.Length - 2] = (byte)(iterations >> 16);
            data[data.Length - 1] = (byte)(iterations >> 24);
            return data;
        }

        private byte[] ComputeClientKey(byte[] saltedPassword)
        {
            return ComputeHMAC(saltedPassword, StringClientKey);
        }

        private byte[] ComputeServerKey(byte[] saltedPassword)
        {
            return ComputeHMAC(saltedPassword, StringServerKey);
        }


       
        //private byte[] GenerateSaltedPassword(string password, byte[] salt, int iterations)
        //{
        //    byte[] passwordBytes = UTF8.GetBytes(password.Normalize(NormalizationForm.FormKC));
        //    using (var pass = new Rfc2898DeriveBytes(passwordBytes, salt, iterations))
        //    {
        //        return pass.GetBytes(HashSize);
        //    }
        //}

        private byte[] ComputeStoredKey(byte[] clientKey)
        {
            return Hash<Sha512Digest>.Compute(clientKey);
        }

        private byte[] ComputeHMAC(byte[] key, byte[] message)
        {
            return HMAC<Sha512Digest>.Compute(key, message);
        }

        private byte[] GenerateSaltedPassword(string password, byte[] salt, int iterations)
        {
            byte[] passwordBytes = UTF8.GetBytes(password.Normalize(NormalizationForm.FormKC));
            using (var pass = new PBKDF2(HMACMethod.SHA512, passwordBytes, salt, iterations))
            {
                return pass.GetBytes(HashSize);
            }
        }

        private byte[] GetNonce(int size = HashSize)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] nonce = new byte[size];
                rng.GetBytes(nonce);
                return nonce;
            }
        }
    }
}


