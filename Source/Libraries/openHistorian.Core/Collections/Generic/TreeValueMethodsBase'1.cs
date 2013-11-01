//******************************************************************************************************
//  TreeValueMethodsBase`1.cs - Gbtc
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

//#define GetTreeValueMethodsCallCount

using GSF.IO;

namespace openHistorian.Collections.Generic
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
    public abstract class TreeValueMethodsBase<TValue>
        : CreateValueMethodBase<TValue>
        where TValue : class, new()
    {

#if GetTreeValueMethodsCallCount
        public static void ClearStats()
        {
            CallMethods = new long[100];
        }
        static public long[] CallMethods = new long[100];
        public enum Method
            : int
        {
            Copy,
            ReadBinaryStreamBase,
            WriteBinaryStreamBase,
            IsEqual,
            Create

        }
#endif

        protected TreeValueMethodsBase()
        {
            Size = GetSize();
        }

        public int Size
        {
            get;
            private set;
        }

        protected abstract int GetSize();

        public abstract unsafe void Read(byte* stream, TValue data);

        public abstract unsafe void Write(byte* stream, TValue data);

        public virtual unsafe void Copy(TValue source, TValue destination)
        {
#if GetTreeValueMethodsCallCount
            CallMethods[(int)Method.Copy]++;
#endif
            byte* ptr = stackalloc byte[Size];
            Write(ptr, source);
            Read(ptr, destination);
        }

        public abstract void Clear(TValue data);

        public override TreeValueMethodsBase<TValue> Create()
        {
#if GetTreeValueMethodsCallCount
            CallMethods[(int)Method.Create]++;
#endif
            TreeValueMethodsBase<TValue> obj = (TreeValueMethodsBase<TValue>)MemberwiseClone();
            return obj;
        }
        
        public virtual unsafe bool IsEqual(TValue value, TValue value2)
        {
#if GetTreeValueMethodsCallCount
            CallMethods[(int)Method.IsEqual]++;
#endif
            byte* buffer1 = stackalloc byte[Size];
            byte* buffer2 = stackalloc byte[Size];
            Write(buffer1, value);
            Write(buffer2, value2);
            for (int x = 0; x < Size; x++)
                if (buffer1[x] != buffer2[x])
                    return false;
            return true;
        }

        public virtual unsafe void Write(BinaryStreamBase stream, TValue data)
        {
#if GetTreeValueMethodsCallCount
            CallMethods[(int)Method.WriteBinaryStreamBase]++;
#endif
            byte* ptr = stackalloc byte[Size];
            Write(ptr, data);
            stream.Write(ptr, Size);
        }

        public virtual unsafe void Read(BinaryStreamBase stream, TValue data)
        {
#if GetTreeValueMethodsCallCount
            CallMethods[(int)Method.ReadBinaryStreamBase]++;
#endif
            byte* ptr = stackalloc byte[Size];
            stream.Read(ptr, Size);
            Read(ptr, data);
        }
    }
}