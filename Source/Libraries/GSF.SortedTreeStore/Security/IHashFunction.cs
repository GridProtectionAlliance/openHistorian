using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Security
{
    public interface IHashFunction<T>
    {
        int OutputSize { get; }
        int BlockSize { get; }
        void Initialize();
        void HashCore(byte[] rgb, int ibStart, int cbSize);
        void HashFinal(byte[] hash, int offset);
        byte[] HashFinal();
        void CopyStateTo(T other);

    }
}
