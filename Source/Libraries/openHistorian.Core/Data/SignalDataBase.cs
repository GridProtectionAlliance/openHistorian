//******************************************************************************************************
//  SignalDataBase.cs - Gbtc
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
using openHistorian.Data.Types;

namespace openHistorian.Data.Query
{
    /// <summary>
    /// Contains a series of Times and Values for an individual signal.
    /// Abstract versions of this class should attempt to store the value
    /// in its most appropriate format to reduce the amount of conversion
    /// that must occur.
    /// </summary>
    public abstract class SignalDataBase
    {
        private bool m_isComplete;

        /// <summary>
        /// Gets if this signal has been processed and can no longer be added to.
        /// </summary>
        public bool IsComplete => m_isComplete;

        /// <summary>
        /// Flags this signal as complete which locks down the ability to add 
        /// additional points to it.
        /// </summary>
        public void Completed()
        {
            m_isComplete = true;
        }

        /// <summary>
        /// Provides the type conversion method for the base class to use
        /// </summary>
        protected abstract TypeBase Method
        {
            get;
        }

        /// <summary>
        /// Gets the number of values that are in the signal
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// Adds a value to the signal in its raw 64-bit format.
        /// </summary>
        /// <param name="time">the time value to consider</param>
        /// <param name="value">the 64-bit value</param>
        public abstract void AddDataRaw(ulong time, ulong value);

        /// <summary>
        /// Gets a value from the signal with the provided index in its
        /// raw 64-bit format.
        /// </summary>
        /// <param name="index">The zero based index of the position</param>
        /// <param name="time">an output field for the time</param>
        /// <param name="value">an output field for the raw 64-bit value</param>
        public abstract void GetDataRaw(int index, out ulong time, out ulong value);


        public abstract ulong GetDate(int index);

        /// <summary>
        /// Gets a value from the signal with the provided index and automatically 
        /// converts it to a <see cref="double"/>.
        /// </summary>
        /// <param name="index">The zero based index of the position</param>
        /// <param name="time">an output field for the time</param>
        /// <param name="value">an output field for the converted value</param>
        public virtual void GetData(int index, out ulong time, out double value)
        {
            GetDataRaw(index, out time, out ulong raw);
            Method.ToValue(raw, out value);
        }

        /// <summary>
        /// Gets a value from the signal with the provided index and automatically 
        /// converts it to a <see cref="float"/>.
        /// </summary>
        /// <param name="index">The zero based index of the position</param>
        /// <param name="time">an output field for the time</param>
        /// <param name="value">an output field for the converted value</param>
        public virtual void GetData(int index, out ulong time, out float value)
        {
            GetDataRaw(index, out time, out ulong raw);
            Method.ToValue(raw, out value);
        }

        /// <summary>
        /// Gets a value from the signal with the provided index and automatically 
        /// converts it to a <see cref="long"/>.
        /// </summary>
        /// <param name="index">The zero based index of the position</param>
        /// <param name="time">an output field for the time</param>
        /// <param name="value">an output field for the converted value</param>
        public virtual void GetData(int index, out ulong time, out long value)
        {
            GetDataRaw(index, out time, out ulong raw);
            Method.ToValue(raw, out value);
        }

        /// <summary>
        /// Gets a value from the signal with the provided index and automatically 
        /// converts it to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="index">The zero based index of the position</param>
        /// <param name="time">an output field for the time</param>
        /// <param name="value">an output field for the converted value</param>
        public virtual void GetData(int index, out ulong time, out ulong value)
        {
            GetDataRaw(index, out time, out ulong raw);
            Method.ToValue(raw, out value);
        }

        /// <summary>
        /// Adds a value to the signal and converts it from a <see cref="double"/>
        /// into its native format.
        /// </summary>
        /// <param name="time">the time value to consider</param>
        /// <param name="value">the value to convert</param>
        public virtual void AddData(ulong time, double value)
        {
            if (IsComplete)
                throw new Exception("Signal has already been marked as complete");
            AddDataRaw(time, Method.ToRaw(value));
        }

        /// <summary>
        /// Adds a value to the signal and converts it from a <see cref="float"/>
        /// into its native format.
        /// </summary>
        /// <param name="time">the time value to consider</param>
        /// <param name="value">the value to convert</param>
        public virtual void AddData(ulong time, float value)
        {
            if (IsComplete)
                throw new Exception("Signal has already been marked as complete");
            AddDataRaw(time, Method.ToRaw(value));
        }

        /// <summary>
        /// Adds a value to the signal and converts it from a <see cref="ulong"/>
        /// into its native format.
        /// </summary>
        /// <param name="time">the time value to consider</param>
        /// <param name="value">the value to convert</param>
        public virtual void AddData(ulong time, ulong value)
        {
            if (IsComplete)
                throw new Exception("Signal has already been marked as complete");
            AddDataRaw(time, Method.ToRaw(value));
        }

        /// <summary>
        /// Adds a value to the signal and converts it from a <see cref="long"/>
        /// into its native format.
        /// </summary>
        /// <param name="time">the time value to consider</param>
        /// <param name="value">the value to convert</param>
        public virtual void AddData(ulong time, long value)
        {
            if (IsComplete)
                throw new Exception("Signal has already been marked as complete");
            AddDataRaw(time, Method.ToRaw(value));
        }
    }
}