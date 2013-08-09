////******************************************************************************************************
////  TreeScannerEnsureSequential.cs - Gbtc
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
////  2/10/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;

//namespace openHistorian.Collections
//{
//    public class TreeScannerEnsureSequential : TreeScanner256Base
//    {
//        TreeScanner256Base m_baseScanner;
//        bool m_lastValid;
//        ulong m_lastKey1;
//        ulong m_lastKey2;

//        public TreeScannerEnsureSequential(TreeScanner256Base baseScanner)
//        {
//            m_baseScanner = baseScanner;
//            m_lastValid = false;
//        }

//        public override bool Read()
//        {
//            IsValid = m_baseScanner.Read();
//            Key1 = m_baseScanner.Key1;
//            Key2 = m_baseScanner.Key2;
//            Value1 = m_baseScanner.Value1;
//            Value2 = m_baseScanner.Value2;

//            if (IsValid && m_lastValid)
//            {
//                if (m_lastKey1 > Key1 || (m_lastKey1 == Key1 && m_lastKey2 >= Key2))
//                    throw new Exception("Archive file is corrupt.");
//            }

//            m_lastValid = IsValid;
//            m_lastKey1 = Key1;
//            m_lastKey2 = Key2;
//            return IsValid;
//        }

//        public override void SeekToKey(ulong key1, ulong key2)
//        {
//            m_lastValid = false;
//            m_baseScanner.SeekToKey(key1, key2);
//        }
//    }
//}

