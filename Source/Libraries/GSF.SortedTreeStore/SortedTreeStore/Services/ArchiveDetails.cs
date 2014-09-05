using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    public class ArchiveDetails
    {
        public Guid Id;
        public string FileName;
        public bool IsEmpty;
        public long FileSize;
        public string FirstKey;
        public string LastKey;

        private ArchiveDetails()
        {

        }

        public static ArchiveDetails Create<TKey, TValue>(ArchiveTableSummary<TKey, TValue> table)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return new ArchiveDetails()
                {
                    Id = table.FileId,
                    FileName = table.SortedTreeTable.BaseFile.FilePath,
                    IsEmpty = table.IsEmpty,
                    FileSize = table.SortedTreeTable.BaseFile.ArchiveSize,
                    FirstKey = table.FirstKey.ToString(),
                    LastKey = table.LastKey.ToString()
                };
        }
    }
}
