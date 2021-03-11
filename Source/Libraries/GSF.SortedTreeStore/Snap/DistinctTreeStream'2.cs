//******************************************************************************************************
//  DistinctTreeStream'2.cs - Gbtc
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
//  09/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Snap
{
    /// <summary>
    /// Ensures that a stream is distinct (Never repeats a value).
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DistinctTreeStream<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private bool m_isLastValueValid;
        private readonly TKey m_lastKey;
        private readonly TValue m_lastValue;

        private readonly TreeStream<TKey, TValue> m_baseStream;

        /// <summary>
        /// Creates a <see cref="DistinctTreeStream{TKey,Value}"/>
        /// </summary>
        /// <param name="baseStream">Must be sequential</param>
        public DistinctTreeStream(TreeStream<TKey, TValue> baseStream)
        {
            if (!baseStream.IsAlwaysSequential)
                throw new ArgumentException("Must be sequential access", "baseStream");

            m_lastKey = new TKey();
            m_lastValue = new TValue();
            m_isLastValueValid = false;
            m_baseStream = baseStream;
        }

        public override bool IsAlwaysSequential => true;

        public override bool NeverContainsDuplicates => true;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                m_baseStream.Dispose();
            base.Dispose(disposing);
        }

        protected override void EndOfStreamReached()
        {

        }

        protected override bool ReadNext(TKey key, TValue value)
        {
        TryAgain:
            if (!m_baseStream.Read(key, value))
                return false;
            if (m_isLastValueValid && key.IsEqualTo(m_lastKey))
                goto TryAgain;
            m_isLastValueValid = true;
            key.CopyTo(m_lastKey);
            value.CopyTo(m_lastValue);
            return true;
        }
    }
}
