//******************************************************************************************************
//  HistorianValue.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************


using System;
using System.Text;
using GSF;
using GSF.IO;

namespace openHistorian.Collections
{
    public struct HistorianValueStruct
    {
        public ulong Value1;
        public ulong Value2;
        public ulong Value3;

        public float AsSingle
        {
            get
            {
                return BitMath.ConvertToSingle(Value1);
            }
            set
            {
                Value1 = BitMath.ConvertToUInt64(value);
            }
        }

        public string AsString
        {
            get
            {
                byte[] data = new byte[16];
                BitConverter.GetBytes(Value1).CopyTo(data, 0);
                BitConverter.GetBytes(Value2).CopyTo(data, 8);
                return ASCIIEncoding.ASCII.GetString(data);
            }
            set
            {
                if (value.Length > 16)
                    throw new OverflowException("String cannot be larger than 16 characters");
                byte[] data = new byte[16];
                ASCIIEncoding.ASCII.GetBytes(value).CopyTo(data, 0);
                Value1 = BitConverter.ToUInt64(data, 0);
                Value2 = BitConverter.ToUInt64(data, 8);
            }
        }
    }

    public class HistorianValue
        : HistorianValueBase<HistorianValue>
    {
        /// <summary>
        /// Value 1 should be where the first 64 bits of the field is stored. For 32 bit values, use this field only.
        /// </summary>
        public ulong Value1;
        /// <summary>
        /// Should only be used if value cannot be entirely stored in Value1. Compression penality occurs when using this field.
        /// </summary>
        public ulong Value2;
        /// <summary>
        /// Should contain any kind of digital data such as Quality. Compression penality occurs when used for any other type of field.
        /// </summary>
        public ulong Value3;

        public override bool IsEqualTo(HistorianValue other)
        {
            return Value1 == other.Value1 && Value2 == other.Value2 && Value3 == other.Value3;
        }

        public override void CopyTo(HistorianValue other)
        {
            other.Value1 = Value1;
            other.Value2 = Value2;
            other.Value3 = Value3;
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Value1);
            stream.Write(Value2);
            stream.Write(Value3);
        }

        public override void Read(BinaryStreamBase stream)
        {
            Value1 = stream.ReadUInt64();
            Value2 = stream.ReadUInt64();
            Value3 = stream.ReadUInt64();
        }

        public override void WriteCompressed(BinaryStreamBase stream, HistorianValue previousValue)
        {
            stream.Write7Bit(previousValue.Value1 ^ Value1);
            stream.Write7Bit(previousValue.Value2 ^ Value2);
            stream.Write7Bit(previousValue.Value3 ^ Value3);
        }

        public override void ReadCompressed(BinaryStreamBase stream, HistorianValue previousValue)
        {
            Value1 = stream.Read7BitUInt64() ^ previousValue.Value1;
            Value2 = stream.Read7BitUInt64() ^ previousValue.Value2;
            Value3 = stream.Read7BitUInt64() ^ previousValue.Value3;
        }

        public override void Clear()
        {
            Value1 = 0;
            Value2 = 0;
            Value3 = 0;
        }

        public HistorianValue Clone()
        {
            HistorianValue value = new HistorianValue();
            value.Value1 = Value1;
            value.Value2 = Value2;
            value.Value3 = Value3;
            return value;
        }

        public HistorianValueStruct ToStruct()
        {
            return new HistorianValueStruct()
                {
                    Value1 = Value1,
                    Value2 = Value2,
                    Value3 = Value3
                };
        }

        public float AsSingle
        {
            get
            {
                return BitMath.ConvertToSingle(Value1);
            }
            set
            {
                Value1 = BitMath.ConvertToUInt64(value);
            }
        }

        public string AsString
        {
            get
            {
                byte[] data = new byte[16];
                BitConverter.GetBytes(Value1).CopyTo(data, 0);
                BitConverter.GetBytes(Value2).CopyTo(data, 8);
                return ASCIIEncoding.ASCII.GetString(data);
            }
            set
            {
                if (value.Length > 16)
                    throw new OverflowException("String cannot be larger than 16 characters");
                byte[] data = new byte[16];
                ASCIIEncoding.ASCII.GetBytes(value).CopyTo(data, 0);
                Value1 = BitConverter.ToUInt64(data, 0);
                Value2 = BitConverter.ToUInt64(data, 8);
            }
        }
    }
}