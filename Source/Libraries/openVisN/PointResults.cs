//******************************************************************************************************
//  PointResults.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN
{
    public class PointResults
    {
        bool m_calculated;
        List<ulong> m_dateTime = new List<ulong>();
        List<ulong> m_values = new List<ulong>();

        MetadataBase m_metaData;
        public PointResults(MetadataBase metaData)
        {
            m_metaData = metaData;
        }

        public int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public KeyValuePair<ulong, double> GetDouble(int index)
        {
            return new KeyValuePair<ulong, double>(m_dateTime[index], m_metaData.ToDouble(m_values[index]));
        }

        public void AddPoint(ulong time, ulong value)
        {
            m_dateTime.Add(time);
            m_values.Add(value);
        }

        public void Calculate()
        {
            if (!m_calculated)
            {
                m_calculated = true;
            }
        }
    }

}
