//******************************************************************************************************
//  SrpServerSession.cs - Gbtc
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
using System.CodeDom;
using System.Security.Cryptography;
using System.Text;
using GSF.Collections;
using GSF.IO.Unmanaged;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;

namespace GSF.Security
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class SrpServerSession
    {
        /// <summary>
        /// Gets if the ticket is valid.
        /// </summary>
        public bool IsValid;
        /// <summary>
        /// The key that the server has to decode the ticket.
        /// </summary>
        public Guid ServerKeyName;
        /// <summary>
        /// The time that the ticket was origionally created.
        /// </summary>
        public DateTime AuthenticationTime;
        /// <summary>
        /// The username of the session.
        /// </summary>
        public string Username;
        /// <summary>
        /// The byte representation of the username
        /// </summary>
        public byte[] UsernameBytes;
        /// <summary>
        /// The session secret that is used to generate keys.
        /// </summary>
        public byte[] SessionSecret;
        /// <summary>
        /// The session key
        /// </summary>
        public byte[] SessionKey;
        /// <summary>
        /// The initialization vector to use to decrypt the message.
        /// </summary>
        public byte[] InitializationVector;
        /// <summary>
        /// The raw bytes for the ticket. This value is safe to transcode.
        /// </summary>
        public byte[] Ticket;

        unsafe public SrpServerSession(string username, byte[] usernameBytes, byte[] sessionSecret, byte[] sessionKey, Guid serverKeyName, byte[] serverHMACKey, byte[] serverEncryptionkey)
        {
            IsValid = true;
            ServerKeyName = serverKeyName;
            AuthenticationTime = DateTime.UtcNow;
            Username = username;
            UsernameBytes = usernameBytes;
            SessionSecret = sessionSecret;
            SessionKey = sessionKey;
            InitializationVector = SaltGenerator.Create(16);

            int len = sessionSecret.Length;
            int blockLen = (len + 15) & ~15; //Add 15, then round down. (Effecitvely rounds up to the nearest 128 bit boundary).
            byte[] dataToEncrypt = new byte[blockLen];
            sessionSecret.CopyTo(dataToEncrypt, 0);

            //fill the remainder of the block with random bits
            if (len != blockLen)
                SaltGenerator.Create(blockLen - len).CopyTo(dataToEncrypt, len);

            Ticket = new byte[1 + 16 + 8 + 2 + usernameBytes.Length + 16 + 2 + blockLen + 32];

            var aes = new RijndaelManaged();
            aes.Key = serverEncryptionkey;
            aes.IV = InitializationVector;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            var decrypt = aes.CreateEncryptor();
            var encryptedData = decrypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);

            fixed (byte* lp = Ticket)
            {
                var stream = new BinaryStreamPointerWrapper(lp, Ticket.Length);
                stream.Write((byte)1);
                stream.Write((short)usernameBytes.Length);
                stream.Write((short)len);
                stream.Write(ServerKeyName);
                stream.Write(AuthenticationTime);
                stream.Write(InitializationVector);
                stream.Write(usernameBytes);
                stream.Write(encryptedData);
                stream.Write(HMAC<Sha256Digest>.Compute(serverHMACKey, Ticket, 0, Ticket.Length - 32));
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticket">The serialized data for the key</param>
        /// <param name="serverKeyName">the name of server decryption and signing keys</param>
        /// <param name="serverHMACKey">a 32 byte signature key</param>
        /// <param name="serverEncryptionkey">a 32 byte encryption key</param>
        unsafe public SrpServerSession(byte[] ticket, Guid serverKeyName, byte[] serverHMACKey, byte[] serverEncryptionkey)
        {
            IsValid = false;
            ServerKeyName = serverKeyName;
            //Ticket Structure:
            //  byte      TicketVersion = 1
            //  int16     Username Length
            //  int16     Session Key Length 
            //  Guid      ServerKeyName
            //  DateTime  AuthenticationTime
            //  byte[16]  IV
            //  byte[]    Username
            //  byte[]    Encrypted Session Data (Round [Session Key Length] up to a 128 bit number to get the length of the encrypted data)
            //  byte[32]  HMAC (Sha2-256)

            if (ticket == null || ticket.Length < 1 + 16 + 8 + 16 + 2 + 32)
                return;
            if (serverHMACKey == null || serverHMACKey.Length != 32)
                return;
            if (serverEncryptionkey == null || serverEncryptionkey.Length != 32)
                return;

            fixed (byte* lp = ticket)
            {
                var stream = new BinaryStreamPointerWrapper(lp, ticket.Length);
                if (stream.ReadUInt8() != 1)
                    return;

                int usernameLength = stream.ReadUInt16();
                if (usernameLength < 0 || usernameLength > 1024)
                    return;

                int sessionKeyLength = stream.ReadUInt16();
                if (sessionKeyLength < 0 || sessionKeyLength > 1024) //Max session key is 8192 SRP.
                    return;

                int encryptedDataLength = (sessionKeyLength + 15) & ~15; //Add 15, then round down. (Effecitvely rounds up to the nearest 128 bit boundary).
                if (ticket.Length != 1 + 2 + 2 + 16 + 8 + 16 + usernameLength + encryptedDataLength + 32)
                    return;

                if (!serverKeyName.SecureEquals(stream.ReadGuid()))
                    return;

                long maxTicketAge = DateTime.UtcNow.Ticks + TimeSpan.TicksPerMinute * 10; //Allows for time syncing issues that might move the server's time back by a few minutes.
                long minTicketAge = maxTicketAge - TimeSpan.TicksPerDay;

                long issueTime = stream.ReadInt64();
                if (issueTime < minTicketAge || issueTime > maxTicketAge)
                    return;

                AuthenticationTime = new DateTime(issueTime);
                InitializationVector = stream.ReadBytes(16);

                //Verify the signature if everything else looks good.
                //This is last because it is the most computationally complex.
                //This limits denial of service attackes.
                byte[] hmac = HMAC<Sha256Digest>.Compute(serverHMACKey, ticket, 0, ticket.Length - 32);
                if (!hmac.SecureEquals(ticket, ticket.Length - 32, 32))
                    return;

                UsernameBytes = stream.ReadBytes(usernameLength);
                Username = Encoding.UTF8.GetString(UsernameBytes);

                byte[] encryptedData = stream.ReadBytes(encryptedDataLength);
                var aes = new RijndaelManaged();
                aes.Key = serverEncryptionkey;
                aes.IV = InitializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding=PaddingMode.None;
                var decrypt = aes.CreateDecryptor();
                SessionSecret = decrypt.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                IsValid = true;
            }


        }
    }
}


