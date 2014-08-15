using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.IO.FileStructure;
using Org.BouncyCastle.Crypto;

namespace GSF.Security
{
    public static class Hash<T>
        where T : IDigest, new()
    {
        public static byte[] Compute(byte[] data)
        {
            var hash = new T();
            var result = new byte[hash.GetDigestSize()];
            hash.BlockUpdate(data,0,data.Length);
            hash.DoFinal(result, 0);
            return result;
        }

    }
}
