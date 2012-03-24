using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Specialized
{
    public interface IKeyType<T>
    {
        ITreeLeafNodeMethods<T> GetLeafNodeMethods();
        ITreeInternalNodeMethods<T> GetInternalNodeMethods();
    }
}
