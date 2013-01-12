//******************************************************************************************************
//  HistorianDataReaderBase.cs - Gbtc
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Linq;

namespace openHistorian
{
    public abstract class HistorianDataReaderBase : IHistorianDataReader
    {
        public virtual IPointStream Read(ulong key1)
        {
            return Read(KeyParserPrimary.CreateFromRange(key1, key1), KeyParserSecondary.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        public virtual IPointStream Read(ulong startKey1, ulong endKey1)
        {
            return Read(KeyParserPrimary.CreateFromRange(startKey1, endKey1), KeyParserSecondary.CreateAllKeysValid(), DataReaderOptions.Default);
        }

        public virtual IPointStream Read(ulong startKey1, ulong endKey1, IEnumerable<ulong> listOfKey2)
        {
            return Read(KeyParserPrimary.CreateFromRange(startKey1, endKey1), KeyParserSecondary.CreateFromList(listOfKey2.ToList()), DataReaderOptions.Default);
        }

        public virtual IPointStream Read(KeyParserPrimary key1, IEnumerable<ulong> listOfKey2)
        {
            return Read(key1, KeyParserSecondary.CreateFromList(listOfKey2.ToList()), DataReaderOptions.Default);
        }

        public abstract IPointStream Read(KeyParserPrimary key1, KeyParserSecondary key2, DataReaderOptions readerOptions);

        public abstract void Close();
        public abstract void Dispose();
    }
}
