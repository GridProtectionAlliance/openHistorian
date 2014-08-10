//
// System.Security.Cryptography.SHA512Managed.cs
//
// Authors:
//	Dan Lewis (dihlewis@yahoo.co.uk)
//	Sebastien Pouliot (sebastien@ximian.com)
//
// (C) 2002
// Implementation translated from Bouncy Castle JCE (http://www.bouncycastle.org/)
// See bouncycastle.txt for license.
// Copyright (C) 2004-2005 Novell, Inc (http://www.novell.com)
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

using System.Runtime.CompilerServices;
using GSF.Security;

namespace System.Security.Cryptography
{
    public class SHA512Core
        : IHashFunction<SHA512Core>
    {
        // SHA-384 and SHA-512 Constants
        // Represent the first 64 bits of the fractional parts of the
        // cube roots of the first sixty-four prime numbers
        public readonly static ulong[] S_K2 = {
			0x428a2f98d728ae22L, 0x7137449123ef65cdL, 0xb5c0fbcfec4d3b2fL, 0xe9b5dba58189dbbcL,
			0x3956c25bf348b538L, 0x59f111f1b605d019L, 0x923f82a4af194f9bL, 0xab1c5ed5da6d8118L,
			0xd807aa98a3030242L, 0x12835b0145706fbeL, 0x243185be4ee4b28cL, 0x550c7dc3d5ffb4e2L,
			0x72be5d74f27b896fL, 0x80deb1fe3b1696b1L, 0x9bdc06a725c71235L, 0xc19bf174cf692694L,
			0xe49b69c19ef14ad2L, 0xefbe4786384f25e3L, 0x0fc19dc68b8cd5b5L, 0x240ca1cc77ac9c65L,
			0x2de92c6f592b0275L, 0x4a7484aa6ea6e483L, 0x5cb0a9dcbd41fbd4L, 0x76f988da831153b5L,
			0x983e5152ee66dfabL, 0xa831c66d2db43210L, 0xb00327c898fb213fL, 0xbf597fc7beef0ee4L,
			0xc6e00bf33da88fc2L, 0xd5a79147930aa725L, 0x06ca6351e003826fL, 0x142929670a0e6e70L,
			0x27b70a8546d22ffcL, 0x2e1b21385c26c926L, 0x4d2c6dfc5ac42aedL, 0x53380d139d95b3dfL,
			0x650a73548baf63deL, 0x766a0abb3c77b2a8L, 0x81c2c92e47edaee6L, 0x92722c851482353bL,
			0xa2bfe8a14cf10364L, 0xa81a664bbc423001L, 0xc24b8b70d0f89791L, 0xc76c51a30654be30L,
			0xd192e819d6ef5218L, 0xd69906245565a910L, 0xf40e35855771202aL, 0x106aa07032bbd1b8L,
			0x19a4c116b8d2d0c8L, 0x1e376c085141ab53L, 0x2748774cdf8eeb99L, 0x34b0bcb5e19b48a8L,
			0x391c0cb3c5c95a63L, 0x4ed8aa4ae3418acbL, 0x5b9cca4f7763e373L, 0x682e6ff3d6b2b8a3L,
			0x748f82ee5defb2fcL, 0x78a5636f43172f60L, 0x84c87814a1f0ab72L, 0x8cc702081a6439ecL,
			0x90befffa23631e28L, 0xa4506cebde82bde9L, 0xbef9a3f7b2c67915L, 0xc67178f2e372532bL,
			0xca273eceea26619cL, 0xd186b8c721c0c207L, 0xeada7dd6cde0eb1eL, 0xf57d4f7fee6ed178L,
			0x06f067aa72176fbaL, 0x0a637dc5a2c898a6L, 0x113f9804bef90daeL, 0x1b710b35131c471bL,
			0x28db77f523047d84L, 0x32caab7b40c72493L, 0x3c9ebe0a15c9bebcL, 0x431d67c49c100d4cL,
			0x4cc5d4becb3e42b6L, 0x597f299cfc657e2aL, 0x5fcb6fab3ad6faecL, 0x6c44198c4a475817L
		};

        private byte[] xBuf;
        private int xBufOff;

        private ulong byteCount1;
        private ulong byteCount2;

        private ulong H1, H2, H3, H4, H5, H6, H7, H8;

        private ulong[] W;
        private int wOff;

        public SHA512Core()
        {
            xBuf = new byte[8];
            W = new ulong[80];
            Initialize(false); // limited initialization
        }

        public void CopyStateTo(SHA512Core other)
        {
            other.H1 = H1;
            other.H2 = H2;
            other.H3 = H3;
            other.H4 = H4;
            other.H5 = H5;
            other.H6 = H6;
            other.H7 = H7;
            other.H8 = H8;
            other.byteCount1 = byteCount1;
            other.byteCount2 = byteCount2;

            other.wOff = wOff;
            if (wOff > 0)
                Array.Copy(W, 0, other.W, 0, wOff);

            other.xBufOff = xBufOff;
            xBuf.CopyTo(other.xBuf, 0);
        }

        private void Initialize(bool reuse)
        {
            // SHA-512 initial hash value
            // The first 64 bits of the fractional parts of the square roots
            // of the first eight prime numbers
            H1 = 0x6a09e667f3bcc908L;
            H2 = 0xbb67ae8584caa73bL;
            H3 = 0x3c6ef372fe94f82bL;
            H4 = 0xa54ff53a5f1d36f1L;
            H5 = 0x510e527fade682d1L;
            H6 = 0x9b05688c2b3e6c1fL;
            H7 = 0x1f83d9abfb41bd6bL;
            H8 = 0x5be0cd19137e2179L;

            if (reuse)
            {
                byteCount1 = 0;
                byteCount2 = 0;

                xBufOff = 0;
                for (int i = 0; i < xBuf.Length; i++)
                    xBuf[i] = 0;

                wOff = 0;
                for (int i = 0; i != W.Length; i++)
                    W[i] = 0;
            }
        }

        public int OutputSize
        {
            get
            {
                return 64;
            }
        }

        public int BlockSize
        {
            get
            {
                return 128;
            }
        }

        public void Initialize()
        {
            Initialize(true); // reuse instance
        }

        // protected

        public void HashCore(byte[] rgb, int ibStart, int cbSize)
        {
            // fill the current word
            while ((xBufOff != 0) && (cbSize > 0))
            {
                update(rgb[ibStart]);
                ibStart++;
                cbSize--;
            }

            // process whole words.
            while (cbSize > xBuf.Length)
            {
                processWord(rgb, ibStart);
                ibStart += xBuf.Length;
                cbSize -= xBuf.Length;
                byteCount1 += (ulong)xBuf.Length;
            }

            // load in the remainder.
            while (cbSize > 0)
            {
                update(rgb[ibStart]);
                ibStart++;
                cbSize--;
            }
        }

        public void HashFinal(byte[] hash, int offset)
        {
            adjustByteCounts();

            ulong lowBitLength = byteCount1 << 3;
            ulong hiBitLength = byteCount2;

            // add the pad bytes.
            update(128);
            while (xBufOff != 0)
                update(0);

            processLength(lowBitLength, hiBitLength);
            processBlock();

            unpackWord(H1, hash, offset + 0);
            unpackWord(H2, hash, offset + 8);
            unpackWord(H3, hash, offset + 16);
            unpackWord(H4, hash, offset + 24);
            unpackWord(H5, hash, offset + 32);
            unpackWord(H6, hash, offset + 40);
            unpackWord(H7, hash, offset + 48);
            unpackWord(H8, hash, offset + 56);

            Initialize();
        }

        public byte[] HashFinal()
        {
            byte[] output = new byte[64];
            HashFinal(output, 0);
            return output;
        }



        private void update(byte input)
        {
            xBuf[xBufOff++] = input;
            if (xBufOff == xBuf.Length)
            {
                processWord(xBuf, 0);
                xBufOff = 0;
            }
            byteCount1++;
        }

        private void processWord(byte[] input, int inOff)
        {
            W[wOff++] = ((ulong)input[inOff] << 56)
                | ((ulong)input[inOff + 1] << 48)
                | ((ulong)input[inOff + 2] << 40)
                | ((ulong)input[inOff + 3] << 32)
                | ((ulong)input[inOff + 4] << 24)
                | ((ulong)input[inOff + 5] << 16)
                | ((ulong)input[inOff + 6] << 8)
                | ((ulong)input[inOff + 7]);
            if (wOff == 16)
                processBlock();
        }

        private void unpackWord(ulong word, byte[] output, int outOff)
        {
            output[outOff] = (byte)(word >> 56);
            output[outOff + 1] = (byte)(word >> 48);
            output[outOff + 2] = (byte)(word >> 40);
            output[outOff + 3] = (byte)(word >> 32);
            output[outOff + 4] = (byte)(word >> 24);
            output[outOff + 5] = (byte)(word >> 16);
            output[outOff + 6] = (byte)(word >> 8);
            output[outOff + 7] = (byte)word;
        }

        // adjust the byte counts so that byteCount2 represents the
        // upper long (less 3 bits) word of the byte count.
        private void adjustByteCounts()
        {
            if (byteCount1 > 0x1fffffffffffffffL)
            {
                byteCount2 += (byteCount1 >> 61);
                byteCount1 &= 0x1fffffffffffffffL;
            }
        }

        private void processLength(ulong lowW, ulong hiW)
        {
            if (wOff > 14)
                processBlock();
            W[14] = hiW;
            W[15] = lowW;
        }

        private void processBlock()
        {
            adjustByteCounts();
            // expand 16 word block into 80 word blocks.
            for (int t = 16; t <= 79; t++)
                W[t] = Sigma1(W[t - 2]) + W[t - 7] + Sigma0(W[t - 15]) + W[t - 16];

            // set up working variables.
            ulong a = H1;
            ulong b = H2;
            ulong c = H3;
            ulong d = H4;
            ulong e = H5;
            ulong f = H6;
            ulong g = H7;
            ulong h = H8;

            for (int t = 0; t <= 79; t++)
            {
                ulong T1 = h + Sum1(e) + Ch(e, f, g) + S_K2[t] + W[t];
                ulong T2 = Sum0(a) + Maj(a, b, c);
                h = g;
                g = f;
                f = e;
                e = d + T1;
                d = c;
                c = b;
                b = a;
                a = T1 + T2;
            }

            H1 += a;
            H2 += b;
            H3 += c;
            H4 += d;
            H5 += e;
            H6 += f;
            H7 += g;
            H8 += h;
            // reset the offset and clean out the word buffer.
            wOff = 0;
            for (int i = 0; i != W.Length; i++)
                W[i] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong rotateRight(ulong x, int n)
        {
            return (x >> n) | (x << (64 - n));
        }

        /* SHA-512 and SHA-512 functions (as for SHA-256 but for longs) */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Ch(ulong x, ulong y, ulong z)
        {
            return ((x & y) ^ ((~x) & z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Maj(ulong x, ulong y, ulong z)
        {
            return ((x & y) ^ (x & z) ^ (y & z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Sum0(ulong x)
        {
            return rotateRight(x, 28) ^ rotateRight(x, 34) ^ rotateRight(x, 39);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Sum1(ulong x)
        {
            return rotateRight(x, 14) ^ rotateRight(x, 18) ^ rotateRight(x, 41);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Sigma0(ulong x)
        {
            return rotateRight(x, 1) ^ rotateRight(x, 8) ^ (x >> 7);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Sigma1(ulong x)
        {
            return rotateRight(x, 19) ^ rotateRight(x, 61) ^ (x >> 6);
        }
    }

}