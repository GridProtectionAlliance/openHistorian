using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.StorageSystem.File
{
    public unsafe interface IMemoryUnit : IDisposable
    {
        bool IsValid { get; }
        bool IsReadOnly { get; }
        uint BlockIndex { get; }
        int Length { get; }
        byte* Pointer { get; }
    }
}
