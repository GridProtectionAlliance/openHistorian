//******************************************************************************************************
//  KeyParserSecondary.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using openHistorian.Collections;
using openHistorian.IO;

namespace openHistorian
{
    public abstract class KeyParserSecondary
    {
        ulong m_maxValue;

        public static KeyParserSecondary CreateAllKeysValid()
        {
            return UniverseFilter.Instance;
        }
        public static KeyParserSecondary CreateFromList(IEnumerable<ulong> listOfKeys)
        {
            KeyParserSecondary filter;
            ulong maxValue = 0;
            if (listOfKeys.Any())
                maxValue = listOfKeys.Max();

            if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
            {
                filter = new BitArrayFilter(listOfKeys, maxValue);
            }
            else if (maxValue <= uint.MaxValue)
            {
                filter = new UintHashSetFilter(listOfKeys);
            }
            else
            {
                filter = new UlongHashSetFilter(listOfKeys);
            }
            filter.m_maxValue = maxValue;
            return filter;
        }
        
        public static KeyParserSecondary CreateFromStream(BinaryStreamBase stream)
        {
            KeyParserSecondary filter;
            var version = stream.ReadByte();
            ulong maxValue;
            int count;
            switch (version)
            {
                case 0:
                    return UniverseFilter.Instance;
                case 1:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    if (maxValue < 8 * 1024 * 64) //64KB of space, 524288
                    {
                        filter = new BitArrayFilter(stream, count, maxValue);
                    }
                    else
                    {
                        filter = new UintHashSetFilter(stream, count, maxValue);
                    }
                    break;
                case 2:
                    maxValue = stream.ReadUInt64();
                    count = stream.ReadInt32();
                    filter = new UlongHashSetFilter(stream, count, maxValue);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
            filter.m_maxValue = maxValue;
            return filter;
        }

        public abstract bool ContainsKey(ulong key);

        protected abstract void WriteToStream(BinaryStreamBase stream);
        protected abstract int Count { get; }

        public void Save(BinaryStreamBase stream)
        {
            if (this is UniverseFilter)
            {
                stream.Write((byte)0); //No data stored

            }
            else if (this is BitArrayFilter)
            {
                stream.Write((byte)1); //Stored as array of uint[]
                stream.Write(m_maxValue);
                stream.Write(Count);
                WriteToStream(stream);
            }
            else if (this is UintHashSetFilter)
            {
                stream.Write((byte)1); //Stored as array of uint[]
                stream.Write(m_maxValue);
                stream.Write(Count);
                WriteToStream(stream);
            }
            else if (this is UlongHashSetFilter)
            {
                stream.Write((byte)2); //Stored as array of ulong[]
                stream.Write(m_maxValue);
                stream.Write(Count);
                WriteToStream(stream);
            }
            else
            {
                throw new NotSupportedException("The provided inherited class cannot be serialized");
            }
        }

        /// <summary>
        /// A filter that returns true for every key
        /// </summary>
        class UniverseFilter : KeyParserSecondary
        {
            public static UniverseFilter Instance { get; private set; }
            static UniverseFilter()
            {
                Instance = new UniverseFilter();
            }
            public override bool ContainsKey(ulong key)
            {
                return true;
            }
            protected override void WriteToStream(BinaryStreamBase stream)
            {
            }

            protected override int Count
            {
                get
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// A filter that uses a <see cref="BitArray"/> to set true and false values
        /// </summary>
        class BitArrayFilter : KeyParserSecondary
        {
            BitArray m_points;
            ulong m_maxValue;

            public BitArrayFilter(BinaryStreamBase stream, int pointCount, ulong maxValue)
            {
                m_maxValue = maxValue;
                m_points = new BitArray((int)maxValue + 1, false);
                while (pointCount > 0)
                {
                    m_points.SetBit((int)stream.ReadUInt32());
                    pointCount--;
                }
            }

            public BitArrayFilter(IEnumerable<ulong> points, ulong maxValue)
            {
                m_maxValue = maxValue;
                m_points = new BitArray((int)maxValue + 1, false);
                foreach (ulong pt in points)
                {
                    m_points.SetBit((int)pt);
                }
            }

            public override bool ContainsKey(ulong key)
            {
                return key <= m_maxValue && m_points[(int)key];
            }

            protected override void WriteToStream(BinaryStreamBase stream)
            {
                foreach (int x in m_points.GetAllSetBits())
                {
                    stream.Write((uint)x);
                }
            }

            protected override int Count
            {
                get
                {
                    return m_points.SetCount;
                }
            }
        }

        class UintHashSetFilter : KeyParserSecondary
        {
            HashSet<uint> m_points;

            public UintHashSetFilter(BinaryStreamBase stream, int pointCount, ulong maxValue)
            {
                m_points = new HashSet<uint>();
                while (pointCount > 0)
                {
                    m_points.Add(stream.ReadUInt32());
                    pointCount--;
                }
            }

            public UintHashSetFilter(IEnumerable<ulong> points)
            {
                m_points = new HashSet<uint>();
                m_points.UnionWith(points.Select(x => (uint)x));
            }

            public override bool ContainsKey(ulong key)
            {
                return key <= uint.MaxValue && m_points.Contains((uint)key);
            }

            protected override void WriteToStream(BinaryStreamBase stream)
            {
                foreach (uint x in m_points)
                {
                    stream.Write(x);
                }
            }

            protected override int Count
            {
                get
                {
                    return m_points.Count;
                }
            }
        }

        class UlongHashSetFilter : KeyParserSecondary
        {
            HashSet<ulong> m_points;

            public UlongHashSetFilter(BinaryStreamBase stream, int pointCount, ulong maxValue)
            {
                m_points = new HashSet<ulong>();
                while (pointCount > 0)
                {
                    m_points.Add(stream.ReadUInt64());
                    pointCount--;
                }
            }

            public UlongHashSetFilter(IEnumerable<ulong> points)
            {
                m_points = new HashSet<ulong>(points);
            }

            public override bool ContainsKey(ulong key)
            {
                return m_points.Contains(key);
            }

            protected override void WriteToStream(BinaryStreamBase stream)
            {
                foreach (ulong x in m_points)
                {
                    stream.Write(x);
                }
            }
            protected override int Count
            {
                get
                {
                    return m_points.Count;
                }
            }

        }


    }
}
