using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Unmanaged.Generic
{
    public interface IValueType<T>
    {
        int SizeOf { get; }
        void LoadValue(BinaryStream stream);
        void SaveValue(BinaryStream stream);
       
        /// <summary>
        /// Return 0 if equal. 
        /// Return -1 if instance is before stream
        /// Return 1 if instance is after stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        int CompareToStream(BinaryStream stream);
        int CompareTo(T key);
    }
}
