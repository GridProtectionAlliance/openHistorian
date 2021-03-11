//******************************************************************************************************
//  GenerateCertificate.cs - Gbtc
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
//  8/29/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace GSF.Security
{
    //http://stackoverflow.com/questions/3770233/is-it-possible-to-programmatically-generate-an-x509-certificate-using-only-c/3771913#3771913
    //http://stackoverflow.com/questions/22230745/generate-self-signed-certificate-on-the-fly
    //https://blog.differentpla.net/post/53/how-do-i-create-a-self-signed-certificate-using-bouncy-castle-
    //https://blog.differentpla.net/post/20/how-do-i-convert-a-bouncy-castle-certificate-to-a-net-certificate-
    //http://www.fkollmann.de/v2/post/Creating-certificates-using-BouncyCastle.aspx

    /// <summary>
    /// Generates <see cref="X509Certificate2"/>s.
    /// </summary>
    public static class GenerateCertificate
    {
        /// <summary>
        /// Opens a certificate, loading the private key of the PFX file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 OpenCertificate(string fileName, string password)
        {
            return new X509Certificate2(fileName, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }

        /// <summary>
        /// Creates new certificate
        /// </summary>
        /// <returns></returns>
        public static void CreateSelfSignedCertificate(string subjectDirName, DateTime startDate, DateTime endDate, int signatureBits, int keyStrength, string password, string fileName)
        {
            string signatureAlgorithm;
            switch (signatureBits)
            {
                case 160:
                    signatureAlgorithm = "SHA1withRSA";
                    break;
                case 224:
                    signatureAlgorithm = "SHA224withRSA";
                    break;
                case 256:
                    signatureAlgorithm = "SHA256withRSA";
                    break;
                case 384:
                    signatureAlgorithm = "SHA384withRSA";
                    break;
                case 512:
                    signatureAlgorithm = "SHA512withRSA";
                    break;
                default:
                    throw new ArgumentException("Invalid signature bit size.", "signatureBits");
            }

            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            // Generate public/private keys.
            AsymmetricCipherKeyPair encryptionKeys;

            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            encryptionKeys = keyPairGenerator.GenerateKeyPair();

            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random));
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);
            certificateGenerator.SetIssuerDN(new X509Name(subjectDirName));
            certificateGenerator.SetSubjectDN(new X509Name(subjectDirName));
            certificateGenerator.SetNotBefore(startDate);
            certificateGenerator.SetNotAfter(endDate);
            certificateGenerator.SetPublicKey(encryptionKeys.Public);

            // self-sign certificate
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(encryptionKeys.Private, random);

            Pkcs12Store store = new Pkcs12Store();
            string friendlyName = certificate.SubjectDN.ToString();
            X509CertificateEntry certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(encryptionKeys.Private), new[] { certificateEntry });

            MemoryStream stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), random);

            //Verify that the certificate is valid.
            _ = new X509Certificate2(stream.ToArray(), password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            //Write the file.
            File.WriteAllBytes(fileName, stream.ToArray());

            File.WriteAllBytes(Path.ChangeExtension(fileName, ".cer"), certificate.GetEncoded());
        }


        /// <summary>
        /// Creates a self signed certificate that can be used in SSL communications
        /// </summary>
        /// <param name="subjectDirName">A valid DirName formated string. Example: CN=ServerName</param>
        /// <param name="signatureBits">Bitstrength of signature algorithm. Supported Lengths are 160,256, and 384 </param>
        /// <param name="keyStrength">RSA key strength. Typically a multiple of 1024.</param>
        /// <returns></returns>
        public static X509Certificate2 CreateSelfSignedCertificate(string subjectDirName, int signatureBits, int keyStrength)
        {
            DateTime startDate = DateTime.UtcNow.AddYears(-1);
            DateTime endDate = DateTime.UtcNow.AddYears(100); ;

            string signatureAlgorithm;
            switch (signatureBits)
            {
                case 160:
                    signatureAlgorithm = "SHA1withRSA";
                    break;
                case 256:
                    signatureAlgorithm = "SHA256withRSA";
                    break;
                case 384:
                    signatureAlgorithm = "SHA384withRSA";
                    break;
                default:
                    throw new ArgumentException("Invalid signature bit size.", "signatureBits");
            }

            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            // Generate public/private keys.
            AsymmetricCipherKeyPair encryptionKeys;

            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            encryptionKeys = keyPairGenerator.GenerateKeyPair();

            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random));
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);
            certificateGenerator.SetIssuerDN(new X509Name(subjectDirName));
            certificateGenerator.SetSubjectDN(new X509Name(subjectDirName));
            certificateGenerator.SetNotBefore(startDate);
            certificateGenerator.SetNotAfter(endDate);
            certificateGenerator.SetPublicKey(encryptionKeys.Public);

            // selfsign certificate
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(encryptionKeys.Private, random);

            Pkcs12Store store = new Pkcs12Store();
            string friendlyName = certificate.SubjectDN.ToString();
            X509CertificateEntry certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(encryptionKeys.Private), new[] { certificateEntry });

            MemoryStream stream = new MemoryStream();
            store.Save(stream, "".ToCharArray(), random);

            //Verify that the certificate is valid.
            X509Certificate2 convertedCertificate = new X509Certificate2(stream.ToArray(), "", X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            return convertedCertificate;
        }

        //private static bool addCertToStore(X509Certificate2 cert, StoreName st, StoreLocation sl)
        //{
        //    bool bRet = false;

        //    try
        //    {
        //        X509Store store = new X509Store(st, sl);
        //        store.Open(OpenFlags.ReadWrite);
        //        store.Add(cert);

        //        store.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.WriteLine(ex.ToString());
        //    }

        //    return bRet;
        //}
    }
}
