
using System.Diagnostics;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    /// Contains basic information about a page of memory
    /// </summary>
    public unsafe class MemoryUnit
    {
        public byte* Pointer;
        public uint PageIndex;
        public uint Length;

        public MemoryUnit(uint pageIndex, byte* pointer, uint length)
        {
            PageIndex = pageIndex;
            Pointer = pointer;
            Length = length;
        }

        ~MemoryUnit()
        {
            Debugger.Break();
            Debug.Assert(false,"Memory object failed to properly be disposed of.");
        }

    }
}
