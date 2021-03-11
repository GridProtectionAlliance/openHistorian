//******************************************************************************************************
//  GetSignalsWithCalculationsMethods.cs - Gbtc
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
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Snap.Filters;
using openHistorian.Snap;

namespace openHistorian.Data.Query
{
    public static class GetSignalsWithCalculationsMethods
    {
        public static IDictionary<Guid, SignalDataBase> GetSignalsWithCalculations(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ISignalCalculation> signals)
        {
            return database.GetSignalsWithCalculations(TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime), signals, SortedTreeEngineReaderOptions.Default);
        }

        public static IDictionary<Guid, SignalDataBase> GetSignalsWithCalculations(this ClientDatabaseBase<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, IEnumerable<ISignalCalculation> signals, SortedTreeEngineReaderOptions readerOptions)
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