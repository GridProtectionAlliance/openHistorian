//******************************************************************************************************
//  SrpServer.cs - Gbtc
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

using System.IO;
using System.Linq;
using System.Text;
using GSF.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace GSF.Security
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

        static UTF8Encoding UTF8 = new UTF8Encoding(true);

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
        /// <returns></returns>
        public SrpServerSession AuthenticateAsServer(Stream stream)
        {
            Sha512Digest hash = new Sha512Digest();

            //Read from Client: Username
            byte[] usernameBytes = stream.ReadBytes();
            string userName = UTF8.GetString(usernameBytes);
            var user = Users.Lookup(userName);

            //Send to Client: Salt, Iterations, Strength, Server's Public Value
            stream.WriteWithLength(user.Salt);
            stream.Write(user.Iterations);
            stream.Write((int)user.SrpStrength);
            stream.Flush(); //since computing B takes a long time. Go ahead and flush

            var param = SrpConstants.Lookup(user.SrpStrength);
            Srp6Server server = new Srp6Server(param, user.VerificationInteger);
            BigInteger pubB = server.GenerateServerCredentials();
            byte[] pubBBytes = pubB.ToByteArrayUnsigned();
            stream.WriteWithLength(pubBBytes);
            stream.Flush();

            //Read from client: A
            byte[] pubABytes = stream.ReadBytes();
            BigInteger pubA = new BigInteger(1, pubABytes);

            //Calculate Session Key
            BigInteger S = server.CalculateSecret(hash, pubA);
            byte[] K = ComputeHash(hash, S.ToByteArrayUnsigned());

            //Prove to each other the session key.
            byte[] clientProofCheck = GenerateClientProof(hash, param.kb2, usernameBytes, user.Salt, pubABytes, pubBBytes, K);
            byte[] clientProof = stream.ReadBytes();

            if (clientProof.SequenceEqual(clientProofCheck))
            {
                byte[] serverProof = GenerateServerProof(hash, pubABytes, clientProof, K);
                stream.WriteWithLength(serverProof);
                stream.Flush();

                var session = new SrpServerSession();
                session.Username = userName;
                session.SessionSecret = S;
                session.SessionKey = K;
                return session;
            }

            return null;
        }

        private byte[] GenerateClientProof(IDigest hash, byte[] k2, byte[] i, byte[] s, byte[] A, byte[] B, byte[] K)
        {
            //M = H(H(N) xor H(g), H(I), s, A, B, K)
            return ComputeHash(hash, k2, ComputeHash(hash, i), s, A, B, K);
        }

        private byte[] GenerateServerProof(IDigest hash, byte[] A, byte[] M, byte[] K)
        {
            //H(A, M, K)
            return ComputeHash(hash, A, M, K);
        }

        private static byte[] ComputeHash(IDigest hash, params byte[][] words)
        {
            foreach (var w in words)
            {
                hash.BlockUpdate(w, 0, w.Length);
            }
            byte[] rv = new byte[hash.GetDigestSize()];
            hash.DoFinal(rv, 0);
            return rv;
        }

    }
}


