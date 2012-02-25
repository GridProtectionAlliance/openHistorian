using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Importer
{
    class TimeBucket
    {
        /// <summary>
        /// Adds the following data to the internal data structure.
        /// By the time it gets here, the data must be pre-sorted
        /// </summary>
        /// <param name="block"></param>
        public void AddToTree(BinaryStream block)
        {
            long start = block.Position;
        }

        //bool MergeWithTree(BinaryStream stream, BPlusTree tree)
        //{
        //    DateTime date = stream.ReadDateTime();
        //    DateTime origDate = date;
        //    TimeKey key = new TimeKey(date);

        //    while (origDate == date)
        //    {
        //        byte[] existing = tree.GetData(key);
        //        stream.Write(stream.ReadInt32());
        //        stream.Write(stream.ReadDouble());
        //    }
        //}

    }
}
