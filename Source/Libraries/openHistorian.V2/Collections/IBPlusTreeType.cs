using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections
{
    public interface IBPlusTreeType<T>
    {
        int SizeOf { get; }
        void LoadValue(IBinaryStream stream);
        void SaveValue(IBinaryStream stream);
       
        /// <summary>
        /// Return 0 if equal. 
        /// Return -1 if instance is before stream
        /// Return 1 if instance is after stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        int CompareToStream(IBinaryStream stream);
        int CompareTo(T key);
    }
}
