//******************************************************************************************************
//  Murmur3.cs - Gbtc
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
//  1/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

// Checksum is based on code found at the following websites
// http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
// http://en.wikipedia.org/wiki/MurmurHash
// http://code.google.com/p/smhasher/wiki/MurmurHash3

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// A specialized implementation of MurMur3 that requires the data be aligned 
    /// to 16-byte boundaries.
    /// </summary>
    internal static unsafe class Murmur3
    {
        public static void ComputeHash(byte* bb, int length, out ulong checksum1, out ulong checksum2)
        {
            if ((length & 15) != 0)
                throw new Exception("Checksum only valid for a length multiple of 16");

            const uint seed = 0;
            const ulong C1 = 0x87c37b91114253d5L;
            const ulong C2 = 0x4cf5ad432745937fL;

            ulong* blocks = (ulong*)bb;
            ulong h1;
            ulong h2;
            int nblocks = length >> 4;
            h1 = seed;
            h2 = seed;

            for (int i = 0; i < nblocks; i++)
            {
                ulong k1 = blocks[2 * i + 0];
                ulong k2 = blocks[2 * i + 1];

                k1 *= C1;
                k1 = (k1 << 31) | (k1 >> (64 - 31));
                k1 *= C2;
                h1 ^= k1;

                h1 = (h1 << 27) | (h1 >> (64 - 27));
                h1 += h2;
                h1 = h1 * 5 + 0x52dce729;

                k2 *= C2;
                k2 = (k2 << 33) | (k2 >> (64 - 33));
                k2 *= C1;
                h2 ^= k2;


                h2 = (h2 << 31) | (h2 >> (64 - 31));
                h2 += h1;
                h2 = h2 * 5 + 0x38495ab5;
            }

            //Since this will always align on 16 byte boundaries, I can skip the tail section

            h1 ^= (ulong)length;
            h2 ^= (ulong)length;

            h1 += h2;
            h2 += h1;

            h1 ^= h1 >> 33;
            h1 *= 0xff51afd7ed558ccdL;
            h1 ^= h1 >> 33;
            h1 *= 0xc4ceb9fe1a85ec53L;
            h1 ^= h1 >> 33;

            h2 ^= h2 >> 33;
            h2 *= 0xff51afd7ed558ccdL;
            h2 ^= h2 >> 33;
            h2 *= 0xc4ceb9fe1a85ec53L;
            h2 ^= h2 >> 33;

            h1 += h2;
            h2 += h1;

            checksum1 = h1;
            checksum2 = h2;
        }
    }

    [Obsolete("For testing only")]
    internal static class IntHelpers
    {
        public static ulong RotateLeft(this ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        public static ulong RotateRight(this ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        public static unsafe ulong GetUInt64(this byte[] bb, int pos)
        {
            // we only read aligned longs, so a simple casting is enough
            fixed (byte* pbyte = &bb[pos])
            {
                return *(ulong*)pbyte;
            }
        }
    }

    [Obsolete("For testing only")]
    internal class Murmur3Orig
    {
        // 128 bit output, 64 bit platform version

        public static ulong READ_SIZE = 16;
        private static readonly ulong C1 = 0x87c37b91114253d5L;
        private static readonly ulong C2 = 0x4cf5ad432745937fL;

        private ulong length;
        private uint seed; // if want to start with a seed, create a constructor
        private ulong h1;
        private ulong h2;

        private void MixBody(ulong k1, ulong k2)
        {
            h1 ^= MixKey1(k1);

            h1 = h1.RotateLeft(27);
            h1 += h2;
            h1 = h1 * 5 + 0x52dce729;

            h2 ^= MixKey2(k2);

            h2 = h2.RotateLeft(31);
            h2 += h1;
            h2 = h2 * 5 + 0x38495ab5;
        }

        private static ulong MixKey1(ulong k1)
        {
            k1 *= C1;
            k1 = k1.RotateLeft(31);
            k1 *= C2;
            return k1;
        }

        private static ulong MixKey2(ulong k2)
        {
            k2 *= C2;
            k2 = k2.RotateLeft(33);
            k2 *= C1;
            return k2;
        }

        private static ulong MixFinal(ulong k)
        {
            // avalanche bits

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }

        public byte[] ComputeHash(byte[] bb)
        {
            ProcessBytes(bb);
            return Hash;
        }

        private void ProcessBytes(byte[] bb)
        {
            h1 = seed;
            h2 = seed;
            this.length = 0L;

            int pos = 0;
            ulong remaining = (ulong)bb.Length;

            // read 128 bits, 16 bytes, 2 longs in eacy cycle
            while (remaining >= READ_SIZE)
            {
                ulong k1 = bb.GetUInt64(pos);
                pos += 8;

                ulong k2 = bb.GetUInt64(pos);
                pos += 8;

                length += READ_SIZE;
                remaining -= READ_SIZE;

                MixBody(k1, k2);
            }

            // if the input MOD 16 != 0
            if (remaining > 0)
                ProcessBytesRemaining(bb, remaining, pos);
        }

        private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos)
        {
            ulong k1 = 0;
            ulong k2 = 0;
            length += remaining;

            // little endian (x86) processing
            switch (remaining)
            {
                case 15:
                    k2 ^= (ulong)bb[pos + 14] << 48; // fall through
                    goto case 14;
                case 14:
                    k2 ^= (ulong)bb[pos + 13] << 40; // fall through
                    goto case 13;
                case 13:
                    k2 ^= (ulong)bb[pos + 12] << 32; // fall through
                    goto case 12;
                case 12:
                    k2 ^= (ulong)bb[pos + 11] << 24; // fall through
                    goto case 11;
                case 11:
                    k2 ^= (ulong)bb[pos + 10] << 16; // fall through
                    goto case 10;
                case 10:
                    k2 ^= (ulong)bb[pos + 9] << 8; // fall through
                    goto case 9;
                case 9:
                    k2 ^= bb[pos + 8]; // fall through
                    goto case 8;
                case 8:
                    k1 ^= bb.GetUInt64(pos);
                    break;
                case 7:
                    k1 ^= (ulong)bb[pos + 6] << 48; // fall through
                    goto case 6;
                case 6:
                    k1 ^= (ulong)bb[pos + 5] << 40; // fall through
                    goto case 5;
                case 5:
                    k1 ^= (ulong)bb[pos + 4] << 32; // fall through
                    goto case 4;
                case 4:
                    k1 ^= (ulong)bb[pos + 3] << 24; // fall through
                    goto case 3;
                case 3:
                    k1 ^= (ulong)bb[pos + 2] << 16; // fall through
                    goto case 2;
                case 2:
                    k1 ^= (ulong)bb[pos + 1] << 8; // fall through
                    goto case 1;
                case 1:
                    k1 ^= bb[pos]; // fall through
                    break;
                default:
                    throw new Exception("Something went wrong with remaining bytes calculation.");
            }

            h1 ^= MixKey1(k1);
            h2 ^= MixKey2(k2);
        }

        public byte[] Hash
        {
            get
            {
                h1 ^= length;
                h2 ^= length;

                h1 += h2;
                h2 += h1;

                h1 = MixFinal(h1);
                h2 = MixFinal(h2);

                h1 += h2;
                h2 += h1;

                byte[] hash = new byte[READ_SIZE];

                Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
                Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

                return hash;
            }
        }
    }
}