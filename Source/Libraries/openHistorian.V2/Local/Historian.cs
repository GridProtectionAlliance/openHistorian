//******************************************************************************************************
//  Historian.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  9/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Local
{
    public partial class Historian : IHistorian
    {
        SortedTree256 m_tree;
        BinaryStream m_stream;

        public Historian()
        {
            m_stream = new BinaryStream();
            m_tree = new SortedTree256(m_stream, 4096);
        }

        public IHistorianReadWrite ConnectToDatabase(string databaseName)
        {
            return new HistorianReadWrite(this);
        }

        public bool IsCommitted(long transactionId)
        {
            return true;
        }

        public bool IsDiskCommitted(long transactionId)
        {
            return true;
        }

        public IManageHistorian Manage()
        {
            throw new NotImplementedException();
        }
    }
}
