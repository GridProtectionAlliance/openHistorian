using System;
using System.Collections.Generic;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Collections.BPlusTreeTypes;
using openHistorian.V2.Collections.Specialized;
using openHistorian.V2.FileSystem;
using openHistorian.V2.Providers;

namespace openHistorian.V2
{
    public class Archive
    {
        VirtualFileSystem m_fileSystem;
        TransactionalEdit m_currentTransaction;
        ArchiveFileStream m_stream1;
        BinaryStream m_binaryStream1;
        BPlusTreeTSD m_tree;

        //public Archive(string file)
        //{
        //    if (File.Exists(file))
        //        OpenFile(file);
        //    else
        //        CreateFile(file);
        //}
        public Archive()
        {
            CreateFileInMemory();
        }

        //void OpenFile(string file)
        //{
        //    m_fileSystem = VirtualFileSystem.OpenArchive(file, false);
        //    m_currentTransaction = m_fileSystem.BeginEdit();
        //    m_stream = m_currentTransaction.OpenFile(0);
        //    m_binaryStream = new BinaryStream(m_stream);
        //    m_tree = new BPlusTree<KeyType, TreeTypeIntFloat>(m_binaryStream);
        //}
        //void CreateFile(string file)
        //{
        //    m_fileSystem = VirtualFileSystem.CreateArchive(file);
        //    m_currentTransaction = m_fileSystem.BeginEdit();
        //    m_stream = m_currentTransaction.CreateFile(new Guid("{7bfa9083-701e-4596-8273-8680a739271d}"), 1);
        //    m_binaryStream = new BinaryStream(m_stream);
        //    m_tree = new BPlusTree<KeyType, TreeTypeIntFloat>(m_binaryStream, ArchiveConstants.DataBlockDataLength);
        //}
        void CreateFileInMemory()
        {
            m_fileSystem = VirtualFileSystem.CreateInMemoryArchive();
            m_currentTransaction = m_fileSystem.BeginEdit();
            m_stream1 = m_currentTransaction.CreateFile(new Guid("{7bfa9083-701e-4596-8273-8680a739271c}"), 1);
            m_binaryStream1 = new BinaryStream(m_stream1);
            m_tree = new BPlusTreeTSD(m_binaryStream1, ArchiveConstants.DataBlockDataLength);
        }

        public IMetaDataProvider[] MetaData;
        public ITimeSeriesDataProvider[] TimeSeriesData;
        
        public void AddPoint(DateTime date, long pointId, int flags, float data)
        {
            DateTimeLong key = default(DateTimeLong);
            key.Time = date;
            key.Key = pointId;

            IntegerFloat value = new IntegerFloat(flags, data);

            m_tree.Add(key, value);
        }

        public IEnumerable<Tuple<DateTime, long, int, float>> GetData(long pointId, DateTime startDate, DateTime stopDate)
        {
            DateTimeLong start = default(DateTimeLong);
            DateTimeLong end = default(DateTimeLong);
            start.Time = startDate;
            start.Key = pointId;
            end.Time = stopDate;
            end.Key = pointId;

            foreach (var kvp in m_tree.GetRange(start, end))
            {
                DateTimeLong key = kvp.Key;
                IntegerFloat value = kvp.Value;
                if (key.Key == pointId)
                    yield return new Tuple<DateTime, long, int, float>(key.Time, key.Key, value.Value1, value.Value2);
            }

        }
        public IEnumerable<Tuple<DateTime, long, int, float>> GetData(DateTime startDate, DateTime stopDate)
        {
            DateTimeLong start = default(DateTimeLong);
            DateTimeLong end = default(DateTimeLong);
            start.Time = startDate;
            start.Key = long.MinValue;
            end.Time = stopDate;
            end.Key = long.MaxValue;

            foreach (var kvp in m_tree.GetRange(start, end))
            {
                DateTimeLong key = kvp.Key;
                IntegerFloat value = kvp.Value;
                yield return new Tuple<DateTime, long, int, float>(key.Time, key.Key, value.Value1, value.Value2);
            }
        }

        public void Close()
        {
            //m_tree.Save();
            m_stream1.Flush();
            m_currentTransaction.Commit();
            m_fileSystem.Dispose();
        }

    }
}
