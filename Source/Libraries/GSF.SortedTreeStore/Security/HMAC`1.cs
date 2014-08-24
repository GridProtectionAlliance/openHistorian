using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace GSF.Security
{
    public static class HMAC
    {
        public static byte[] Compute(IDigest hash, byte[] key, byte[] data)
        {
            var hmac = new HMac(hash);
            hmac.Init(new KeyParameter(key));
            var result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(data, 0, data.Length);
            hmac.DoFinal(result, 0);
            return result;
        }
    }

    public static class HMAC<T>
      where T : IDigest, new()
    {
        public static byte[] Compute(byte[] key, byte[] data)
        {
            return HMAC.Compute(new T(), key, data);
        }
    }
}
