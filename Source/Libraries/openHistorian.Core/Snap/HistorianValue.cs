//******************************************************************************************************
//  HistorianValue.cs - Gbtc
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
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************


using System;
using GSF;
using GSF.IO;
using GSF.Snap;

namespace openHistorian.Snap
{
    /// <summary>
    /// The standard value used in the OpenHistorian.
    /// </summary>
    public class HistorianValue
        : SnapTypeBase<HistorianValue>
    {
        /// <summary>
        /// Value 1 should be where the first 64 bits of the field is stored. For 32 bit values, use this field only.
        /// </summary>
        public ulong Value1;
        /// <summary>
        /// Should only be used if value cannot be entirely stored in Value1. Compression penalty occurs when using this field.
        /// </summary>
        public ulong Value2;
        /// <summary>
        /// Should contain any kind of digital data such as Quality. Compression penalty occurs when used for any other type of field.
        /// </summary>
        public ulong Value3;

        public override Guid GenericTypeGuid =>
            // {24DDE7DC-67F9-42B6-A11B-E27C3E62D9EF}
            new Guid(0x24dde7dc, 0x67f9, 0x42b6, 0xa1, 0x1b, 0xe2, 0x7c, 0x3e, 0x62, 0xd9, 0xef);

        public override int Size => 24;

        public override void CopyTo(HistorianValue destination)
        {
            destination.Value1 = Value1;
            destination.Value2 = Value2;
            destination.Value3 = Value3;
        }

        public override int CompareTo(HistorianValue other)
        {
            if (Value1 < other.Value1)
                return -1;
            if (Value1 > other.Value1)
                return 1;
            if (Value2 < other.Value2)
                return -1;
            if (Value2 > other.Value2)
                return 1;
            if (Value3 < other.Value3)
                return -1;
            if (Value3 > other.Value3)
                return 1;

            return 0;
        }

        public override void SetMin()
        {
            Value1 = 0;
            Value2 = 0;
            Value3 = 0;
        }

        public override void SetMax()
        {
            Value1 = ulong.MaxValue;
            Value2 = ulong.MaxValue;
            Value3 = ulong.MaxValue;
        }
        
        /// <summary>
        /// Sets the value to the default values.
        /// </summary>
        public override void Clear()
        {
            Value1 = 0;
            Value2 = 0;
            Value3 = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Value1 = stream.ReadUInt64();
            Value2 = stream.ReadUInt64();
            Value3 = stream.ReadUInt64();
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Value1);
            stream.Write(Value2);
            stream.Write(Value3);
        }

        /// <summary>
        /// Clones this instance of the class.
        /// </summary>
        /// <returns></returns>
        public HistorianValue Clone()
        {
            HistorianValue value = new HistorianValue();
            value.Value1 = Value1;
            value.Value2 = Value2;
            value.Value3 = Value3;
            return value;
        }

        /// <summary>
        /// Creates a struct from this data.
        /// </summary>
        /// <returns></returns>
        public HistorianValueStruct ToStruct()
        {
            return new HistorianValueStruct
                {
                    Value1 = Value1,
                    Value2 = Value2,
                    Value3 = Value3
                };
        }

        /// <summary>
        /// Type casts the <see cref="Value1"/> as a single.
        /// </summary>
        public float AsSingle
        {
            get => BitConvert.ToSingle(Value1);
            set => Value1 = BitConvert.ToUInt64(value);
        }

        /// <summary>
        /// Type casts <see cref="Value1"/> and <see cref="Value2"/> into a 16 character string.
        /// </summary>
        public string AsString
        {
            get
            {
                byte[] data = new byte[16];
                BitConverter.GetBytes(Value1).CopyTo(data, 0);
                BitConverter.GetBytes(Value2).CopyTo(data, 8);
                return System.Text.Encoding.ASCII.GetString(data);
            }
            set
            {
                if (value.Length > 16)
                    throw new OverflowException("String cannot be larger than 16 characters");
                byte[] data = new byte[16];
                System.Text.Encoding.ASCII.GetBytes(value).CopyTo(data, 0);
                Value1 = BitConverter.ToUInt64(data, 0);
                Value2 = BitConverter.ToUInt64(data, 8);
            }
        }


        #region [ Optional Overrides ]

        // Read(byte*)
        // Write(byte*)
        // IsLessThan(T)
        // IsEqualTo(T)
        // IsGreaterThan(T)
        // IsLessThanOrEqualTo(T)
        // IsBetween(T,T)

        public override unsafe void Read(byte* stream)
        {
            Value1 = *(ulong*)stream;
            Value2 = *(ulong*)(stream + 8);
            Value3 = *(ulong*)(stream + 16);
        }
        public override unsafe void Write(byte* stream)
        {
            *(ulong*)stream = Value1;
            *(ulong*)(stream + 8) = Value2;
            *(ulong*)(stream + 16) = Value3;
        }

        #endregion
    }
}