////******************************************************************************************************
////  StreamInsertHelper.cs - Gbtc
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
////  3/3/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF;

//namespace openHistorian.Collections
//{
//    public class StreamInsertHelper
//    {
//        Stream256Base m_stream;
//        ulong m_firstKey;
//        ulong m_lastKey;
//        bool m_skipNextRead;

//        public StreamInsertHelper(Stream256Base stream, ulong firstKey, ulong lastKey)
//        {
//            m_stream = stream;
//            m_skipNextRead = false;
//            m_stream = stream;
//            m_firstKey = firstKey;
//            m_lastKey = lastKey;
//        }

//        public bool IsValid
//        {
//            get
//            {
//                return m_stream.IsValid;
//            }
//        }

//        public ulong Key1
//        {
//            get
//            {
//                return m_stream.Key1;
//            }
//        }

//        public ulong Key2
//        {
//            get
//            {
//                return m_stream.Key2;
//            }
//        }

//        public ulong Value1
//        {
//            get
//            {
//                return m_stream.Value1;
//            }
//        }

//        public ulong Value2
//        {
//            get
//            {
//                return m_stream.Value2;
//            }
//        }

//        public ulong LastKey
//        {
//            get
//            {
//                return m_lastKey;
//            }
//        }

//        public ulong FirstKey
//        {
//            get
//            {
//                return m_firstKey;
//            }
//        }

//        public bool Read()
//        {
//            if (m_skipNextRead)
//            {
//                m_skipNextRead = false;
//                return true;
//            }
//            if (m_stream.Read())
//            {
//                m_lastKey = Math.Max(m_lastKey, Key1);
//                m_firstKey = Math.Min(m_firstKey, Key1);
//                return true;
//            }
//            return false;
//        }

//        public void UnRead()
//        {
//            if (m_skipNextRead)
//                throw new Exception("Only 1 value can be unread");
//            m_skipNextRead = true;
//        }


//    }
//}

