using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.StorageSystem.BufferPool
{
    public unsafe interface IDataSetIo
    {
        void ReadBlock(uint blockIndex, byte* array, int length);
        void WriteBlock(uint blockIndex, byte* array, int length);
    }
}
