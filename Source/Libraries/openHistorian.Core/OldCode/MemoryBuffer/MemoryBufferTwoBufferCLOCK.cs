using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Historian.MemoryBuffer
{
    /// <summary>
    /// This class implements a memory buffer method similiar to the way MySQL does it in InnoDB.
    /// </summary>
    public class MemoryBufferTwoBufferCLOCK : IMemoryBuffer
    {
        #region [ Members ]
        #region [ Constants ]

        #endregion
        #region [ Delegates ]
        #endregion
        #region [ Events ]
        #endregion
        #region [ Fields ]
        
        /// <summary>
        /// Contains the next available DataSetID
        /// </summary>
        long m_NextDataSetID;
        /// <summary>
        /// Determines the number of bytes that exists in each block.
        /// Must be a power of 2
        /// </summary>
        int m_BlockSize;
        /// <summary>
        /// The number of bits taken up by the block.  Since the blocks are aligned
        /// taking the absolute address and right shifting by this value will get the 
        /// block number
        /// </summary>
        int m_pageBits;
        /// <summary>
        /// The mask that can be AND'ed to the absolute address to find the relative address
        /// within the block
        /// </summary>
        int m_pageMask;

        /// <summary>
        /// Determines the maximum allowable pages that can be in the memory buffer
        /// </summary>
        int m_TotalBufferBlocks;

        /// <summary>
        /// Buffer 0 contains all of the blocks that have been read only once, but have not been re-referenced
        /// </summary>
        Queue<MemoryUnit> BufferLevel0;
        /// <summary>
        /// Buffer 1 contains all of the blocks that have been re-ferenced multiple times
        /// </summary>
        Queue<MemoryUnit> BufferLevel1;
        
        /// <summary>
        /// Contains a lookup dictionary of the blocks.  
        /// The first key lookup is the DataSetID.
        /// The second key is the block number
        /// </summary>
        Dictionary<long, Dictionary<uint, MemoryUnit>> AllocatedMemory = new Dictionary<long, Dictionary<uint, MemoryUnit>>();

        #endregion
        #endregion

        #region [ Constructors ]
  
        /// <summary>
        /// Creates an expanding memory buffer with a maximum size that is provided.
        /// </summary>
        /// <param name="initialSize"></param>
        public MemoryBufferTwoBufferCLOCK(long maximumSize)
        {
            m_BlockSize = 65536;
            m_pageBits = 16;
            m_pageMask = m_BlockSize - 1;
            m_TotalBufferBlocks = (int)(maximumSize / m_BlockSize);
            BufferLevel0 = new Queue<MemoryUnit>(m_TotalBufferBlocks);
            BufferLevel1 = new Queue<MemoryUnit>(m_TotalBufferBlocks);
            for (int x = 0; x < m_TotalBufferBlocks; x += 2)
            {
                BufferLevel0.Enqueue(new MemoryUnit(m_BlockSize));
                BufferLevel1.Enqueue(new MemoryUnit(m_BlockSize));
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The number of bytes in each block
        /// </summary>
        public int BlockSize
        {
            get { return m_BlockSize; }
        }

        /// <summary>
        /// The maximum size of the buffer space.
        /// </summary>
        public long MaximumBufferSize
        {
            get { return (long)m_TotalBufferBlocks * BlockSize; }
        }

        /// <summary>
        /// The number of bytes that are currently allocated
        /// </summary>
        public long AllocatedBufferSize
        {
            get { return (long)(BufferLevel0.Count + BufferLevel1.Count) * BlockSize; }
        }
       
        public int BlockBits
        {
            get { return m_pageBits; }
        }

        public int BlockMask
        {
            get { return m_pageMask; }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// This will remove all instances of this file from the buffer space.
        /// </summary>
        /// <param name="fileID"></param>
        public void FreeBufferFromFile(long fileID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This will mark this page as unallocated.
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="address"></param>
        public void FreeBufferFromPage(long fileID, uint address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to find the memory cache.  
        /// If it is found, the memory unit is returned and pressure is added to the object. 
        /// If not found, this function returns null
        /// </summary>
        /// <param name="DataSetID"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <remarks>When this memory page is no longer needed, call ReleasePressure method of the Memory Unit</remarks>
        public MemoryUnit LookupPage(long DataSetID, uint address)
        {
            Dictionary<uint, MemoryUnit> pageTable;
            if (AllocatedMemory.TryGetValue(DataSetID, out pageTable))
            {
                MemoryUnit memUnit;
                if (pageTable.TryGetValue(address, out memUnit))
                {
                    memUnit.AddPressure();
                    return memUnit;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the next available page from the buffer.
        /// </summary>
        /// <returns></returns>
        public MemoryUnit GetFreePage()
        {
            for (; ; )
            {
                MemoryUnit next = BufferLevel0.Dequeue();
                if (next.IsReferenced || next.IsDirty)
                {
                    BufferLevel1.Enqueue(next);
                    BufferLevel0.Enqueue(BufferLevel1.Dequeue());
                }
                else if (next.IsRead)
                {
                    next.IsRead = false;
                    BufferLevel1.Enqueue(next);
                    BufferLevel0.Enqueue(BufferLevel1.Dequeue());
                }
                else
                {
                    return next;
                }
                
            }
           
        }

        /// <summary>
        /// Adds the chunk of memory back to the memory buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public void AddToBuffer(long DataSetID, uint address, MemoryUnit buffer)
        {
            buffer.Initialize(DataSetID, address);
            BufferLevel0.Enqueue(buffer);

            Dictionary<uint, MemoryUnit> pageTable;
            if (AllocatedMemory.TryGetValue(buffer.DataSetID, out pageTable))
            {
                pageTable.Add(buffer.BlockIndex, buffer);
            }
            else
            {
                pageTable = new Dictionary<uint, MemoryUnit>();
                pageTable.Add(buffer.BlockIndex, buffer);
                AllocatedMemory.Add(buffer.DataSetID, pageTable);
            }
        }

        /// <summary>
        /// Returns the next available data set ID value
        /// </summary>
        /// <returns></returns>
        public long GetNextDataSetID()
        {
            return Interlocked.Increment(ref m_NextDataSetID);
        }

        #endregion
        #region [ Operators ]
        #endregion
        #region [ Static ]
        #region [ Fields ]
        #endregion
        #region [ Constructors ]
        #endregion
        #region [ Properties ]
        #endregion
        #region [ Methods ]
        #endregion
        #endregion

    }
}
