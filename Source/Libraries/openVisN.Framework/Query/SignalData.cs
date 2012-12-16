//******************************************************************************************************
//  SignalData.cs - Gbtc
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
//  12/15/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************
using System;
using System.Collections.Generic;

namespace openVisN.Query
{
    public abstract class SignalDataBase
    {
        protected abstract ValueTypeConversionBase ConversionMethod { get; }
        public abstract int Count { get; }
        public abstract void AddDataRaw(ulong time, ulong value);
        public abstract void GetDataRaw(int index, out ulong time, out ulong value);

        public virtual void GetData(int index, out ulong time, out double value)
        {
            ulong raw;
            GetDataRaw(index, out time, out raw);
            ConversionMethod.ToValue(raw, out value);
        }

        public virtual void GetData(int index, out ulong time, out float value)
        {
            ulong raw;
            GetDataRaw(index, out time, out raw);
            ConversionMethod.ToValue(raw, out value);
        }

        public virtual void GetData(int index, out ulong time, out long value)
        {
            ulong raw;
            GetDataRaw(index, out time, out raw);
            ConversionMethod.ToValue(raw, out value);
        }
        public virtual void GetData(int index, out ulong time, out ulong value)
        {
            ulong raw;
            GetDataRaw(index, out time, out raw);
            ConversionMethod.ToValue(raw, out value);
        }

        public virtual void AddData(ulong time, double value)
        {
            AddDataRaw(time, ConversionMethod.ToRaw(value));
        }
        public virtual void AddData(ulong time, float value)
        {
            AddDataRaw(time, ConversionMethod.ToRaw(value));
        }
        public virtual void AddData(ulong time, ulong value)
        {
            AddDataRaw(time, ConversionMethod.ToRaw(value));
        }
        public virtual void AddData(ulong time, long value)
        {
            AddDataRaw(time, ConversionMethod.ToRaw(value));
        }

    }

    /// <summary>
    /// This type of signal only supports reading and writing via ulong value type.
    /// </summary>
    public class SignalDataRaw
        : SignalDataBase
    {
        List<ulong> m_dateTime = new List<ulong>();
        List<ulong> m_values = new List<ulong>();

        public SignalDataRaw()
        {
        }

        protected override ValueTypeConversionBase ConversionMethod
        {
            get
            {
                throw new Exception("SignalDataRaw only supports raw formats and will not convert any values.");
            }
        }

        public override int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public override void AddDataRaw(ulong time, ulong value)
        {
            m_dateTime.Add(time);
            m_values.Add(value);
        }

        public override void GetDataRaw(int index, out ulong time, out ulong value)
        {
            time = m_dateTime[index];
            value = m_values[index];
        }
    }

    public class SignalData
        : SignalDataBase
    {
        List<ulong> m_dateTime = new List<ulong>();
        List<ulong> m_values = new List<ulong>();

        ValueTypeConversionBase m_typeConversion;

        public SignalData(ValueTypeConversionBase typeConversion)
        {
            m_typeConversion = typeConversion;
        }

        protected override ValueTypeConversionBase ConversionMethod
        {
            get
            {
                return m_typeConversion;
            }
        }

        public override int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public override void AddDataRaw(ulong time, ulong value)
        {
            m_dateTime.Add(time);
            m_values.Add(value);
        }

        public override void GetDataRaw(int index, out ulong time, out ulong value)
        {
            time = m_dateTime[index];
            value = m_values[index];
        }
    }

    public class SignalDataSingle
        : SignalDataBase
    {
        List<ulong> m_dateTime = new List<ulong>();
        List<float> m_values = new List<float>();

        ValueTypeConversionBase m_typeConversion;

        public SignalDataSingle()
        {
            m_typeConversion = ValueTypeConversionSingle.Instance;
        }

        protected override ValueTypeConversionBase ConversionMethod
        {
            get
            {
                return m_typeConversion;
            }
        }

        public override int Count
        {
            get
            {
                return m_values.Count;
            }
        }

        public override void AddData(ulong time, float value)
        {
            m_dateTime.Add(time);
            m_values.Add(value);
        }

        public override void GetData(int index, out ulong time, out float value)
        {
            time = m_dateTime[index];
            value = m_values[index];
        }

        public unsafe override void AddDataRaw(ulong time, ulong value)
        {
            uint tmp = (uint)value;
            AddData(time, *(float*)&tmp);
        }

        public unsafe override void GetDataRaw(int index, out ulong time, out ulong value)
        {
            float tmp;
            GetData(index, out time, out tmp);
            value = *(uint*)&tmp;
        }
    }


}
