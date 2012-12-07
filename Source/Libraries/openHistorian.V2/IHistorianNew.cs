////******************************************************************************************************
////  IHistorian.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  9/14/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using openHistorian.IO;
//using openHistorian.Local;

//namespace openHistorian.New
//{

//    public interface IPointStream
//    {
//        bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2);
//        void Cancel();
//    }

//    public interface IHistorianServer : IDisposable
//    {
//        IHistorian ConnectToDatabase(string databaseName);
//        bool Contains(string databaseName);
//        void Add(string databaseName, IDatabaseConfig config = null);
//        void Drop(string databaseName, float waitTimeSeconds = 0);
//        void Shutdown(float waitTimeSeconds);
//    }

//    public interface IHistorian : IDisposable
//    {
//        bool IsOnline { get; }

//        IHistorianWriter OpenWriter();
//        IHistorianReader OpenReader();
//        IDatabaseConfig GetConfig();
//        void SetConfig(IDatabaseConfig config);
//        void TakeOffline(float waitTimeSeconds = 0);
//        void BringOnline();
//        void Shutdown(float waitTimeSeconds);

//    }

//    public interface IHistorianReader : IDisposable
//    {
//        long LastCommittedTransactionId { get; }
//        long LastDiskCommittedTransactionId { get; }
//        long CurrentTransactionId { get; }

//        IPointStream Read(ulong key);
//        IPointStream Read(ulong startKey, ulong endKey);
//        IPointStream Read(ulong startKey, ulong endKey, IEnumerable<ulong> points);

//        void Disconnect();
//    }

//    public interface IHistorianWriter : IDisposable
//    {
//        void Write(IPointStream points);
//        void Write(ulong key1, ulong key2, ulong value1, ulong value2);
//        long WriteBulk(IPointStream points);

//        bool IsCommitted(long transactionId);
//        bool IsDiskCommitted(long transactionId);

//        bool WaitForCommitted(long transactionId);
//        bool WaitForDiskCommitted(long transactionId);

//        void Commit();
//        void CommitToDisk();

//        long LastCommittedTransactionId { get; }
//        long LastDiskCommittedTransactionId { get; }
//        long CurrentTransactionId { get; }

//        void Disconnect();
//    }

//    public struct WriterOptions
//    {
//        public bool IsValid { get; private set; }
//        /// <summary>
//        /// Determines if writes to this historian will only 
//        /// occur in memory and never commited to the disk.
//        /// </summary>
//        public bool IsInMemoryOnly { get; private set; }
//        /// <summary>
//        /// Determines the number of seconds to allow the realtime queue 
//        /// to buffer points before forcing them to be added to the historian.
//        /// </summary>
//        /// <remarks>
//        /// This interval controls the maximum lag time between adding a point 
//        /// to the historian and it becomes queryable by the user.
//        /// 
//        /// Setting to null allows the historian to auto configure this parameter.
//        /// </remarks>
//        public float? MemoryCommitInterval { get; private set; }
//        /// <summary>
//        /// Determines the number of seconds to allow memory buffers to manage newly 
//        /// inserted points before forcing them to be committed to the disk. 
//        /// </summary>
//        /// <remarks>
//        /// Setting this parameter too low can greatly increase the Disk IO. Only one disk
//        /// commit can happen at a time per instance. Therefore, if the Disk IO becomes the bottleneck
//        /// it will cause the historian to artificially increase this value.
//        /// 
//        /// Setting this parameter too large can increase the amount of data that will be lost
//        /// in an unexpected crash and increase the amount of time it takes to shutdown the historian 
//        /// as all these points must be committed to the disk before it will be shutdown.
//        /// 
//        /// Setting to null allows the historian to auto configure this parameter.
//        /// </remarks>
//        public float? DiskCommitInterval { get; private set; }
//        /// <summary>
//        /// Gives the historian an idea of the volume of data that it will be dealing with.
//        /// </summary>
//        /// <remarks>
//        /// This value is used to estimate the volume of streaming data and how large
//        /// archive files should be when initialized and autogrown.
//        /// 
//        /// Setting to null allows the historian to auto configure this parameter.
//        /// </remarks>
//        public float? OptimalPointsPerSecond { get; private set; }

//        public static WriterOptions IsMemoryOnly(float commitInterval = 0.25f, float optimalPointsPerSecond = 8*30*10)
//        {
//            WriterOptions options = default(WriterOptions);
//            options.IsValid = true;
//            options.IsInMemoryOnly = true;
//            options.MemoryCommitInterval = commitInterval;
//            options.DiskCommitInterval = null;
//            options.OptimalPointsPerSecond = optimalPointsPerSecond;
//            return options;
//        }

//        public static WriterOptions IsFileBased(float memoryCommitInterval = 0.25f, float diskCommitInterval = 10f, float optimalPointsPerSecond = 8*30*10)
//        {
//            WriterOptions options = default(WriterOptions);
//            options.IsValid = true;
//            options.IsInMemoryOnly = false;
//            options.MemoryCommitInterval = memoryCommitInterval;
//            options.DiskCommitInterval = diskCommitInterval;
//            options.OptimalPointsPerSecond = optimalPointsPerSecond;
//            return options;
//        }
//    }

//    public interface IDatabaseConfig
//    {
//        WriterOptions? Writer { get; set; }

//        /// <summary>
//        /// Gets if this instance is online.
//        /// </summary>
//        bool IsOnline { get; }

//        /// <summary>
//        /// Gets all of the paths that are known by this historian.
//        /// A path can be a file name or a folder.
//        /// </summary>
//        IPathList Paths { get; }
//    }

//    public interface IPathList
//    {
//        IEnumerable<string> GetPaths();
//        IEnumerable<string> GetSavePaths();
//        void AddPath(string path, bool allowWritingToPath);
//        void DropPath(string path, float waitTimeSeconds);
//        void DropSavePath(string path, bool terminateActiveFiles);
//    }



//}
