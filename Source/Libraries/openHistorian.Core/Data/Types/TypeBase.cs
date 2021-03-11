//******************************************************************************************************
//  TypeBase.cs - Gbtc
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

namespace openHistorian.Data.Types
{
    /// <summary>
    /// This base class supports proper conversion of 
    /// each primitive type into a native format.
    /// The native format is specified.
    /// If not overloading individual properties, boxing will
    /// occur each time that value is called.
    /// </summary>
    public abstract class TypeBase
    {
        public virtual void ToValue(ulong raw, out double value)
        {
            value = GetValue(raw).ToDouble(null);
        }

        public virtual void ToValue(ulong raw, out float value)
        {
            value = GetValue(raw).ToSingle(null);
        }

        public virtual void ToValue(ulong raw, out long value)
        {
            value = GetValue(raw).ToInt64(null);
        }

        public virtual void ToValue(ulong raw, out ulong value)
        {
            value = GetValue(raw).ToUInt64(null);
        }

        public virtual void ToValue(ulong raw, out int value)
        {
            value = GetValue(raw).ToInt32(null);
        }

        public virtual void ToValue(ulong raw, out uint value)
        {
            value = GetValue(raw).ToUInt32(null);
        }

        public virtual void ToValue(ulong raw, out short value)
        {
            value = GetValue(raw).ToInt16(null);
        }

        public virtual void ToValue(ulong raw, out ushort value)
        {
            value = GetValue(raw).ToUInt16(null);
        }

        public virtual void ToValue(ulong raw, out sbyte value)
        {
            value = GetValue(raw).ToSByte(null);
        }

        public virtual void ToValue(ulong raw, out byte value)
        {
            value = GetValue(raw).ToByte(null);
        }

        public virtual void ToValue(ulong raw, out bool value)
        {
            value = GetValue(raw).ToBoolean(null);
        }

        public double ToDouble(ulong value)
        {
            ToValue(value, out double tmp);
            return tmp;
        }

        public float ToSingle(ulong value)
        {
            ToValue(value, out float tmp);
            return tmp;
        }

        public long ToInt64(ulong value)
        {
            ToValue(value, out long tmp);
            return tmp;
        }

        public ulong ToUInt64(ulong value)
        {
            ToValue(value, out ulong tmp);
            return tmp;
        }

        public int ToInt32(ulong value)
        {
            ToValue(value, out int tmp);
            return tmp;
        }

        public uint ToUInt32(ulong value)
        {
            ToValue(value, out uint tmp);
            return tmp;
        }

        public short ToInt16(ulong value)
        {
            ToValue(value, out short tmp);
            return tmp;
        }

        public ushort ToUInt16(ulong value)
        {
            ToValue(value, out ushort tmp);
            return tmp;
        }

        public sbyte ToSByte(ulong value)
        {
            ToValue(value, out sbyte tmp);
            return tmp;
        }

        public byte ToByte(ulong value)
        {
            ToValue(value, out byte tmp);
            return tmp;
        }

        public bool ToBoolean(ulong value)
        {
            ToValue(value, out bool tmp);
            return tmp;
        }


        public virtual ulong ToRaw(double value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(float value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(long value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(ulong value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(int value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(uint value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(short value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(ushort value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(byte value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(sbyte value)
        {
            return ToRaw((IConvertible)value);
        }

        public virtual ulong ToRaw(bool value)
        {
            return ToRaw((IConvertible)value);
        }

        protected abstract ulong ToRaw(IConvertible value);
        protected abstract IConvertible GetValue(ulong value);
    }
}