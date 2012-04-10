//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Historian.StorageSystem;

//namespace Historian
//{
//    class MemoryManager
//    {
//        //private byte[] buffer; 


//        /// <summary>
//        /// Manages the large buffers that provide buffered IO.
//        /// </summary>
//        /// <param name="DesiredSize">The number of bytes to allocate for the buffer</param>
//        /// <remarks>To speed up development, this code currently does not manage memory, it just allocates new buffers when requested.</remarks>
//        public MemoryManager(long DesiredSize)
//        {
//            //buffer = new byte[DesiredSize];
//        }

//        /// <summary>
//        /// Maintains a list of all of the open archive files.  
//        /// </summary>
//        public List<Historian.ArchiveFile.ArchiveFile> Files = new List<Historian.ArchiveFile.ArchiveFile>();

//        /// <summary>
//        /// Returns the following data from disk or from cache.
//        /// </summary>
//        /// <param name="PageID"></param>
//        /// <param name="Data"></param>
//        /// <param name="Start"></param>
//        /// <param name="NumberOfPages"></param>
//        public void GetPage(long PageID, out byte[] Data, out int Start, int NumberOfPages)
//        {
//            int FileID = (int)(PageID & 0x7fffff000000)>>32;
           
//            GetFreePage(out Data, out Start, NumberOfPages);

//            //Files[FileID].ReadData(PageID, Data, Start, NumberOfPages);
//        }
        
//        /// <summary>
//        /// Finds the next available free page and allocates this page for use.
//        /// </summary>
//        /// <param name="Data"></param>
//        /// <param name="Start"></param>
//        /// <param name="NumberOfPages"></param>
//        /// <remarks>This function currently allocates a new block.  In the future this functionality will be tweaked.</remarks>
//        private void GetFreePage(out byte[] Data, out int Start, int NumberOfPages)
//        {
//            Data = new byte[65536 * NumberOfPages];
//            Start = 0;
//        }

//    }
//}
