using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.BlockSorter
{
    public interface IBlockKey8
    {
        /// <summary>
        /// Returns the 64 bit key code associated with this key.
        /// </summary>
        /// <param name="key1">the high order key that is sorted first</param>
        void GetKey(out long key1);
    }
}
