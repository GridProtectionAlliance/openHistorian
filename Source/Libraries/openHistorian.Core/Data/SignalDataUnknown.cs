//******************************************************************************************************
//  SignalDataUnknown.cs - Gbtc
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
//  12/15/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Data.Types;

namespace openHistorian.Data.Query
{
    /// <summary>
    /// This type of signal only supports reading and writing data via 
    /// its raw type. Type conversions are not supported since its original
    /// type is unknown.
    /// </summary>
    public class SignalDataUnknown
        : SignalDataBase
    {
        private readonly List<ulong> m_dateTime = new List<ulong>();
        private readonly List<ulong> m_values = new List<ulong>();

        public SignalDataUnknown()
        {
        }

        /// <summary>
        /// Provides the type conversion method for the base class to use
        /// </summary>
        protected override TypeBase Method => throw new Exception("SignalDataRaw only supports raw formats and will not convert any values.");

        /// <summary>
        /// Gets the number of values that are in the signal
        /// </summary>
        public override int Count => m_values.Count;

        /// <summary>
        /// Adds a value to the signal in its raw 64-bit format.
        /// </summary>
        /// <param name="time">the time value to consider</param>
        /// <param name="value">the 64-bit value</param>
        public override void AddDataRaw(ulong time, ulong value)
        {
            if (IsComplete)
                throw new Exception("Signal has already been marked as complete");
            m_dateTime.Add(time);
            m_values.Add(value);
        }

        /// <summary>
        /// Gets a value from the signal with the provided index in its
        /// raw 64-bit format.
        /// </summary>
        /// <param name="index">The zero based index of the position</param>
        /// <param name="time">an output field for the time</param>
        /// <param name="value">an output field for the raw 64-bit value</param>
        public override void GetDataRaw(int index, out ulong time, out ulong value)
        {
            time = m_dateTime[index];
            value = m_values[index];
        }

        public override ulong GetDate(int index)
        {
            return m_dateTime[index];
        }
    }
}