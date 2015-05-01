//******************************************************************************************************
//  EventArgs.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  11/12/2004 - J. Ritchie Carroll
//       Initial version of source generated.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  05/11/2011 - J. Ritchie Carroll
//       Marked classes as serializable.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

#region [ Contributor License Agreements ]

/**************************************************************************\
   Copyright © 2009 - J. Ritchie Carroll
   All rights reserved.
  
   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:
  
      * Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.
       
      * Redistributions in binary form must reproduce the above
        copyright notice, this list of conditions and the following
        disclaimer in the documentation and/or other materials provided
        with the distribution.
  
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY
   EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
   IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
   PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
   OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  
\**************************************************************************/

#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace GSF
{
    /// <summary>
    /// Represents a generic event arguments class with one data argument.
    /// </summary>
    /// <typeparam name="T">Type of data argument for this event arguments instance.</typeparam>
    [Serializable]
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the data argument for the event using <see cref="EventArgs{T}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T Argument;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        public EventArgs()
            : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="argument">The argument data for the event.</param>
        public EventArgs(T argument)
        {
            Argument = argument;
        }
    }

    /// <summary>
    /// Represents a generic event arguments class with two data arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first data argument for this event arguments instance.</typeparam>
    /// <typeparam name="T2">The type of the second data argument for this event arguments instance.</typeparam>
    [Serializable]
    public class EventArgs<T1, T2> : EventArgs
    {
        /// <summary>
        /// Gets or sets the first data argument for the event using <see cref="EventArgs{T1,T2}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T1 Argument1;

        /// <summary>
        /// Gets or sets the second data argument for the event using <see cref="EventArgs{T1,T2}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T2 Argument2;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1,T2}"/> class.
        /// </summary>
        public EventArgs()
            : this(default(T1), default(T2))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1,T2}"/> class.
        /// </summary>
        /// <param name="argument1">The first data argument for the event.</param>
        /// <param name="argument2">The second data argument for the event.</param>
        public EventArgs(T1 argument1, T2 argument2)
        {
            Argument1 = argument1;
            Argument2 = argument2;
        }
    }

    /// <summary>
    /// Represents a generic event arguments class with three data arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first data argument for this event arguments instance.</typeparam>
    /// <typeparam name="T2">The type of the second data argument for this event arguments instance.</typeparam>
    /// <typeparam name="T3">The type of the third data argument for this event arguments instance.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes"), Serializable]
    public class EventArgs<T1, T2, T3> : EventArgs
    {
        /// <summary>
        /// Gets or sets the first data argument for the event using <see cref="EventArgs{T1,T2,T3}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T1 Argument1;

        /// <summary>
        /// Gets or sets the second data argument for the event using <see cref="EventArgs{T1,T2,T3}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T2 Argument2;

        /// <summary>
        /// Gets or sets the third data argument for the event using <see cref="EventArgs{T1,T2,T3}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T3 Argument3;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1,T2,T3}"/> class.
        /// </summary>
        public EventArgs()
            : this(default(T1), default(T2), default(T3))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1,T2,T3}"/> class.
        /// </summary>
        /// <param name="argument1">The first data argument for the event.</param>
        /// <param name="argument2">The second data argument for the event.</param>
        /// <param name="argument3">The third data argument for the event.</param>
        public EventArgs(T1 argument1, T2 argument2, T3 argument3)
        {
            Argument1 = argument1;
            Argument2 = argument2;
            Argument3 = argument3;
        }
    }

    /// <summary>
    /// Represents a generic event arguments class with three data arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first data argument for this event arguments instance.</typeparam>
    /// <typeparam name="T2">The type of the second data argument for this event arguments instance.</typeparam>
    /// <typeparam name="T3">The type of the third data argument for this event arguments instance.</typeparam>
    /// <typeparam name="T4">The type of the fourth data argument for this event arguments instance.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes"), Serializable]
    public class EventArgs<T1, T2, T3, T4> : EventArgs
    {
        /// <summary>
        /// Gets or sets the first data argument for the event using <see cref="EventArgs{T1,T2,T3,T4}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T1 Argument1;

        /// <summary>
        /// Gets or sets the second data argument for the event using <see cref="EventArgs{T1,T2,T3,T4}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T2 Argument2;

        /// <summary>
        /// Gets or sets the third data argument for the event using <see cref="EventArgs{T1,T2,T3,T4}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T3 Argument3;

        /// <summary>
        /// Gets or sets the fourth data argument for the event using <see cref="EventArgs{T1,T2,T3,T4}"/> for its <see cref="EventArgs"/>.
        /// </summary>
        public T4 Argument4;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1,T2,T3,T4}"/> class.
        /// </summary>
        public EventArgs()
            : this(default(T1), default(T2), default(T3), default(T4))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T1,T2,T3}"/> class.
        /// </summary>
        /// <param name="argument1">The first data argument for the event.</param>
        /// <param name="argument2">The second data argument for the event.</param>
        /// <param name="argument3">The third data argument for the event.</param>
        /// <param name="argument4">The fourth data argument for the event.</param>
        public EventArgs(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
        {
            Argument1 = argument1;
            Argument2 = argument2;
            Argument3 = argument3;
            Argument4 = argument4;
        }
    }
}