//******************************************************************************************************
//  SHA1Core.cs - Gbtc
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
//  8/1/2014 - Steven E. Chisholm
//       Generated original version of source code base on Mono implementation. 
//       https://github.com/mono/mono/blob/master/mcs/class/corlib/System.Security.Cryptography/SHA256Managed.cs
//       There is some additional functionality that was needed. 
//
//******************************************************************************************************

//
// System.Security.Cryptography.SHA256Managed.cs
//
// Author:
//   Matthew S. Ford (Matthew.S.Ford@Rose-Hulman.Edu)
//
// (C) 2001 
// Copyright (C) 2004, 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using GSF.Security;

namespace System.Security.Cryptography
{

    public class SHA256Core
        : IHashFunction<SHA256Core>
    {
        // SHA-256 Constants
        // Represent the first 32 bits of the fractional parts of the
        // cube roots of the first sixty-four prime numbers
        public readonly static uint[] S_K1 = {
			0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5,
			0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
			0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3,
			0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
			0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC,
			0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
			0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7,
			0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
			0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13,
			0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
			0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3,
			0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
			0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5,
			0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
			0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208,
			0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2
		};

        private const int BLOCK_SIZE_BYTES = 64;
        private uint[] _H;
        private ulong count;
        private byte[] _ProcessingBuffer;   // Used to start data when passed less than a block worth.
        private int _ProcessingBufferCount; // Counts how much data we have stored that still needs processed.
        private uint[] buff;

        public SHA256Core()
        {
            _H = new uint[8];
            _ProcessingBuffer = new byte[BLOCK_SIZE_BYTES];
            buff = new uint[64];
            Initialize();
        }

        public void CopyStateTo(SHA256Core other)
        {
            _H.CopyTo(other._H, 0);
            other._ProcessingBufferCount = _ProcessingBufferCount;
            if (_ProcessingBufferCount > 0)
                Array.Copy(_ProcessingBuffer, 0, other._ProcessingBuffer, 0, _ProcessingBufferCount);
            other.count = count;
        }

        public void HashCore(byte[] rgb, int ibStart, int cbSize)
        {
            int i;

            if (_ProcessingBufferCount != 0)
            {
                if (cbSize < (BLOCK_SIZE_BYTES - _ProcessingBufferCount))
                {
                    Buffer.BlockCopy(rgb, ibStart, _ProcessingBuffer, _ProcessingBufferCount, cbSize);
                    _ProcessingBufferCount += cbSize;
                    return;
                }
                else
                {
                    i = (BLOCK_SIZE_BYTES - _ProcessingBufferCount);
                    Buffer.BlockCopy(rgb, ibStart, _ProcessingBuffer, _ProcessingBufferCount, i);
                    ProcessBlock(_ProcessingBuffer, 0);
                    _ProcessingBufferCount = 0;
                    ibStart += i;
                    cbSize -= i;
                }
            }

            for (i = 0; i < cbSize - cbSize % BLOCK_SIZE_BYTES; i += BLOCK_SIZE_BYTES)
            {
                ProcessBlock(rgb, ibStart + i);
            }

            if (cbSize % BLOCK_SIZE_BYTES != 0)
            {
                Buffer.BlockCopy(rgb, cbSize - cbSize % BLOCK_SIZE_BYTES + ibStart, _ProcessingBuffer, 0, cbSize % BLOCK_SIZE_BYTES);
                _ProcessingBufferCount = cbSize % BLOCK_SIZE_BYTES;
            }
        }

        public void HashFinal(byte[] hash, int offset)
        {
            int i, j;

            ProcessFinalBlock(_ProcessingBuffer, 0, _ProcessingBufferCount);

            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    hash[i * 4 + j + offset] = (byte)(_H[i] >> (24 - j * 8));
                }
            }
        }

        public byte[] HashFinal()
        {
            byte[] hash = new byte[32];
            HashFinal(hash, 0);
            return hash;
        }

        public int OutputSize
        {
            get
            {
                return 32;
            }
        }

        public int BlockSize
        {
            get
            {
                return 64;
            }
        }

        public void Initialize()
        {
            count = 0;
            _ProcessingBufferCount = 0;

            _H[0] = 0x6A09E667;
            _H[1] = 0xBB67AE85;
            _H[2] = 0x3C6EF372;
            _H[3] = 0xA54FF53A;
            _H[4] = 0x510E527F;
            _H[5] = 0x9B05688C;
            _H[6] = 0x1F83D9AB;
            _H[7] = 0x5BE0CD19;
        }

        private void ProcessBlock(byte[] inputBuffer, int inputOffset)
        {
            uint a, b, c, d, e, f, g, h;
            uint t1, t2;
            int i;
            uint[] K1 = S_K1;
            uint[] buff = this.buff;

            count += BLOCK_SIZE_BYTES;

            for (i = 0; i < 16; i++)
            {
                buff[i] = (uint)(((inputBuffer[inputOffset + 4 * i]) << 24)
                    | ((inputBuffer[inputOffset + 4 * i + 1]) << 16)
                    | ((inputBuffer[inputOffset + 4 * i + 2]) << 8)
                    | ((inputBuffer[inputOffset + 4 * i + 3])));
            }


            for (i = 16; i < 64; i++)
            {
                t1 = buff[i - 15];
                t1 = (((t1 >> 7) | (t1 << 25)) ^ ((t1 >> 18) | (t1 << 14)) ^ (t1 >> 3));

                t2 = buff[i - 2];
                t2 = (((t2 >> 17) | (t2 << 15)) ^ ((t2 >> 19) | (t2 << 13)) ^ (t2 >> 10));
                buff[i] = t2 + buff[i - 7] + t1 + buff[i - 16];
            }

            a = _H[0];
            b = _H[1];
            c = _H[2];
            d = _H[3];
            e = _H[4];
            f = _H[5];
            g = _H[6];
            h = _H[7];

            for (i = 0; i < 64; i++)
            {
                t1 = h + (((e >> 6) | (e << 26)) ^ ((e >> 11) | (e << 21)) ^ ((e >> 25) | (e << 7))) + ((e & f) ^ (~e & g)) + K1[i] + buff[i];

                t2 = (((a >> 2) | (a << 30)) ^ ((a >> 13) | (a << 19)) ^ ((a >> 22) | (a << 10)));
                t2 = t2 + ((a & b) ^ (a & c) ^ (b & c));
                h = g;
                g = f;
                f = e;
                e = d + t1;
                d = c;
                c = b;
                b = a;
                a = t1 + t2;
            }

            _H[0] += a;
            _H[1] += b;
            _H[2] += c;
            _H[3] += d;
            _H[4] += e;
            _H[5] += f;
            _H[6] += g;
            _H[7] += h;
        }

        private void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ulong total = count + (ulong)inputCount;
            int paddingSize = (56 - (int)(total % BLOCK_SIZE_BYTES));

            if (paddingSize < 1)
                paddingSize += BLOCK_SIZE_BYTES;

            byte[] fooBuffer = new byte[inputCount + paddingSize + 8];

            for (int i = 0; i < inputCount; i++)
            {
                fooBuffer[i] = inputBuffer[i + inputOffset];
            }

            fooBuffer[inputCount] = 0x80;
            for (int i = inputCount + 1; i < inputCount + paddingSize; i++)
            {
                fooBuffer[i] = 0x00;
            }

            // I deal in bytes. The algorithm deals in bits.
            ulong size = total << 3;
            AddLength(size, fooBuffer, inputCount + paddingSize);
            ProcessBlock(fooBuffer, 0);

            if (inputCount + paddingSize + 8 == 128)
            {
                ProcessBlock(fooBuffer, 64);
            }
        }

        internal void AddLength(ulong length, byte[] buffer, int position)
        {
            buffer[position++] = (byte)(length >> 56);
            buffer[position++] = (byte)(length >> 48);
            buffer[position++] = (byte)(length >> 40);
            buffer[position++] = (byte)(length >> 32);
            buffer[position++] = (byte)(length >> 24);
            buffer[position++] = (byte)(length >> 16);
            buffer[position++] = (byte)(length >> 8);
            buffer[position] = (byte)(length);
        }
    }
}