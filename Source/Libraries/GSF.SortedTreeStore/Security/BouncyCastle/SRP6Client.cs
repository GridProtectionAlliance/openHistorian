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

using GSF.Security.Authentication;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Agreement.Srp
{
    /**
     * Implements the client side SRP-6a protocol. Note that this class is stateful, and therefore NOT threadsafe.
     * This implementation of SRP is based on the optimized message sequence put forth by Thomas Wu in the paper
     * "SRP-6: Improvements and Refinements to the Secure Remote Password Protocol, 2002"
     */
    internal class Srp6Client
    {
        private readonly SrpConstants param;

        protected BigInteger privA;
        protected BigInteger pubA;

        protected BigInteger B;

        protected BigInteger x;
        protected BigInteger u;
        protected BigInteger S;

        protected SecureRandom random;

        public Srp6Client(SrpConstants param)
        {
            this.param = param;
            random = new SecureRandom();
        }

        /**
         * Generates client's credentials given the client's salt, identity and password
         * @param salt The salt used in the client's verifier.
         * @param identity The user's identity (eg. username)
         * @param password The user's password
         * @return Client's public value to send to server
         */
        public virtual BigInteger GenerateClientCredentials(IDigest digest, byte[] salt, byte[] identity, byte[] password)
        {
            this.x = Srp6Utilities.CalculateX(digest, param.N, salt, identity, password);
            this.privA = SelectPrivateValue();
            this.pubA = param.g.ModPow(privA, param.N);

            return pubA;
        }

        /**
         * Generates client's verification message given the server's credentials
         * @param serverB The server's credentials
         * @return Client's verification message for the server
         * @throws CryptoException If server's credentials are invalid
         */
        public virtual BigInteger CalculateSecret(IDigest digest, BigInteger serverB)
        {
            this.B = Srp6Utilities.ValidatePublicValue(param.N, serverB);
            this.u = Srp6Utilities.CalculateU(digest, param.N, pubA, B);
            this.S = CalculateS();

            return S;
        }

        protected virtual BigInteger SelectPrivateValue()
        {
            return Srp6Utilities.GeneratePrivateValue(param.N, random);
        }

        private BigInteger CalculateS()
        {
            BigInteger exp = u.Multiply(x).Add(privA);
            BigInteger tmp = param.g.ModPow(x, param.N).Multiply(param.k).Mod(param.N);
            return B.Subtract(tmp).Mod(param.N).ModPow(exp, param.N);
        }
    }
}
