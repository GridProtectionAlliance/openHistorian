//******************************************************************************************************
//  PBKDF2.cs - Gbtc
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
//  8/1/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;

namespace GSF.Security
{
    [TestFixture]
    public class PBKDF2_Test
    {
        readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        [Test]
        public void TestPBE()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] data = new byte[30];
            rng.GetBytes(data);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes("test", data, 400000);
            _ = rfc.GetBytes(20);
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
        [Test]
        public void TestPBE2()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] data = new byte[30];
            rng.GetBytes(data);
            _ = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA512, Encoding.UTF8.GetBytes("test"), data, 400000, 20);
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        [Test]
        public void TestPBE3()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] data = new byte[30];
            rng.GetBytes(data);
            Pkcs5S2ParametersGenerator pk = new Pkcs5S2ParametersGenerator(new Sha512Digest());
            pk.Init(Encoding.UTF8.GetBytes("test"), data, 400000);
            pk.GenerateDerivedMacParameters(20 * 8);
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        [Test]
        public void TestHMAC()
        {
            for (int x = 0; x < 100; x++)
            {
                TestHMACs2(x, x);
                TestHMACs2(x, 2 * x);
                TestHMACs2(2 * x, x);
            }
        }

        public void TestHMACs2(int key, int value)
        {
            byte[] keyB = new byte[key];
            byte[] valueB = new byte[value];

            rng.GetBytes(keyB);
            rng.GetBytes(valueB);

            byte[] code = HMAC<Sha1Digest>.Compute(keyB, valueB);

            HMACSHA1 hmac1 = new HMACSHA1(keyB);
            byte[] code2 = hmac1.ComputeHash(valueB);
            if (!code.SequenceEqual(code2))
                throw new Exception();


            hmac1.Dispose();
        }


        [Test]
        public void TestSHA1()
        {
            //Using test vectors from http://tools.ietf.org/html/rfc6070

            TestSHA1("password", "salt", 1, 20, "0c 60 c8 0f 96 1f 0e 71 f3 a9 b5 24 af 60 12 06 2f e0 37 a6");
            TestSHA1("password", "salt", 2, 20, "ea 6c 01 4d c7 2d 6f 8c cd 1e d9 2a ce 1d 41 f0 d8 de 89 57");
            TestSHA1("password", "salt", 4096, 20, "4b 00 79 01 b7 65 48 9a be ad 49 d9 26 f7 21 d0 65 a4 29 c1");
            //Takes about 1 minute to compute this one. So I commented it out. But it passed.
            //TestSHA1("password", "salt", 16777216, 20, "ee fe 3d 61 cd 4d a4 e4 e9 94 5b 3d 6b a2 15 8c 26 34 e9 84"); 
            TestSHA1("passwordPASSWORDpassword", "saltSALTsaltSALTsaltSALTsaltSALTsalt", 4096, 25, "3d 2e ec 4f e4 1c 84 9b 80 c8 d8 36 62 c0 e4 4a 8b 29 1a 96 4c f2 f0 70 38");
            TestSHA1("pass\0word", "sa\0lt", 4096, 16, "56 fa 6a a7 55 48 09 9d cc 37 d7 f0 34 25 e0 c3");

        }

        void TestSHA1(string P, string S, int c, int dkLen, string dk)
        {
            byte[] dkExpected = StringToByteArray(dk);

            using (PBKDF2 pdf = new PBKDF2(HMACMethod.SHA1, Encoding.ASCII.GetBytes(P), Encoding.ASCII.GetBytes(S), c))
            {
                byte[] dkBytes = pdf.GetBytes(dkLen);
                if (!dkBytes.SequenceEqual(dkExpected))
                {
                    throw new Exception();
                }

            }
        }

        [Test]
        public void TestSHA256()
        {
            //Using test vectors from http://stackoverflow.com/questions/5130513/pbkdf2-hmac-sha2-test-vectors

            TestSHA256("password", "salt", 1, 32, "12 0f b6 cf fc f8 b3 2c 43 e7 22 52 56 c4 f8 37 a8 65 48 c9 2c cc 35 48 08 05 98 7c b7 0b e1 7b");
            TestSHA256("password", "salt", 2, 32, "ae 4d 0c 95 af 6b 46 d3 2d 0a df f9 28 f0 6d d0 2a 30 3f 8e f3 c2 51 df d6 e2 d8 5a 95 47 4c 43");
            TestSHA256("password", "salt", 4096, 32, "c5 e4 78 d5 92 88 c8 41 aa 53 0d b6 84 5c 4c 8d 96 28 93 a0 01 ce 4e 11 a4 96 38 73 aa 98 13 4a");
            //I skiped the one that takes 10 minutes
            TestSHA256("passwordPASSWORDpassword", "saltSALTsaltSALTsaltSALTsaltSALTsalt", 4096, 40, "34 8c 89 db cb d3 2b 2f 32 d8 14 b8 11 6e 84 cf 2b 17 34 7e bc 18 00 18 1c 4e 2a 1f b8 dd 53 e1 c6 35 51 8c 7d ac 47 e9");
            TestSHA256("pass\0word", "sa\0lt", 4096, 16, "89 b6 9d 05 16 f8 29 89 3c 69 62 26 65 0a 86 87");
        }

        void TestSHA256(string P, string S, int c, int dkLen, string dk)
        {
            byte[] dkExpected = StringToByteArray(dk);

            using (PBKDF2 pdf = new PBKDF2(HMACMethod.SHA256, Encoding.ASCII.GetBytes(P), Encoding.ASCII.GetBytes(S), c))
            {
                byte[] dkBytes = pdf.GetBytes(dkLen);
                if (!dkBytes.SequenceEqual(dkExpected))
                {
                    throw new Exception();
                }

            }
        }


        [Test]
        public void TestSHA512()
        {
            //Using test vectors from http://stackoverflow.com/questions/5130513/pbkdf2-hmac-sha2-test-vectors

            TestSHA512("password", "salt", 1, 64, "86 7f 70 cf 1a de 02 cf f3 75 25 99 a3 a5 3d c4 af 34 c7 a6 69 81 5a e5 d5 13 55 4e 1c 8c f2 52 c0 2d 47 0a 28 5a 05 01 ba d9 99 bf e9 43 c0 8f 05 02 35 d7 d6 8b 1d a5 5e 63 f7 3b 60 a5 7f ce");
            TestSHA512("password", "salt", 2, 64, "e1 d9 c1 6a a6 81 70 8a 45 f5 c7 c4 e2 15 ce b6 6e 01 1a 2e 9f 00 40 71 3f 18 ae fd b8 66 d5 3c f7 6c ab 28 68 a3 9b 9f 78 40 ed ce 4f ef 5a 82 be 67 33 5c 77 a6 06 8e 04 11 27 54 f2 7c cf 4e");
            TestSHA512("password", "salt", 4096, 64, "d1 97 b1 b3 3d b0 14 3e 01 8b 12 f3 d1 d1 47 9e 6c de bd cc 97 c5 c0 f8 7f 69 02 e0 72 f4 57 b5 14 3f 30 60 26 41 b3 d5 5c d3 35 98 8c b3 6b 84 37 60 60 ec d5 32 e0 39 b7 42 a2 39 43 4a f2 d5 ");
            TestSHA512("passwordPASSWORDpassword", "saltSALTsaltSALTsaltSALTsaltSALTsalt", 4096, 64, "8c 05 11 f4 c6 e5 97 c6 ac 63 15 d8 f0 36 2e 22 5f 3c 50 14 95 ba 23 b8 68 c0 05 17 4d c4 ee 71 11 5b 59 f9 e6 0c d9 53 2f a3 3e 0f 75 ae fe 30 22 5c 58 3a 18 6c d8 2b d4 da ea 97 24 a3 d3 b8 ");
        }

        void TestSHA512(string P, string S, int c, int dkLen, string dk)
        {
            byte[] dkExpected = StringToByteArray(dk);

            using (PBKDF2 pdf = new PBKDF2(HMACMethod.SHA512, Encoding.ASCII.GetBytes(P), Encoding.ASCII.GetBytes(S), c))
            {
                byte[] dkBytes = pdf.GetBytes(dkLen);
                if (!dkBytes.SequenceEqual(dkExpected))
                {
                    throw new Exception();
                }

            }
        }

        static byte[] StringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }






    }
}
