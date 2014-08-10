using GSF.Security;
using Org.BouncyCastle.Crypto;

namespace System.Security.Cryptography
{
    public class Hash<T>
        : IHashAlgorithm
        where T : IHashFunction<T>, new()
    {
        public static byte[] Compute(byte[] data)
        {
            T hash = new T();
            hash.Initialize();
            hash.HashCore(data, 0, data.Length);
            return hash.HashFinal();
        }

        private byte[] m_innerHash;
        private T m_digest;
        private int m_blockSize;
        private int m_outputSize;

        public Hash()
        {
            m_digest = new T();
            m_blockSize = m_digest.BlockSize;
            m_outputSize = m_digest.OutputSize;
            Initialize();
        }

        public void Initialize()
        {
            m_digest.Initialize();
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