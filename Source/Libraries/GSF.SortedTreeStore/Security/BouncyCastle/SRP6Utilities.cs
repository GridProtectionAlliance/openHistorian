//
// Copyright (c) 2000 - 2013 The Legion of the Bouncy Castle Inc. 
// (http://www.bouncycastle.org)

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the 
// Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, subject to the 
// following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Agreement.Srp
{
    internal static class Srp6Utilities
    {
        public static BigInteger CalculateK(IDigest digest, BigInteger N, BigInteger g)
        {
            return HashPaddedPair(digest, N, N, g);
        }

        public static BigInteger CalculateU(IDigest digest, BigInteger N, BigInteger A, BigInteger B)
        {
            return HashPaddedPair(digest, N, A, B);
        }

        public static BigInteger CalculateX(IDigest digest, BigInteger N, byte[] salt, byte[] identity, byte[] password)
        {
            byte[] output = new byte[digest.GetDigestSize()];

            digest.BlockUpdate(identity, 0, identity.Length);
            digest.Update((byte)':');
            digest.BlockUpdate(password, 0, password.Length);
            digest.DoFinal(output, 0);

            digest.BlockUpdate(salt, 0, salt.Length);
            digest.BlockUpdate(output, 0, output.Length);
            digest.DoFinal(output, 0);

            return new BigInteger(1, output).Mod(N);
        }

        public static BigInteger GeneratePrivateValue(BigInteger N, SecureRandom random)
        {
            int minBits = System.Math.Min(256, N.BitLength / 2);
            BigInteger min = BigInteger.One.ShiftLeft(minBits - 1);
            BigInteger max = N.Subtract(BigInteger.One);

            return BigIntegers.CreateRandomInRange(min, max, random);
        }

        public static BigInteger ValidatePublicValue(BigInteger N, BigInteger val)
        {
            val = val.Mod(N);

            // Check that val % N != 0
            if (val.Equals(BigInteger.Zero))
                throw new CryptoException("Invalid public value: 0");

            return val;
        }

        /// <summary>
        /// Pads n1 and n2 to the same number of bytes as N. Then hashes them.
        /// </summary>
        /// <returns>The hash Mod N</returns>
        private static BigInteger HashPaddedPair(IDigest digest, BigInteger N, BigInteger n1, BigInteger n2)
        {
            int padLength = (N.BitLength + 7) / 8;

            byte[] n1_bytes = GetPadded(n1, padLength);
            byte[] n2_bytes = GetPadded(n2, padLength);

            digest.BlockUpdate(n1_bytes, 0, n1_bytes.Length);
            digest.BlockUpdate(n2_bytes, 0, n2_bytes.Length);

            byte[] output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);

            return new BigInteger(1, output).Mod(N);
        }

        /// <summary>
        /// Pads a byte[] to the specified length, 
        /// with zeroes at the start of the buffer.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static byte[] GetPadded(BigInteger n, int length)
        {
            byte[] bs = BigIntegers.AsUnsignedByteArray(n);
            if (bs.Length < length)
            {
                byte[] tmp = new byte[length];
                Array.Copy(bs, 0, tmp, length - bs.Length, bs.Length);
                bs = tmp;
            }
            return bs;
        }
        /// <summary>
        /// Gets the byte representation of <see cref="n"/> that is padded to 
        /// match the byte length of <see cref="length"/>.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] ToPaddedArray(this BigInteger n, int length)
        {
            return GetPadded(n, length);
        }
    }
}
