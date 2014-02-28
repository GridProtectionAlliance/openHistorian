//******************************************************************************************************
//  GetSignalsWithCalculationsMethods.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Engine.Reader;
using GSF.SortedTreeStore.Filters;
using openHistorian.Collections;

namespace openHistorian.Data.Query
{
    public static class GetSignalsWithCalculationsMethods
    {
        public static IDictionary<Guid, SignalDataBase> GetSignalsWithCalculations(this SortedTreeEngineBase<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ISignalCalculation> signals)
        {
            return database.GetSignalsWithCalculations(TimestampFilter.CreateFromRange<HistorianKey>(startTime, endTime), signals, SortedTreeEngineReaderOptions.Default);
        }

        public static IDictionary<Guid, SignalDataBase> GetSignalsWithCalculations(this SortedTreeEngineBase<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, IEnumerable<ISignalCalculation> signals, SortedTreeEngineReaderOptions readerOptions)
        {
            Dictionary<ulong, SignalDataBase> queryResults = database.GetSignals(timestamps, signals, readerOptions);

            Dictionary<Guid, SignalDataBase> calculatedResults = new Dictionary<Guid, SignalDataBase>();
            foreach (ISignalCalculation signal in signals)
            {
                if (signal.HistorianId.HasValue)
                {
                    calculatedResults.Add(signal.SignalId, queryResults[signal.HistorianId.Value]);
                }
                else
                {
                    calculatedResults.Add(signal.SignalId, new SignalData(signal.Functions));
                }
            }

            foreach (ISignalCalculation signal in signals)
            {
                signal.Calculate(calculatedResults);
            }
            return calculatedResults;
        }
    }
}