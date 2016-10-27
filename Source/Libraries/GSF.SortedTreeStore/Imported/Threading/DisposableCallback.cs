//******************************************************************************************************
//  DisposableCallback.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Threading
{
    /// <summary>
    /// A struct that executes a certain action upon disposing.
    /// The intended use for this is inside of a using block to assist
    /// in the proper release of a lock.
    /// Being a struct, it is unsafe make copies of this struct
    /// as Dispose may be called multiple times.
    /// </summary>
    public struct DisposableCallback
        : IDisposable
    {
        private Action m_callback;

        /// <summary>
        /// Creates a read lock
        /// </summary>
        /// <param name="callback"></param>
        public DisposableCallback(Action callback)
        {
            m_callback = callback;
        }

        public void Dispose()
        {
            if (m_callback != null)
            {
                m_callback();
                m_callback = null;
            }
        }
    }
}