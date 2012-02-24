using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.PointQueue
{
    class IndividualQueueBuffer
    {
        public List<Points> Queue = new List<Points>(10000);
    }
}
