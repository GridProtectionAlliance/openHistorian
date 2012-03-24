using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Specialized
{
    public interface ITreeInternalNodeMethods<TKey>
    {
        void Initialize(BinaryStream stream, byte nodeLevel, int blockSize, AllocateNewNode allocateNewNode, NodeSplitRequired<TKey> nodeSplit);

        void SetCurrentNode(uint nodeIndex, bool isForWriting);

        void Insert(TKey key, uint childNodeIndex);

        uint GetNodeIndex(TKey key);

        uint CreateEmptyNode(byte level, uint childNodeBefore, TKey key, uint childNodeAfter);
    }
}
