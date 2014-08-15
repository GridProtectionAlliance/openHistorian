//******************************************************************************************************
//  StreamExtensions.cs - Gbtc
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
//  8/15/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;

namespace GSF.IO
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, int value)
        {
            byte[] valueBytes = LittleEndian.GetBytes(value);
            stream.Write(valueBytes, 0, 4);
        }

        public static void WriteWithLength(this Stream stream, byte[] data)
        {
            Encoding7Bit.Write(stream.WriteByte, (uint)data.Length);
            stream.Write(data, 0, data.Length);
        }

        public static byte[] ReadBytes(this Stream stream)
        {
            int length = (int)stream.Read7BitUInt32();
            if (length < 0)
                throw new Exception("Invalid length");
            byte[] data = new byte[length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        public static int ReadInt32(this Stream stream)
        {
            //Little endian encoded integer
            byte b1 = stream.ReadNextByte();
            byte b2 = stream.ReadNextByte();
            byte b3 = stream.ReadNextByte();
            byte b4 = stream.ReadNextByte();
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }

        public static byte ReadNextByte(this Stream stream)
        {
            int value = stream.ReadByte();
            if (value < 0)
                ThrowEOS();
            return (byte)value;
        }

        public static uint Read7BitUInt32(this Stream stream)
        {
            return Encoding7Bit.ReadUInt32(stream);
        }

        static void ThrowEOS()
        {
            throw new EndOfStreamException("End of stream");
        }

    }
}
