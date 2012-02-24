//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Historian.PointQueue;
//using Historian;
//using Historian.TimeSeriesDataStructure;


//namespace Historian
//{
//    class Historian2
//    {
//        public LiveDataProcessor LiveDataProcessor = new LiveDataProcessor();
//        public ArciveDataProcessor ArchiveDataProcessor = new ArciveDataProcessor();
//        public PointLibrary.PointLibrary GlobalPointLibrary = new PointLibrary.PointLibrary();

//        public void QueuePointToArchive(Points pt)
//        {
//            LiveDataProcessor.AddToQueue(pt);
//        }
//        public void ArchiveLiveData(out int ArchivedSize)
//        {
//            BuildPointArchiveSolution data = new BuildPointArchiveSolution();
//            data.CompactLiveData(LiveDataProcessor);
//            ArchivedSize = data.TotalSize;
//        }
        
//    }
//}
