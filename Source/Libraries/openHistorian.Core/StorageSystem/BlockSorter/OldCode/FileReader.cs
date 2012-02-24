using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Historian.StorageSystem.BlockSorter
{
    public class FileReader
    {
        Stream m_file;
        BinaryReader m_reader;
        BinaryWriter m_writer;
        public FileReader(Stream file)
        {
            m_file = file;
            m_reader = new BinaryReader(file);
            m_writer = new BinaryWriter(file);

        }
       
        public void AddData(IBlockKey8 key, byte[] data)
        {
        }
        public bool Exists(IBlockKey8 key)
        {
            return true;
        }
        public byte[] GetData(IBlockKey8 key)
        {
            return null;
        }
        public long CountEntries()
        {
            return -1;
        }
    }
}
