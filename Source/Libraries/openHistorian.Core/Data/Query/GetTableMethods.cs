//******************************************************************************************************
//  GetTableMethods.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GSF.Snap.Services;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{
    public interface IDelinearlizedSignals
    {
        IList<string> ColumnHeaders
        {
            get;
        }

        KeyValuePair<object, IList<ISignalWithType>> ColumnGroups
        {
            get;
        }
    }

    public interface ISignalWithName : ISignalWithType
    {
        string TagName
        {
            get;
        }
    }

    public static class GetTableMethods
    {
        public static DataTable GetTable(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong start, ulong stop, IDelinearlizedSignals signals)
        {
            return null;
        }

        public static DataTable GetTable(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong start, ulong stop, IList<ISignalWithName> columns)
        {
            if (columns.Any((x) => !x.HistorianId.HasValue))
            {
                throw new Exception("All columns must be contained in the historian for this function to work.");
            }

            Dictionary<ulong, SignalDataBase> results = database.GetSignals(start, stop, columns);
            int[] columnPosition = new int[columns.Count];
            object[] rowValues = new object[columns.Count + 1];
            SignalDataBase[] signals = new SignalDataBase[columns.Count];

            DataTable table = new DataTable();
            table.Columns.Add("Time", typeof(DateTime));
            foreach (ISignalWithName signal in columns)
            {
                table.Columns.Add(signal.TagName, typeof(double));
            }

            for (int x = 0; x < columns.Count; x++)
            {
                signals[x] = results[columns[x].HistorianId.Value];
            }

            while (true)
            {
                ulong minDate = ulong.MaxValue;
                for (int x = 0; x < columns.Count; x++)
                {
                    SignalDataBase signal = signals[x];
                    if (signal.Count < columnPosition[x])
                    {
                        minDate = Math.Min(minDate, signals[x].GetDate(columnPosition[x]));
                    }
                }

                rowValues[0] = null;
                for (int x = 0; x < columns.Count; x++)
                {
                    SignalDataBase signal = signals[x];
                    if (signal.Count < columnPosition[x] && minDate == signals[x].GetDate(columnPosition[x]))
                    {
                        signals[x].GetData(columnPosition[x], out ulong date, out double value);
                        rowValues[x + 1] = value;
                        columnPosition[x]++;
                    }
                    else
                    {
                        rowValues[x + 1] = null;
                    }
                }

                if (minDate == ulong.MaxValue && rowValues.All((x) => x is null))
                    return table;
                rowValues[0] = minDate;

                table.Rows.Add(rowValues);
            }
        }
    }
}