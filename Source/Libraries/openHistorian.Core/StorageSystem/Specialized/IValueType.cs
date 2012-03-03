using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Specialized
{
    public interface IValueType
    {
        int SizeOf { get; }
        void LoadValue(BinaryStream stream);
        void SaveValue(BinaryStream stream);
    }
}
