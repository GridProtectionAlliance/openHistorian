//******************************************************************************************************
//  MetadataBase.cs - Gbtc
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

namespace openVisN
{

    public abstract class MetadataBase
    {
        public Guid UniqueId { get; private set; }
        public ulong HistorianId { get; private set; }
        public abstract EnumValueType ValueType { get; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public CalculationMethod Calculations { get; private set; }

        protected MetadataBase(Guid uniqueId, ulong historianId, string name, string description, CalculationMethod calculations)
        {
            if (calculations == null)
            {
                calculations = CalculationMethod.Empty;
            }
            Calculations = calculations;
            UniqueId = uniqueId;
            HistorianId = historianId;
            Name = name;
            Description = description;
        }

        public virtual double ToDouble(ulong value)
        {
            return GetValue(value).ToDouble(null);
        }
        public virtual long ToInt64(ulong value)
        {
            return GetValue(value).ToInt64(null);
        }
        public virtual ulong ToUInt64(ulong value)
        {
            return GetValue(value).ToUInt64(null);
        }

        public virtual ulong ToNative(ulong value)
        {
            return ToNative((IConvertible)value);

        }
        public virtual ulong ToNative(long value)
        {
            return ToNative((IConvertible)value);
        }
        public virtual ulong ToNative(double value)
        {
            return ToNative((IConvertible)value);
        }

        protected abstract ulong ToNative(IConvertible value);
        protected abstract IConvertible GetValue(ulong value);

        public override bool Equals(object obj)
        {
            MetadataBase obj2 = obj as MetadataBase;
            if (obj2 == null)
                return false;
            return obj2.UniqueId == UniqueId;
        }
        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }
    }

    public unsafe class MetadataSingle : MetadataBase
    {
        public MetadataSingle(Guid uniqueId, ulong historianId, string name, string description, CalculationMethod calculations = null)
            : base(uniqueId, historianId, name, description, calculations)
        {
        }

        public override EnumValueType ValueType
        {
            get
            {
                return EnumValueType.Single;
            }
        }

        public override ulong ToUInt64(ulong value)
        {
            return (ulong)*(float*)&value;
        }

        public override long ToInt64(ulong value)
        {
            return (long)*(float*)&value;
        }

        public override double ToDouble(ulong value)
        {
            return *(float*)&value;
        }

        protected override ulong ToNative(IConvertible value)
        {
            float tmp = value.ToSingle(null);
            return *(uint*)&tmp;
        }

        protected override IConvertible GetValue(ulong value)
        {
            return *(float*)&value;
        }
    }

    public unsafe class MetadataDouble : MetadataBase
    {
        public MetadataDouble(Guid uniqueId, ulong historianId, string name, string description, CalculationMethod calculations = null)
            : base(uniqueId, historianId, name, description, calculations)
        {
        }

        public override EnumValueType ValueType
        {
            get
            {
                return EnumValueType.Single;
            }
        }

        public override ulong ToUInt64(ulong value)
        {
            return (ulong)*(double*)&value;
        }

        public override long ToInt64(ulong value)
        {
            return (long)*(double*)&value;
        }

        public override double ToDouble(ulong value)
        {
            return *(double*)&value;
        }

        protected override IConvertible GetValue(ulong value)
        {
            return *(double*)&value;
        }

        protected override ulong ToNative(IConvertible value)
        {
            double tmp = value.ToDouble(null);
            return *(ulong*)&tmp;
        }
    }
}
