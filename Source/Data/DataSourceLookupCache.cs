//******************************************************************************************************
//  DataSourceLookupCache.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/31/2016 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace GSF.TimeSeries.Data
{
    /// <summary>
    /// Represents a lookup cache for adapter data source data.
    /// </summary>
    public class DataSourceLookupCache
    {
        internal DataSet DataSet { get; private set; }

        /// <summary>
        /// Table lookup for active measurements.
        /// </summary>
        public readonly ActiveMeasurementsTableLookup ActiveMeasurements;

        internal DataSourceLookupCache(DataSet dataSet)
        {
            DataSet = dataSet;
            dataSet.Disposed += DataSet_Disposed;

            ActiveMeasurements = new ActiveMeasurementsTableLookup(dataSet);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DataSet_Disposed(object sender, EventArgs e)
        {
            //Do Nothing. The purpose of this is so a strong reference to this class is maintained by DataSet.
            DataSet = null;
        }
    }
}