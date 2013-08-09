//******************************************************************************************************
//  SubFileName.cs - Gbtc
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
//  5/18/2013 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace openHistorian.FileStructure
{
    /// <summary>
    /// This is used to generate the file name that will be used for the sub file. 
    /// </summary>
    public class SubFileName : IComparable<SubFileName>
    {
        public long RawValue1
        {
            get;
            private set;
        }

        public long RawValue2
        {
            get;
            private set;
        }

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

        public void Save(BinaryWriter writer)
        {
            writer.Write(RawValue1);
            writer.Write(RawValue2);
            writer.Write(RawValue3);
        }

        /// <summary>
        /// An empty sub file name. Should not generally be used as a single file system 
        /// must have unique file names.
        /// </summary>
        public static SubFileName Empty
        {
            get
            {
                return new SubFileName(0, 0, 0);
            }
        }

        /// <summary>
        /// Creates a random 
        /// </summary>
        /// <returns></returns>
        public static SubFileName CreateRandom()
        {
            return Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        }

        public static unsafe SubFileName Create(Guid fileType, Guid keyType, Guid valueType)
        {
            byte[] data = new byte[16 * 3];
            fixed (byte* lp = data)
            {
                *(Guid*)(lp) = fileType;
                *(Guid*)(lp + 16) = keyType;
                *(Guid*)(lp + 32) = valueType;
            }
            return Create(data);
        }

        public static SubFileName Create(long rawValue1, long rawValue2, int rawValue3)
        {
            return new SubFileName(rawValue1, rawValue2, rawValue3);
        }

        public static SubFileName Create(string name)
        {
            byte[] data = Encoding.Unicode.GetBytes(name);
            return Create(data);
        }

        public static unsafe SubFileName Create(byte[] data)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] hash = sha1.ComputeHash(data);
            fixed (byte* lp = hash)
            {
                return new SubFileName(*(long*)(lp), *(long*)(lp + 8), *(int*)(lp + 16));
            }
        }

        public static SubFileName Load(BinaryReader reader)
        {
            long value1 = reader.ReadInt64();
            long value2 = reader.ReadInt64();
            int value3 = reader.ReadInt32();
            return new SubFileName(value1, value2, value3);
        }

        public static bool operator ==(SubFileName a, SubFileName b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return (a.RawValue1 == b.RawValue1 && a.RawValue2 == b.RawValue2 && a.RawValue3 == b.RawValue3);
        }

        public static bool operator !=(SubFileName a, SubFileName b)
        {
            return !(a == b);
        }


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

        public override bool Equals(object obj)
        {
            SubFileName p = obj as SubFileName;
            if (ReferenceEquals(p, null))
                return false;
            return Equals(p);
        }

        public bool Equals(SubFileName p)
        {
            return this == p;
        }
    }
}