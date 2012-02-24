using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.BlockSorter
{
    public enum InsertResults
    {
        InsertedOK,
        NodeIsFullError,
        DuplicateKeyError
    }
}
