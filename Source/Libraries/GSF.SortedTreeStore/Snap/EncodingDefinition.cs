//******************************************************************************************************
//  EncodingDefinition.cs - Gbtc
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
//  02/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.IO;

namespace GSF.Snap
{
    /// <summary>
    /// An immutable class that represents the compression method used 
    /// by the SortedTreeStore.
    /// </summary>
    /// <remarks>
    /// 
    /// Serializes as:
    /// If Combined KeyValue encoding
    /// byte type = 1
    /// Guid KeyValueEncodingMethod
    /// 
    /// If Individual Compression
    /// byte type = 2
    /// Guid KeyEncodingMethod
    /// Guid ValueEncodingMethod
    /// </remarks>
    public class EncodingDefinition : IComparable<EncodingDefinition>, IComparable, IEquatable<EncodingDefinition>
    {
        /// <summary>
        /// Gets if the compression method compresses the key and value as a unit.
        /// </summary>
        public bool IsKeyValueEncoded { get; private set; }

        private readonly Guid m_keyEncodingMethod;
        private readonly Guid m_valueEncodingMethod;
        private readonly Guid m_keyValueEncodingMethod;
        private readonly int m_hashCode;
        private readonly bool m_isFixedSizeEncoding;

        /// <summary>
        /// Loads a <see cref="EncodingDefinition"/> from a stream
        /// </summary>
        /// <param name="stream">the stream to load from.</param>
        public EncodingDefinition(BinaryStreamBase stream)
        {
            byte code = stream.ReadUInt8();
            if (code == 1)
            {
                m_keyEncodingMethod = Guid.Empty;
                m_valueEncodingMethod = Guid.Empty;
                m_keyValueEncodingMethod = stream.ReadGuid();
                IsKeyValueEncoded = true;
            }
            else if (code == 2)
            {
                m_keyEncodingMethod = stream.ReadGuid();
                m_valueEncodingMethod = stream.ReadGuid();
                m_keyValueEncodingMethod = Guid.Empty;
                IsKeyValueEncoded = false;
            }
            m_hashCode = ComputeHashCode();
            m_isFixedSizeEncoding = this == FixedSizeCombinedEncoding ||
                                    this == FixedSizeIndividualEncoding;
        }

        /// <summary>
        /// Loads a <see cref="EncodingDefinition"/> from a stream
        /// </summary>
        /// <param name="stream">the stream to load from.</param>
        public EncodingDefinition(Stream stream)
        {
            byte code = stream.ReadNextByte();
            if (code == 1)
            {
                m_keyEncodingMethod = Guid.Empty;
                m_valueEncodingMethod = Guid.Empty;
                m_keyValueEncodingMethod = stream.ReadGuid();
                IsKeyValueEncoded = true;
            }
            else if (code == 2)
            {
                m_keyEncodingMethod = stream.ReadGuid();
                m_valueEncodingMethod = stream.ReadGuid();
                m_keyValueEncodingMethod = Guid.Empty;
                IsKeyValueEncoded = false;
            }
            m_hashCode = ComputeHashCode();
            m_isFixedSizeEncoding = this == FixedSizeCombinedEncoding ||
                                    this == FixedSizeIndividualEncoding;
        }

        /// <summary>
        /// Specifies a combined key/value encoding method with the provided <see cref="Guid"/>.
        /// </summary>
        /// <param name="keyValueEncoding">A <see cref="Guid"/> that is the encoding method that is registered with the system.</param>
        public EncodingDefinition(Guid keyValueEncoding)
        {
            m_keyEncodingMethod = Guid.Empty;
            m_valueEncodingMethod = Guid.Empty;
            m_keyValueEncodingMethod = keyValueEncoding;
            IsKeyValueEncoded = true;
            m_hashCode = ComputeHashCode();
            m_isFixedSizeEncoding = keyValueEncoding == FixedSizeIndividualGuid;
        }

        /// <summary>
        /// Specifies an encoding method that independently compresses the key and the value.
        /// </summary>
        /// <param name="keyEncoding">the encoding of the key</param>
        /// <param name="valueEncoding">the encoding of the value</param>
        public EncodingDefinition(Guid keyEncoding, Guid valueEncoding)
        {
            m_keyEncodingMethod = keyEncoding;
            m_valueEncodingMethod = valueEncoding;
            m_keyValueEncodingMethod = Guid.Empty;
            IsKeyValueEncoded = false;
            m_hashCode = ComputeHashCode();
            m_isFixedSizeEncoding = keyEncoding == FixedSizeIndividualGuid &&
                                    valueEncoding == FixedSizeIndividualGuid;
        }

        /// <summary>
        /// Gets if the encoding method is the special fixed size encoding method.
        /// </summary>
        public bool IsFixedSizeEncoding => m_isFixedSizeEncoding;

        /// <summary>
        /// Gets the compression method if <see cref="IsKeyValueEncoded"/> is false.
        /// Throw an exception otherwise.
        /// </summary>
        public Guid KeyEncodingMethod
        {
            get
            {
                if (IsKeyValueEncoded)
                    throw new Exception("Not Valid");
                return m_keyEncodingMethod;
            }
        }

        /// <summary>
        /// Gets the compression method if <see cref="IsKeyValueEncoded"/> is false.
        /// Throw an exception otherwise.
        /// </summary>
        public Guid ValueEncodingMethod
        {
            get
            {
                if (IsKeyValueEncoded)
                    throw new Exception("Not Valid");
                return m_valueEncodingMethod;
            }
        }

        /// <summary>
        /// Gets the compression method if <see cref="IsKeyValueEncoded"/> is true.
        /// Throw an exception otherwise.
        /// </summary>
        public Guid KeyValueEncodingMethod
        {
            get
            {
                if (!IsKeyValueEncoded)
                    throw new Exception("Not Valid");
                return m_keyValueEncodingMethod;
            }
        }

        /// <summary>
        /// Serializes the <see cref="EncodingDefinition"/> to the provided <see cref="stream"/>
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public void Save(BinaryStreamBase stream)
        {
            if (IsKeyValueEncoded)
            {
                stream.Write((byte)1);
                stream.Write(KeyValueEncodingMethod);
            }
            else
            {
                stream.Write((byte)2);
                stream.Write(KeyEncodingMethod);
                stream.Write(ValueEncodingMethod);
            }
        }
        /// <summary>
        /// Serializes the <see cref="EncodingDefinition"/> to the provided <see cref="stream"/>
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public void Save(Stream stream)
        {
            if (IsKeyValueEncoded)
            {
                stream.Write((byte)1);
                stream.Write(KeyValueEncodingMethod);
            }
            else
            {
                stream.Write((byte)2);
                stream.Write(KeyEncodingMethod);
                stream.Write(ValueEncodingMethod);
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(EncodingDefinition other)
        {
            int cmp;
            cmp = m_hashCode.CompareTo(other.m_hashCode); if (cmp != 0) return cmp;
            cmp = IsKeyValueEncoded.CompareTo(other.IsKeyValueEncoded); if (cmp != 0) return cmp;
            cmp = m_keyEncodingMethod.CompareTo(other.m_keyEncodingMethod); if (cmp != 0) return cmp;
            cmp = m_valueEncodingMethod.CompareTo(other.m_valueEncodingMethod); if (cmp != 0) return cmp;
            return m_keyValueEncodingMethod.CompareTo(other.m_keyValueEncodingMethod);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EncodingDefinition other)
        {
            return (object)other != null &&
                   m_hashCode == other.m_hashCode &&
                   IsKeyValueEncoded == other.IsKeyValueEncoded &&
                   m_keyEncodingMethod == other.m_keyEncodingMethod &&
                   m_valueEncodingMethod == other.m_valueEncodingMethod &&
                   m_keyValueEncodingMethod == other.m_keyValueEncodingMethod;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            EncodingDefinition o = obj as EncodingDefinition;
            if (o is null)
                return false;
            return Equals(o);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            EncodingDefinition o = obj as EncodingDefinition;
            if (o is null)
                return -1;
            return CompareTo(o);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        private int ComputeHashCode()
        {
            return IsKeyValueEncoded.GetHashCode() ^
                          m_keyEncodingMethod.GetHashCode() ^
                          m_valueEncodingMethod.GetHashCode() ^
                          m_keyValueEncodingMethod.GetHashCode();
        }

        /// <summary>
        /// Checks for inequality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(EncodingDefinition a, EncodingDefinition b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(EncodingDefinition a, EncodingDefinition b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null)
                return false;
            return a.Equals(b);
        }

        static EncodingDefinition()
        {
            FixedSizeIndividualGuid = new Guid(0x1dea326d, 0xa63a, 0x4f73, 0xb5, 0x1c, 0x7b, 0x31, 0x25, 0xc6, 0xda,0x55);
            FixedSizeCombinedEncoding = new EncodingDefinition(FixedSizeIndividualGuid);
            FixedSizeIndividualEncoding = new EncodingDefinition(FixedSizeIndividualGuid, FixedSizeIndividualGuid);
        }

        /// <summary>
        /// Represents a FixedSize combined encoding method.
        /// </summary>
        public static readonly EncodingDefinition FixedSizeCombinedEncoding;

        /// <summary>
        /// Represents a FixedSize combined encoding method made up of two individual fixed size IDs. 
        /// Functionally implemented the same as <see cref="FixedSizeCombinedEncoding"/> 
        /// </summary>
        public static readonly EncodingDefinition FixedSizeIndividualEncoding;

        /// <summary>
        /// The Guid associated with the individual encoding method of a FixedSize
        /// </summary>
        public static readonly Guid FixedSizeIndividualGuid;

    }


}
