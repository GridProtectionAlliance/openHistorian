using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.PointQueue
{
    class LiveDataProcessor
    {
        public Dictionary<int, IndividualQueueBuffer> BufferQueue = new Dictionary<int, IndividualQueueBuffer>();
        public void AddToQueue(Points pt)
        {
            IndividualQueueBuffer pointQueue;
            if (!BufferQueue.TryGetValue(pt.PointID, out pointQueue))
            {
                pointQueue = new IndividualQueueBuffer();
                BufferQueue.Add(pt.PointID, pointQueue);
            }
            pointQueue.Queue.Add(pt);
        }

     
    }
}
