//******************************************************************************************************
//  StreamEncoding.cs - Gbtc
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
//  08/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using GSF.Snap.Tree;

namespace GSF.Snap.Encoding
{
    /// <summary>
    /// A set of stream based encoding method. 
    /// </summary>
    public class StreamEncoding
    {
        private readonly DualEncodingDictionary<CreateStreamEncodingBase> m_doubleEncoding;

        //static Dictionary<Type, SortedTreeValueMethodsBase> s_valueMethods;

        internal StreamEncoding()
        {
            m_doubleEncoding = new DualEncodingDictionary<CreateStreamEncodingBase>();
        }

        /// <summary>
        /// Registers the provided type in the encoding library.
        /// </summary>
        /// <param name="encoding">the encoding to register</param>
        internal void Register(CreateStreamEncodingBase encoding)
        {
            m_doubleEncoding.Register(encoding);
        }

        /// <summary>
        /// Creates a stream encoding from the provided <see cref="encodingMethod"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="encodingMethod">the encoding method</param>
        /// <returns></returns>
        internal StreamEncodingBase<TKey, TValue> CreateStreamEncoding<TKey, TValue>(EncodingDefinition encodingMethod)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            CreateStreamEncodingBase encoding;

            if (m_doubleEncoding.TryGetEncodingMethod<TKey, TValue>(encodingMethod, out encoding))
                return encoding.Create<TKey, TValue>();
            
            return new GenericStreamEncoding<TKey, TValue>(encodingMethod);
        }
       
    }
}