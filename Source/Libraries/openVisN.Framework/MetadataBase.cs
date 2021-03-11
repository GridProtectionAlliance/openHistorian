//******************************************************************************************************
//  MetadataBase.cs - Gbtc
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Data.Query;
using openHistorian.Data.Types;
using openVisN.Calculations;

namespace openVisN
{
    public abstract class MetadataBase
        : TypeBase, ISignalCalculation
    {
        public Guid UniqueId
        {
            get;
            private set;
        }

        public ulong? HistorianId
        {
            get;
            private set;
        }

        public TypeBase Functions => this;

        public abstract EnumValueType ValueType
        {
            get;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public CalculationMethod Calculations
        {
            get;
            private set;
        }

        protected MetadataBase(Guid uniqueId, ulong? historianId, string name, string description, CalculationMethod calculations)
        {
            if (calculations is null)
            {
                calculations = CalculationMethod.Empty;
            }
            Calculations = calculations;
            UniqueId = uniqueId;
            HistorianId = historianId;
            Name = name;
            Description = description;
        }

        public override bool Equals(object obj)
        {
            MetadataBase obj2 = obj as MetadataBase;
            if (obj2 is null)
                return false;
            return obj2.UniqueId == UniqueId;
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }

        public Guid SignalId => UniqueId;

        public void Calculate(IDictionary<Guid, SignalDataBase> signals)
        {
            Calculations.Calculate(signals);
        }
    }

    public unsafe class MetadataSingle : MetadataBase
    {
        public MetadataSingle(Guid uniqueId, ulong? historianId, string name, string description, CalculationMethod calculations = null)
            : base(uniqueId, historianId, name, description, calculations)
        {
        }

        public override EnumValueType ValueType => EnumValueType.Single;

        protected override ulong ToRaw(IConvertible value)
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
        public MetadataDouble(Guid uniqueId, ulong? historianId, string name, string description, CalculationMethod calculations = null)
            : base(uniqueId, historianId, name, description, calculations)
        {
        }

        public override EnumValueType ValueType => EnumValueType.Single;

        protected override IConvertible GetValue(ulong value)
        {
            return *(double*)&value;
        }

        protected override ulong ToRaw(IConvertible value)
        {
            double tmp = value.ToDouble(null);
            return *(ulong*)&tmp;
        }
    }
}