using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Specialized
{
    public interface ITreeLeafNodeMethods<TKey>
    {
        void Initialize(BinaryStream stream, int blockSize, int valueSize, MethodCall writeValue, MethodCall readValue, AllocateNewNode allocateNewNode, NodeSplitRequired<TKey> nodeSplit);
                
        bool Insert(uint nodeIndex, TKey key);
        
        bool GetValue(uint nodeIndex, TKey key);

        uint CreateEmptyNode();

    }
}
