using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Collections.KeyValue
{
    //ToDo: Implement a union scanner.
    public class UnionScanner256 : ITreeScanner256
    {
        IList<ITreeScanner256> m_trees;

        public UnionScanner256(IList<ITreeScanner256> trees)
        {
            m_trees = trees;
        }

        public bool GetNextKey(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
        {
            throw new NotImplementedException();
        }

        public void SeekToKey(ulong key1, ulong key2)
        {
            throw new NotImplementedException();
        }
    }
}
