using System.Security.Policy;
using GSF.Security;

namespace System.Security.Cryptography
{
    public class HMAC<T>
        : IHashAlgorithm
        where T : IHashFunction<T>, new()
    {
        public static byte[] Compute(byte[] key, byte[] message)
        {
            T hash = new T();
            key = PadKey(key, hash);

            for (int x = 0; x < key.Length; x++)
            {
                //Key is inner key
                key[x] ^= 0x36;
            }

            hash.Initialize();
            hash.HashCore(key, 0, key.Length);
            hash.HashCore(message,0,message.Length);

            byte[] innerCode = hash.HashFinal();

            for (int x = 0; x < key.Length; x++)
            {
                //Key is outer key
                key[x] ^= 0x5c ^ 0x36; //Undo the inner, then transorm outer
            }

            hash.Initialize();
            hash.HashCore(key, 0, key.Length);
            hash.HashCore(innerCode, 0, innerCode.Length);
            hash.HashFinal(innerCode, 0);
            return innerCode;
        }

        private byte[] m_innerHash;
        private T m_digest;
        private T m_outterDigest;
        private T m_innerDigest;
        private int m_blockSize;
        private int m_outputSize;

        public HMAC(byte[] key)
        {
            m_digest = new T();
            m_blockSize = m_digest.BlockSize;
            m_outputSize = m_digest.OutputSize;

            m_outterDigest = new T();
            m_innerDigest = new T();
            m_innerHash = new byte[m_outputSize];

            key = PadKey(key, m_digest);
            byte[] outterKey = (byte[])key.Clone();
            byte[] innerKey = (byte[])key.Clone();

            for (int x = 0; x < m_blockSize; x++)
            {
                outterKey[x] ^= 0x5c;
                innerKey[x] ^= 0x36;
            }

            m_outterDigest.Initialize();
            m_outterDigest.HashCore(outterKey, 0, m_blockSize);

            m_innerDigest.Initialize();
            m_innerDigest.HashCore(innerKey, 0, m_blockSize);
            Initialize();
        }

        static byte[] PadKey(byte[] orig, T digest)
        {
            int blockSize = digest.BlockSize;
            if (orig.Length == blockSize)
                return (byte[])orig.Clone();

            byte[] rv = new byte[blockSize];
            if (orig.Length < blockSize)
            {
                orig.CopyTo(rv, 0);
                return rv;
            }

            digest.Initialize();
            digest.HashCore(orig, 0, orig.Length);
            digest.HashFinal().CopyTo(rv, 0);
            return rv;
        }

        public void Initialize()
        {
            m_innerDigest.CopyStateTo(m_digest);
        }

        protected void HashCore(byte[] array, int ibStart, int cbSize)
        {
            m_digest.HashCore(array, ibStart, cbSize);
        }

        protected byte[] HashFinal()
        {
            byte[] rv = new byte[m_outputSize];
            HashFinal(rv);
            return rv;
        }

        public void HashFinal(byte[] hash)
        {
            m_digest.HashFinal(m_innerHash, 0);
            m_outterDigest.CopyStateTo(m_digest);
            m_digest.HashCore(m_innerHash, 0, m_outputSize);
            m_digest.HashFinal(hash, 0);
        }

        public byte[] ComputeHash(byte[] data)
        {
            Initialize();
            HashCore(data, 0, data.Length);
            return HashFinal();
        }

        public void ComputeHash(byte[] data, byte[] hash)
        {
            Initialize();
            HashCore(data, 0, data.Length);
            HashFinal(hash);
        }

        public int OutputSize
        {
            get
            {
                return m_digest.OutputSize;
            }
        }

        public int BlockSize
        {
            get
            {
                return m_digest.BlockSize;
            }
        }
    }
}