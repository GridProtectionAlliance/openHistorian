using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Collections.KeyValue
{
    /// <summary>
    /// Represents an empty tree scanner. 
    /// </summary>
    public class NullTreeScanner256 : ITreeScanner256
    {
        static NullTreeScanner256()
        {
            Instance = new NullTreeScanner256();
        }

        public static ITreeScanner256 Instance { get; private set; }

        public bool GetNextKey(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
        {
            key1 = 0;
            key2 = 0;
            value1 = 0;
            value2 = 0;
            return false;
        }
        public void SeekToKey(ulong key1, ulong key2)
        {
        }
    }
}
