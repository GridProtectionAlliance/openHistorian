//******************************************************************************************************
//  SrpClient.cs - Gbtc
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
    public class SrpClient
    {
        static UTF8Encoding UTF8 = new UTF8Encoding(true);

        public bool AuthenticateAsClient(Stream stream, string username, string password)
        {
            Sha512Digest hash = new Sha512Digest();

            username = username.Normalize(NormalizationForm.FormKC);
            password = password.Normalize(NormalizationForm.FormKC);
            byte[] usernameBytes = UTF8.GetBytes(username);
            stream.WriteWithLength(usernameBytes);
            stream.Flush();

            byte[] salt = stream.ReadBytes();
            int iterations = stream.ReadInt32();
            SrpStrength strength = (SrpStrength)stream.ReadInt32();

            byte[] hashPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA512, UTF8.GetBytes(password), salt, iterations, 64);

            var param = SrpConstants.Lookup(strength);
            Srp6Client client = new Srp6Client(param);
            BigInteger pubA = client.GenerateClientCredentials(hash, salt, usernameBytes, hashPassword);
            byte[] pubABytes = pubA.ToByteArrayUnsigned();

            stream.WriteWithLength(pubABytes);
            stream.Flush();

            //Read from Server: B
            byte[] pubBBytes = stream.ReadBytes();
            BigInteger pubB = new BigInteger(1, pubBBytes);

            //Calculate Session Key
            BigInteger S = client.CalculateSecret(hash, pubB);
            byte[] K = ComputeHash(hash, S.ToByteArrayUnsigned());

            //Prove to each other the session key.
            byte[] clientProof = GenerateClientProof(hash, param.kb2, usernameBytes, salt, pubABytes, pubBBytes, K);
            stream.WriteWithLength(clientProof);
            stream.Flush();

            byte[] serverProofCheck = GenerateServerProof(hash, pubABytes, clientProof, K);
            byte[] serverProof = stream.ReadBytes();

            return (serverProofCheck.SequenceEqual(serverProof));
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


