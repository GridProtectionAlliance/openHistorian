using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.MemoryBuffer
{
    public interface IMemoryBuffer
    {
        int BlockSize { get; }
        int BlockBits { get; }
        int BlockMask { get; }

        long MaximumBufferSize { get; }
        long AllocatedBufferSize { get; }

        void FreeBufferFromFile(long DataSetID);
        void FreeBufferFromPage(long DataSetID, uint address);
        long GetNextDataSetID();
        /// <summary>
        /// Makes an attempt to find the following data in the memory buffer.  
        /// Returns null if it is not in the buffer.
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        MemoryUnit LookupPage(long DataSetID, uint address);
        MemoryUnit GetFreePage();

        void AddToBuffer(long DataSetID, uint address, MemoryUnit buffer);
    }
}
