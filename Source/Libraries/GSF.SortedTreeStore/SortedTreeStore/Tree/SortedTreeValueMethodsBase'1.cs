//******************************************************************************************************
//  SortedTreeValueMethodsBase`1.cs - Gbtc
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

#define GetTreeValueMethodsCallCount

using System;
using GSF.IO;
using GSF.IO.Unmanaged;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// Specifies all of the core methods that need to be implemented for a <see cref="SortedTree"/> to be able
    /// to utilize this type of value.
    /// </summary>
    /// <remarks>
    /// There are many functions that are generically implemented in this class that can be overridden
    /// for vastly superiour performance.
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    public abstract class SortedTreeValueMethodsBase<TValue>
        where TValue : class, new()
    {

        protected SortedTreeValueMethodsBase()
        {
            Size = GetSize();
        }

        /// <summary>
        /// The fixed size of this value
        /// </summary>
        public int Size
        {
            get;
            private set;
        }

        /// <summary>
        /// The Guid uniquely defining this type. 
        /// It is important to uniquely tie 1 type to 1 guid.
        /// </summary>
        public abstract Guid GenericTypeGuid
        {
            get;
        }
        /// <summary>
        /// Clears the key
        /// </summary>
        /// <param name="data"></param>
        public abstract void Clear(TValue data);
        /// <summary>
        /// Gets the size of this class when serialized
        /// </summary>
        /// <returns></returns>
        protected abstract int GetSize();
        /// <summary>
        /// Reads the key from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public abstract unsafe void Read(byte* stream, TValue data);
        /// <summary>
        /// Writes the key to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public abstract unsafe void Write(byte* stream, TValue data);

        /// <summary>
        /// Copies the source to the destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public virtual unsafe void Copy(TValue source, TValue destination)
        {
            byte* ptr = stackalloc byte[Size];
            Write(ptr, source);
            Read(ptr, destination);
        }

        /// <summary>
        /// Copies the source to the destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public virtual unsafe void Copy(byte* source, byte* destination)
        {
            Memory.Copy(source, destination, Size);
        }


        /// <summary>
        /// Gets if value == value2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public virtual unsafe bool IsEqual(TValue value, TValue value2)
        {
            byte* buffer1 = stackalloc byte[Size];
            byte* buffer2 = stackalloc byte[Size];
            Write(buffer1, value);
            Write(buffer2, value2);
            for (int x = 0; x < Size; x++)
                if (buffer1[x] != buffer2[x])
                    return false;
            return true;
        }
        
        /// <summary>
        /// Writes the value to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public virtual unsafe void Write(BinaryStreamBase stream, TValue data)
        {
            byte* ptr = stackalloc byte[Size];
            Write(ptr, data);
            stream.Write(ptr, Size);
        }

        /// <summary>
        /// reads the value from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public virtual unsafe void Read(BinaryStreamBase stream, TValue data)
        {
            byte* ptr = stackalloc byte[Size];
            stream.ReadAll(ptr, Size);
            Read(ptr, data);
        }
    }
}