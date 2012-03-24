using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Specialized
{
    public interface ITreeLeafNodeMethods<TKey>
    {

        void Initialize(BinaryStream stream, int blockSize, int valueSize, MethodCall writeValue, MethodCall readValue, AllocateNewNode allocateNewNode, NodeSplitRequired<TKey> nodeSplit);

        void SetCurrentNode(uint nodeIndex, bool isForWriting);

        bool Insert(TKey key);
        
        bool GetValue(TKey key);

        uint CreateEmptyNode();

        void PrepareForTableScan(TKey firstKey, TKey lastKey);

        bool GetNextKeyTableScan(out TKey key);

        void CloseTableScan();

    }
}
