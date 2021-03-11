//******************************************************************************************************
//  WeakAction.cs - Gbtc
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
//  1/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Threading
{
#if !SQLCLR
    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakAction : WeakDelegateBase<Action>
    {
        public WeakAction(Action target)
            : base(target)
        {
        }

        /// <summary>
        /// Attempts to invoke the delegate to a weak reference object.
        /// </summary>
        /// <returns>True if successful, false if not</returns>
        public bool TryInvoke()
        {
            return TryInvokeInternal(null);
        }
    }

    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakAction<T> : WeakDelegateBase<Action<T>>
    {
        public WeakAction(Action<T> target)
            : base(target)
        {
        }

        public bool TryInvoke(T obj)
        {
            return TryInvokeInternal(new object[] {obj});
        }
    }
#endif

    /// <summary>
    /// Provides a weak referenced action delegate. 
    /// </summary>
    public class WeakEventHandler<T> : WeakDelegateBase<EventHandler<T>>
        where T : EventArgs
    {
        public WeakEventHandler(EventHandler<T> target)
            : base(target)
        {
        }

        public bool TryInvoke(object sender, T e)
        {
            return TryInvokeInternal(new object[] {sender, e});
        }
    }
}