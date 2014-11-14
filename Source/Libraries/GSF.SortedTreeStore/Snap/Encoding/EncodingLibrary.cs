//******************************************************************************************************
//  EncodingLibrary.cs - Gbtc
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
//  02/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.Snap.Definitions;

namespace GSF.Snap.Encoding
{
    /// <summary>
    /// Contains all of the fundamental encoding methods. Types implementing <see cref="SnapTypeBase{T}"/>
    /// will automatically register when passed to one of the child methods. 
    /// </summary>
    public class EncodingLibrary
    {
        private readonly IndividualEncodingDictionary<IndividualEncodingBaseDefinition> m_individualEncoding;
        private readonly CombinedEncodingDictionary<CombinedEncodingBaseDefinition> m_doubleEncoding;

        internal EncodingLibrary()
        {
            m_individualEncoding = new IndividualEncodingDictionary<IndividualEncodingBaseDefinition>();
            m_doubleEncoding = new CombinedEncodingDictionary<CombinedEncodingBaseDefinition>();
        }

        /// <summary>
        /// Registers the provided type in the encoding library.
        /// </summary>
        /// <param name="encoding">the encoding to register</param>
        internal void Register(IndividualEncodingBaseDefinition encoding)
        {
            m_individualEncoding.Register(encoding);
        }

        /// <summary>
        /// Registers the provided type in the encoding library.
        /// </summary>
        /// <param name="encoding">the encoding to register</param>
        internal void Register(CombinedEncodingBaseDefinition encoding)
        {
            m_doubleEncoding.Register(encoding);
        }

        /// <summary>
        /// Gets the single encoding method if it exists in the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encodingMethod"></param>
        /// <returns></returns>
        public IndividualEncodingBase<T> GetEncodingMethod<T>(Guid encodingMethod)
            where T : SnapTypeBase<T>, new()
        {
            IndividualEncodingBaseDefinition encoding;

            if (m_individualEncoding.TryGetEncodingMethod<T>(encodingMethod, out encoding))
                return encoding.Create<T>();

            throw new Exception("Type is not registered");
        }

        /// <summary>
        /// Gets the Double encoding method
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="encodingMethod"></param>
        /// <returns></returns>
        public CombinedEncodingBase<TKey, TValue> GetEncodingMethod<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            CombinedEncodingBaseDefinition encoding;

            if (m_doubleEncoding.TryGetEncodingMethod<TKey, TValue>(encodingMethod, out encoding))
                return encoding.Create<TKey, TValue>();

            if (encodingMethod.IsKeyValueEncoded)
                throw new Exception("Type is not registered");

            return new CombinedEncodingGeneric<TKey, TValue>(encodingMethod);
        }
    }
}