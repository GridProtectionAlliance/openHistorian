using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace GSF.Security
{
    public static class HMAC<T>
      where T : IDigest, new()
    {
        public static byte[] Compute(byte[] key, byte[] data)
        {
            var hash = new T();
            var hmac = new HMac(hash);
            hmac.Init(new KeyParameter(key));
            var result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(data, 0, data.Length);
            hmac.DoFinal(result, 0);
            return result;
        }

    }
}
