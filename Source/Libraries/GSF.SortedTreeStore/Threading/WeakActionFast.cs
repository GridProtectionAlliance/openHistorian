//******************************************************************************************************
//  WeakActionUnsafe.cs - Gbtc
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
//  1/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a high speed weak referenced action delegate.
    /// This one does not use reflection, so calls will be faster.
    /// 
    /// HOWEVER: a strong reference MUST be maintained for 
    /// the <see cref="Action"/> delegate passed to this class.
    /// 
    /// Careful consideration must be made when deciding 
    /// where to store this strong reference, as this strong reference
    /// will need to also lose reference. A good place would be 
    /// as a member variable of the object of the target method.
    /// </summary>
    public class WeakActionFast 
        : WeakReference
    {

        /// <summary>
        /// Creates a high speed weak action 
        /// </summary>
        /// <param name="target"></param>
        public WeakActionFast(Action target)
            : base(target)
        {

        }

        /// <summary>
        /// Attempts to invoke the delegate to a weak reference object.
        /// </summary>
        /// <returns>True if successful, false if not</returns>
        public bool TryInvoke()
        {
            Action target = (Action)Target;
            if (target == null)
                return false;
            target();
            return true;
        }
    }
}