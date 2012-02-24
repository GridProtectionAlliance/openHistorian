using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace openHistorian.Core.MemoryBuffer
{
    public class MemoryUnit
    {
        private int m_referencedCount;
        private bool m_isDirty;
        private bool m_isRead;
        private long m_DataSetID;
        private uint m_BlockIndex;
        
        public byte[] DataSpace;

        public MemoryUnit(int Size)
        {
            m_referencedCount = 0;
            m_isDirty = false;
            m_isRead = false;
            m_DataSetID = -1;
            m_BlockIndex = 0;
            DataSpace = new byte[Size];
        }

        public long DataSetID
        {
            get{return m_DataSetID;}
        }
        public uint BlockIndex
        {
            get { return m_BlockIndex; }
        }

        public bool IsRead
        {
            get { return m_isRead; }
            set { m_isRead = value; }
        }
        public bool IsDirty
        {
            get { return m_isDirty; }
        }
        public void SetDirty()
        {
            m_isDirty = true;
        }

        public void Initialize(long dataSetID, uint blockIndex)
        {
            m_DataSetID = dataSetID;
            m_BlockIndex = blockIndex;
            m_isDirty = false;
            m_isRead = false;
        }

        public void AddPressure()
        {
            Interlocked.Add(ref m_referencedCount, 1);
        }
        public void RemovePressure()
        {
            Interlocked.Add(ref m_referencedCount, -1);
        }

        /// <summary>
        /// Determines if an object is currently being referenced
        /// </summary>
        public bool IsReferenced
        {
            get { return m_referencedCount != 0; }
        }
    
    }
}
