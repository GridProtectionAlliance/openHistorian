//******************************************************************************************************
//  HistorianValue`1.cs - Gbtc
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
using GSF.IO;
using openHistorian.Collections.Generic;

namespace openHistorian.Collections
{

    /// <summary>
    /// Base implementation of a historian value. 
    /// These are the required functions that are 
    /// necessary for the historian engine to operate
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class HistorianValueBase<TValue>
        : IEquatable<TValue>, ISortedTreeValue<TValue>
        where TValue : class, new()
    {
        /// <summary>
        /// Is the current instance equal to <see cref="other"/>
        /// </summary>
        /// <param name="other">the key to compare to</param>
        /// <returns></returns>
        public abstract bool IsEqualTo(TValue other);
        /// <summary>
        /// Copies the data from this class to <see cref="other"/>
        /// </summary>
        /// <param name="other">the destination of the copy</param>
        public abstract void CopyTo(TValue other);
        /// <summary>
        /// Serializes this value to the <see cref="stream"/> in a fixed sized method.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public abstract void Write(BinaryStreamBase stream);
        /// <summary>
        /// Reads data from the provided <see cref="stream"/> in a fixed size method.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        public abstract void Read(BinaryStreamBase stream);
        /// <summary>
        /// Serializes this value to the <see cref="stream"/> in a condensed method.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="previousValue">the previous value that was serialized</param>
        public abstract void WriteCompressed(BinaryStreamBase stream, TValue previousValue);
        /// <summary>
        /// Reads data from the provided <see cref="stream"/> in a condensed method.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <param name="previousValue">the previous value that was serialized</param>
        public abstract void ReadCompressed(BinaryStreamBase stream, TValue previousValue);

        /// <summary>
        /// Sets the value to the default values.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TValue other)
        {
            return IsEqualTo(other);
        }

        public abstract TreeValueMethodsBase<TValue> CreateValueMethods();
    }
}