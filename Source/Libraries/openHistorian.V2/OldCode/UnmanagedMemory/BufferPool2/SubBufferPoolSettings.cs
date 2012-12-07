////******************************************************************************************************
////  BufferPoolSettings.cs - Gbtc
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
////  6/8/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;

//namespace openHistorian.UnmanagedMemory
//{
//    /// <summary>
//    /// Maintains the core settings for the buffer pool and the methods for how they are calculated.
//    /// </summary>
//    public partial class SubBufferPool
//    {

//        #region [ Members ]

//        public long LevelNone;
//        public long LevelLow;
//        public long LevelNormal;
//        public long LevelHigh;
//        public long LevelVeryHigh;
//        public long LevelCritical;

//        /// <summary>
//        /// The maximum amount of RAM that this buffer pool is configured to support
//        /// Attempting to allocate more than this will cause an out of memory exception
//        /// </summary>
//        long m_maximumBufferSize;


//        #endregion

//        #region [ Constructors ]


//        public void SubBufferPoolSettings(int pageSize)
//        {
           
//        }

//        #endregion

//        #region [ Properties ]
      
//        #endregion

//        #region [ Methods ]

//        public int GetCollectionBasedOnSize(long size)
//        {
//            if (size < LevelNone)
//            {
//                return 0;
//            }
//            else if (size < LevelLow)
//            {
//                return 1;
//            }
//            else if (size < LevelNormal)
//            {
//                return 2;
//            }
//            else if (size < LevelHigh)
//            {
//                return 3;
//            }
//            else if (size < LevelVeryHigh)
//            {
//                return 4;
//            }
//            else
//            {
//                return 5;
//            }
//        }

//        #endregion

//        #region [ Static ]

//        #endregion

//    }
//}
