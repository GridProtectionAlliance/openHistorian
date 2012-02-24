using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.PluginDataFeatures.TableOfContents
{
    public interface ITableOfContents : IPluginDataFeature
    {
        List<IPointTimeData> Points { get; }
    }
}
