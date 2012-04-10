using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Unmanaged
{
    unsafe public interface IKeyType<T>
    {
        int Size { get; }
        T Load(byte* ptr);
        void Save(byte* ptr);
        bool IsNotNull(byte* ptr);
    }
}
