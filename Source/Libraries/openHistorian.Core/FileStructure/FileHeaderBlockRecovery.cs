//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.FileStructure
//{
//    /// <summary>
//    /// Provides a way to recover from a corrupt file allocation table.
//    /// </summary>
//    class FileAllocationTableRecovery : FileAllocationTable
//    {

//        /// <summary>
//        /// This flag is marked as true if while opening the file, the primary file allocation table was corrupt and a backup had to be used.
//        /// </summary>
//        bool m_isTableCompromised;

//        /// <summary>
//        /// Determines if the file is currently opened for exclusive editing.
//        /// If this file was able to be opened for write access, 
//        /// this parameter will suggest that the file was not closed correctly.
//        /// </summary>
//        /// <remarks>This flag will usually be set by a utility that is doing some kind of defragment on the file.
//        /// The file will be locked exclusively by the process.</remarks>
//        bool m_isOpenedForExclusiveEditing;


//        /// <summary>
//        /// This will open an existing archive header that is read only.
//        /// </summary>
//        /// <returns></returns>
//        public static FileAllocationTable OpenHeader(DiskIo diskIo)
//        {
//            Exception openException;
//            byte[] blockBytes = new byte[ArchiveConstants.BlockSize];

//            FileAllocationTable fat0;
//            FileAllocationTable fat1;
//            FileAllocationTable fat2;
//            FileAllocationTable fat3;
//            FileAllocationTable fat4;
//            FileAllocationTable fat5;
//            FileAllocationTable fat6;
//            FileAllocationTable fat7;
//            FileAllocationTable fat8;
//            FileAllocationTable fat9;
//            FileAllocationTable latestHeader;

//            //Attempt to open and return the first header of the file.
//            fat0 = TryOpenFileAllocationTable(0, diskIo, blockBytes, out openException);
//            if (fat0 != null)
//            {
//                if (!fat0.IsReadOnly)
//                    throw new Exception();
//                fat0.m_isTableCompromised = false;
//                return fat0;
//            }
//            //Attempt to open and return the second header of the file if the first is corrupt.
//            fat1 = TryOpenFileAllocationTable(1, diskIo, blockBytes, out openException);
//            if (fat1 != null)
//            {
//                if (!fat1.IsReadOnly)
//                    throw new Exception();
//                fat1.m_isTableCompromised = true;
//                return fat1;
//            }

//            //Read the next 8 headers of the file. All must not be corrupt. Return the header with the largest FileChangeSequenceNumber.
//            //If any of the pages are corrupt, a seperate action must be taken because there is a chance that the user must revert to
//            //an older version of the file, which will result in losing data.  
//            //If all pages are corrupt, the file is basically unrecoverable unless the user can guess the missing inode table data.
//            fat2 = TryOpenFileAllocationTable(2, diskIo, blockBytes, out openException);
//            if (fat2 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat3 = TryOpenFileAllocationTable(3, diskIo, blockBytes, out openException);
//            if (fat3 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat4 = TryOpenFileAllocationTable(4, diskIo, blockBytes, out openException);
//            if (fat4 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat5 = TryOpenFileAllocationTable(5, diskIo, blockBytes, out openException);
//            if (fat5 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat6 = TryOpenFileAllocationTable(6, diskIo, blockBytes, out openException);
//            if (fat6 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat7 = TryOpenFileAllocationTable(7, diskIo, blockBytes, out openException);
//            if (fat7 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat8 = TryOpenFileAllocationTable(8, diskIo, blockBytes, out openException);
//            if (fat8 == null)
//                throw new Exception("File header is corrupt", openException);

//            fat9 = TryOpenFileAllocationTable(9, diskIo, blockBytes, out openException);
//            if (fat9 == null)
//                throw new Exception("File header is corrupt", openException);

//            latestHeader = GetLatestFileAllocationTable(fat2, fat3);
//            latestHeader = GetLatestFileAllocationTable(latestHeader, fat4);
//            latestHeader = GetLatestFileAllocationTable(latestHeader, fat5);
//            latestHeader = GetLatestFileAllocationTable(latestHeader, fat6);
//            latestHeader = GetLatestFileAllocationTable(latestHeader, fat7);
//            latestHeader = GetLatestFileAllocationTable(latestHeader, fat8);
//            latestHeader = GetLatestFileAllocationTable(latestHeader, fat9);

//            latestHeader.m_isTableCompromised = true;

//            if (!latestHeader.IsReadOnly)
//                throw new Exception();
//            return latestHeader;
//        }
//        /// <summary>
//        /// Makes an attempt to open a file allocation table from the data buffer. 
//        /// </summary>
//        /// <param name="blockIndex"></param>
//        /// <param name="diskIo"></param>
//        /// <param name="tempBuffer"></param>
//        /// <param name="error">an output parameter for the error if one was encountered.</param>
//        /// <returns>null if there was an error and puts the exception in the error parameter.</returns>
//        static FileAllocationTable TryOpenFileAllocationTable(int blockIndex, DiskIo diskIo, byte[] tempBuffer, out Exception error)
//        {
//            error = null;
//            try
//            {
//                diskIo.Read(blockIndex, BlockType.FileAllocationTable, 0, 0, int.MaxValue, tempBuffer);
//                return new FileAllocationTable(tempBuffer);
//            }
//            catch (Exception ex)
//            {
//                error = ex;
//            }
//            return null;
//        }

//        /// <summary>
//        /// Returns the file allocation table that has the most recent snapshot sequence number
//        /// </summary>
//        /// <param name="fat1"></param>
//        /// <param name="fat2"></param>
//        /// <returns></returns>
//        static FileAllocationTable GetLatestFileAllocationTable(FileAllocationTable fat1, FileAllocationTable fat2)
//        {
//            if (fat1 == null)
//                return fat2;
//            if (fat2 == null)
//                return fat1;
//            if (fat1.SnapshotSequenceNumber > fat2.SnapshotSequenceNumber)
//                return fat1;
//            return fat2;
//        }

//        /// <summary>
//        /// This will create a new File Allocation Table that is editable and writes the data to the File System.
//        /// </summary>
//        /// <returns></returns>
//        public static FileAllocationTable CreateFileAllocationTable(DiskIo file)
//        {
//            FileAllocationTable table = new FileAllocationTable(file);
//            if (table.IsReadOnly)
//                throw new Exception();
//            return table;
//        }
//    }
//}

