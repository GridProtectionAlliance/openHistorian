using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Specialized
{
    public interface ITreeInternalNodeMethods<TKey>
    {
        void Initialize(BinaryStream stream, byte nodeLevel, int blockSize, AllocateNewNode allocateNewNode, NodeSplitRequired<TKey> nodeSplit);

        void Insert(uint nodeIndex, byte nodeLevel, TKey key, uint childNodeIndex);

        uint GetNodeIndex(uint nodeIndex, byte nodeLevel, TKey key);

        uint CreateEmptyNode(byte level, uint childNodeBefore, TKey key, uint childNodeAfter);
    }
}
