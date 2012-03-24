using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace openHistorian.Core.Unmanaged
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PageObject : IKeyType<PageObject>
    {
        [FieldOffset(0)]
        public int Index;
        [FieldOffset(4)]
        public uint MetaData;
        [FieldOffset(8)]
        public IntPtr Pointer;

        public int Size
        {
            get
            {
                return 16;
            }
        }

        public unsafe PageObject Load(byte* ptr)
        {
            return *(PageObject*)ptr;
        }

        public unsafe void Save(byte* ptr)
        {
            *(PageObject*)ptr = this;
        }

        public unsafe bool IsNotNull(byte* ptr)
        {
            return *(int*)ptr >= 0;
        }
    }
}
