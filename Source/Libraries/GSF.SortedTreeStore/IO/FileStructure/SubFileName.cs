//******************************************************************************************************
//  SubFileName.cs - Gbtc
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
//  05/18/2013 - Steven E. Chisholm
//       Generated original version of source code.
//  04/11/2017 - J. Ritchie Carroll
//       Modified code to use FIPS compatible security algorithms when required.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GSF.Security.Cryptography;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// This is used to generate the file name that will be used for the sub file. 
    /// </summary>
    public class SubFileName
        : IComparable<SubFileName>, IEquatable<SubFileName>
    {
        /// <summary>
        /// the first 8 bytes of the <see cref="SubFileName"/>
        /// </summary>
        public long RawValue1
        {
            get;
            private set;
        }

        /// <summary>
        /// the next 8 bytes of the <see cref="SubFileName"/>
        /// </summary>
        public long RawValue2
        {
            get;
            private set;
        }

        /// <summary>
        /// The final 4 bytes of the <see cref="SubFileName"/>
        /// </summary>
        public int RawValue3
        {
            get;
            private set;
        }

        private SubFileName(long rawValue1, long rawValue2, int rawValue3)
        {
            RawValue1 = rawValue1;
            RawValue2 = rawValue2;
            RawValue3 = rawValue3;
        }

        /// <summary>
        /// Writes the <see cref="SubFileName"/> to the <see cref="writer"/>.
        /// </summary>
        /// <param name="writer"></param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(RawValue1);
            writer.Write(RawValue2);
            writer.Write(RawValue3);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(SubFileName other)
        {
            if (ReferenceEquals(other, null))
                return 1;
            int compare = RawValue1.CompareTo(other.RawValue1);
            if (compare != 0)
                return compare;
            compare = RawValue2.CompareTo(other.RawValue2);
            if (compare != 0)
                return compare;
            return RawValue3.CompareTo(other.RawValue3);
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
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(obj, this))
                return true;
            return Equals(obj as SubFileName);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SubFileName other)
        {
            return this == other;
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
            //Since using SHA1 to compute the name. Taking a single field is good enough.
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return RawValue3 & int.MaxValue;
        }

        #region [ Static ]

        /// <summary>
        /// An empty sub file name. Should not generally be used as a single file system 
        /// must have unique file names.
        /// </summary>
        public static SubFileName Empty => new SubFileName(0, 0, 0);

        /// <summary>
        /// Creates a random <see cref="SubFileName"/>
        /// </summary>
        /// <returns></returns>
        public static SubFileName CreateRandom()
        {
            return Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        }

        /// <summary>
        /// Creates a <see cref="SubFileName"/> from the supplied data.
        /// </summary>
        /// <param name="fileType">the type identifier of the file</param>
        /// <param name="keyType">the guid identifier of the type of the SortedTreeStore</param>
        /// <param name="valueType">the guid identifier of the value type of the SortedTreeStore</param>
        /// <returns></returns>
        public static unsafe SubFileName Create(Guid fileType, Guid keyType, Guid valueType)
        {
            byte[] data = new byte[16 * 3];
            fixed (byte* lp = data)
            {
                *(Guid*)lp = fileType;
                *(Guid*)(lp + 16) = keyType;
                *(Guid*)(lp + 32) = valueType;
            }
            return Create(data);
        }

        /// <summary>
        /// Creates a <see cref="SubFileName"/> from the supplied data.
        /// </summary>
        /// <param name="fileName">a name associated with the data</param>
        /// <param name="keyType">the guid identifier of the type of the SortedTreeStore</param>
        /// <param name="valueType">the guid identifier of the value type of the SortedTreeStore</param>
        /// <returns></returns>
        public static unsafe SubFileName Create(string fileName, Guid keyType, Guid valueType)
        {
            byte[] data = new byte[16 * 2 + fileName.Length * 2];
            fixed (byte* lp = data)
            {
                *(Guid*)lp = keyType;
                *(Guid*)(lp + 16) = valueType;
            }
            Encoding.Unicode.GetBytes(fileName, 0, fileName.Length, data, 32);

            return Create(data);
        }

        /// <summary>
        /// Creates a <see cref="SubFileName"/> from the supplied data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static unsafe SubFileName Create(byte[] data)
        {
            using (SHA1 sha1 = Cipher.CreateSHA1())
            {
                byte[] hash = sha1.ComputeHash(data);
                fixed (byte* lp = hash)
                {
                    return new SubFileName(*(long*)lp, *(long*)(lp + 8), *(int*)(lp + 16));
                }
            }
        }

        /// <summary>
        /// Loads the <see cref="SubFileName"/> from the supplied <see cref="reader"/>.
        /// </summary>
        /// <param name="reader">the reader to read from.</param>
        /// <returns></returns>
        public static SubFileName Load(BinaryReader reader)
        {
            long value1 = reader.ReadInt64();
            long value2 = reader.ReadInt64();
            int value3 = reader.ReadInt32();
            return new SubFileName(value1, value2, value3);
        }

        /// <summary>
        /// Compares the equality of the two file names.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(SubFileName a, SubFileName b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.RawValue1 == b.RawValue1 && a.RawValue2 == b.RawValue2 && a.RawValue3 == b.RawValue3;
        }

        /// <summary>
        /// Compares if the two files are not equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(SubFileName a, SubFileName b)
        {
            return !(a == b);
        }

        #endregion


    }
}