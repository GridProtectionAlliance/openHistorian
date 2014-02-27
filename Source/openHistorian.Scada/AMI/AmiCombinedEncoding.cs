//******************************************************************************************************
//  TsCombinedEncoding`1.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  2/21/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF;
using GSF.IO;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Encoding;
using openHistorian.Scada.AMI;

namespace openHistorian.SortedTreeStore.Types.CustomCompression.Ts
{
    public class CreateAmiCombinedEncoding
         : CreateDoubleValueEncodingBase
    {
        // {FEB9D85C-DF2E-477E-A9F3-3ED6C7708A78}
        public static EncodingDefinition TypeGuid = new EncodingDefinition(new Guid(0xfeb9d85c, 0xdf2e, 0x477e, 0xa9, 0xf3, 0x3e, 0xd6, 0xc7, 0x70, 0x8a, 0x78));

        public override Type KeyTypeIfNotGeneric
        {
            get
            {
                return typeof(AmiKey);
            }
        }

        public override Type ValueTypeIfNotGeneric
        {
            get
            {
                return typeof(AmiValue);
            }
        }

        public override EncodingDefinition Method
        {
            get
            {
                return TypeGuid;
            }
        }

        public override DoubleValueEncodingBase<TKey, TValue> Create<TKey, TValue>()
        {
            return (DoubleValueEncodingBase<TKey, TValue>)(object)(new AmiCombinedEncoding());
        }
    }

    public class AmiCombinedEncoding
        : DoubleValueEncodingBase<AmiKey, AmiValue>
    {
        public override EncodingDefinition EncodingMethod
        {
            get
            {
                return CreateAmiCombinedEncoding.TypeGuid;
            }
        }

        public override bool UsesPreviousKey
        {
            get
            {
                return false;
            }
        }

        public override bool UsesPreviousValue
        {
            get
            {
                return false;
            }
        }

        public override int MaxCompressionSize
        {
            get
            {
                return 100;
            }
        }

        public override bool ContainsEndOfStreamSymbol
        {
            get
            {
                return false;
            }
        }

        public override byte EndOfStreamSymbol
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public unsafe override void Encode(BinaryStreamBase stream, AmiKey prevKey, AmiValue prevValue, AmiKey key, AmiValue value)
        {
            stream.Write(key.Timestamp);
            stream.Write(key.PointID);
            stream.Write(value.CollectedTime);
            stream.Write(value.TableId);
            stream.Write(value.DataLength);
            stream.Write(value.Data, 0, value.DataLength);
        }

        public unsafe override void Decode(BinaryStreamBase stream, AmiKey prevKey, AmiValue prevValue, AmiKey key, AmiValue value, out bool isEndOfStream)
        {
            isEndOfStream = false;
            key.Timestamp = stream.ReadUInt64();
            key.PointID = stream.ReadUInt64();
            value.CollectedTime = stream.ReadUInt64();
            value.TableId = stream.ReadInt32();
            value.DataLength = stream.ReadUInt8();
            stream.Read(value.Data, 0, value.DataLength);
        }

        public override DoubleValueEncodingBase<AmiKey, AmiValue> Clone()
        {
            return this;
        }
    }
}
