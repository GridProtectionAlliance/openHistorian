using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Historian.StorageSystem;

namespace Historian.PluginDataFeatures
{
    public interface IPluginDataFeature
    {
        Guid AssemblyID { get; }
        void InitializeStorageSystem(IStorageSystem storageSystem, long FeatureStartPageIndex);
    }
}
