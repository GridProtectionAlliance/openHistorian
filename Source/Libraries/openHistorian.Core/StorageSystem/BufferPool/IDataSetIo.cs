using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.BufferPool
{
    public unsafe interface IDataSetIo
    {
        void ReadBlock(uint blockIndex, byte* array, int length);
        void WriteBlock(uint blockIndex, byte* array, int length);
    }
}
