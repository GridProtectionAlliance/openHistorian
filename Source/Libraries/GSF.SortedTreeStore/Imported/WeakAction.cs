//******************************************************************************************************
//  WeakActionFast.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a weak referenced action delegate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike many implementations of a weak action, this class does not
    /// use reflection so calls will be faster. However, a strong reference
    /// *must* be maintained for the <see cref="Action"/> callback passed
    /// to this class. This reference is returned through an out parameter
    /// in the constructor.
    /// </para>
    /// <para>
    /// Careful consideration must be made when deciding where to store
    /// the strong reference as this strong reference will need to also
    /// lose reference. A good place would be as a member variable of the
    /// object of the target method.
    /// </para>
    /// </remarks>
    [Serializable]
    public class WeakAction : WeakReference
    {
        /// <summary>
        /// Creates a high speed weak action.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="localStrongReference">A strong reference that must be maintained in the class that is the target of the action.</param>
        public WeakAction(Action callback, out object localStrongReference)
            : base(callback)
        {
            localStrongReference = callback;
        }

        /// <summary>
        /// Attempts to invoke the delegate to a weak reference object.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool TryInvoke()
        {
            Action target = Target as Action;

            if ((object)target == null)
                return false;

            target();

            return true;
        }

        /// <summary>
        /// Clears <see cref="Action"/> callback target.
        /// </summary>
        public void Clear()
        {
            Target = null;
        }
    }

    /// <summary>
    /// Provides a weak referenced action delegate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike many implementations of a weak action, this class does not
    /// use reflection so calls will be faster. However, a strong reference
    /// *must* be maintained for the <see cref="Action"/> callback passed
    /// to this class. This reference is returned through an out parameter
    /// in the constructor.
    /// </para>
    /// <para>
    /// Careful consideration must be made when deciding where to store
    /// the strong reference as this strong reference will need to also
    /// lose reference. A good place would be as a member variable of the
    /// object of the target method.
    /// </para>
    /// </remarks>
    [Serializable]
    public class WeakAction<T> : WeakReference
    {
        /// <summary>
        /// Creates a high speed weak action 
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="localStrongReference">A strong reference that must be maintained in the class that is the target of the action.</param>
        public WeakAction(Action<T> callback, out object localStrongReference)
            : base(callback)
        {
            localStrongReference = callback;
        }

        /// <summary>
        /// Attempts to invoke the delegate to a weak reference object.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool TryInvoke(T args)
        {
            Action<T> target = Target as Action<T>;

            if ((object)target == null)
                return false;

            target(args);

            return true;
        }

        /// <summary>
        /// Clears <see cref="Action"/> callback target.
        /// </summary>
        public void Clear()
        {
            Target = null;
        }
    }
}