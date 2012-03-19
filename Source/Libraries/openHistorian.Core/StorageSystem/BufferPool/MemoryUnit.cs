using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace openHistorian.Core.StorageSystem.BufferPool
{
    /// <summary>
    /// Contains basic information about a page of memory
    /// </summary>
    public unsafe class MemoryUnit
    {
        /// <summary>
        /// The number of times that this block has been referenced
        /// </summary>
        public int ReferencedCount;
        /// <summary>
        /// Determines if the page has been written to.
        /// </summary>
        public bool IsDirty;
        /// <summary>
        /// the index value of the data block
        /// </summary>
        public uint BlockIndex;
        /// <summary>
        /// The data.
        /// </summary>
        public byte[] DataSpace;

        //public MemoryUnit(int size)
        //{
        //    ReferencedCount = 0;
        //    IsDirty = false;
        //    BlockIndex = 0;
        //    DataSpace = new byte[size];
        //}

        public int BufferIndex;
        public byte* Pointer;
        public uint FileId;
        public uint AddressBlock;
    }
}
