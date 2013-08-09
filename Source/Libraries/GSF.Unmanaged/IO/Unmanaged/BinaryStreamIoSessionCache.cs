////******************************************************************************************************
////  BinaryStreamIoSessionCache.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
////  2/22/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;

//namespace GSF.IO.Unmanaged
//{
//    public class BinaryStreamIoSessionCache : IBinaryStreamIoSession
//    {
//        public static long SavedLookups = 0;
//        public static long Lookups = 0;

//        IntPtr m_firstPointer;
//        long m_firstPosition;
//        int m_length;
//        bool m_supportsWriting;
//        public override event EventHandler<IoSessionStatusChangedEventArgs> BaseStatusChanged
//        {
//            add
//            {
//                m_baseStream.BaseStatusChanged += value;
//            }
//            remove
//            {
//                m_baseStream.BaseStatusChanged -= value;
//            }
//        }

//        IBinaryStreamIoSession m_baseStream;
//        public BinaryStreamIoSessionCache(IBinaryStreamIoSession binaryStreamSession)
//        {
//            m_baseStream = binaryStreamSession;
//        }

//        public override void Dispose()
//        {
//            m_baseStream.Dispose();
//            IsDisposed = true;
//        }

//        public override void GetBlock(BlockArguments args)
//        {
//            if (args.position >= m_firstPosition & args.position < m_firstPosition + m_length & (m_supportsWriting | !args.isWriting) & !IsDisposed)
//            {
//                args.firstPointer = m_firstPointer;
//                args.firstPosition = m_firstPosition;
//                args.length = m_length;
//                args.supportsWriting = m_supportsWriting;
//                SavedLookups++;
//                return;
//            }
//            m_baseStream.GetBlock(args);
//            m_firstPointer = args.firstPointer;
//            m_firstPosition = args.firstPosition;
//            m_length = args.length;
//            m_supportsWriting = args.supportsWriting;
//            Lookups++;
//        }

//        public override void Clear()
//        {
//            m_length = 0;
//            m_baseStream.Clear();
//        }
//    }
//}

