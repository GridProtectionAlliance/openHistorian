using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic
{

    public delegate void MethodCall();

    public delegate uint AllocateNewNode();

    public delegate void NodeSplitRequired<TKey>(byte level, uint currentIndex, TKey middleKey, uint newIndex);

}
