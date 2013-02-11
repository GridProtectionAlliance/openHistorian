//******************************************************************************************************
//  TreeScannerEnsureSequential.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Collections.KeyValue
{
    public class TreeScannerEnsureSequential : ITreeScanner256
    {
        ITreeScanner256 m_baseScanner;
        bool m_lastValid;
        ulong m_lastKey1;
        ulong m_lastKey2;

        public TreeScannerEnsureSequential(ITreeScanner256 baseScanner)
        {
            m_baseScanner = baseScanner;
            m_lastValid = false;
        }

        public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
        {
            bool success = m_baseScanner.Read(out key1, out key2, out value1, out value2);
            if (success && m_lastValid)
            {
                if (m_lastKey1 > key1 || (m_lastKey1 == key1 && m_lastKey2 >= key2))
                    throw new Exception("Archive file is corrupt.");
            }
            m_lastValid = success;
            m_lastKey1 = key1;
            m_lastKey2 = key2;
            return success;
        }

        public void SeekToKey(ulong key1, ulong key2)
        {
            m_lastValid = false;
            m_baseScanner.SeekToKey(key1, key2);
        }
    }
}
