//******************************************************************************************************
//  UnixTimeTag.cs - Gbtc
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
using System.Runtime.Serialization;

namespace GSF
{
    /// <summary>
    /// Represents a standard Unix timetag.
    /// </summary>
    [Serializable]
    public class UnixTimeTag : TimeTagBase
    {
        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="UnixTimeTag"/>, given number of seconds since 1/1/1970.
        /// </summary>
        /// <param name="seconds">Number of seconds since 1/1/1970.</param>
        public UnixTimeTag(double seconds)
            : base(BaseTicks, seconds)
        {
        }

        /// <summary>
        /// Creates a new <see cref="UnixTimeTag"/>, given number of seconds since 1/1/1970.
        /// </summary>
        /// <param name="seconds">Number of seconds since 1/1/1970.</param>
        public UnixTimeTag(uint seconds)
            : base(BaseTicks, (double)seconds)
        {
        }

        /// <summary>
        /// Creates a new <see cref="UnixTimeTag"/>, given specified <see cref="Ticks"/>.
        /// </summary>
        /// <param name="timestamp">Timestamp in <see cref="Ticks"/> to create Unix timetag from (minimum valid date is 1/1/1970).</param>
        /// <remarks>
        /// This constructor will accept a <see cref="DateTime"/> parameter since <see cref="Ticks"/> is implicitly castable to a <see cref="DateTime"/>.
        /// </remarks>
        public UnixTimeTag(Ticks timestamp)
            : base(BaseTicks, timestamp)
        {
        }

        /// <summary>
        /// Creates a new <see cref="UnixTimeTag"/> from serialization parameters.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> with populated with data.</param>
        /// <param name="context">The source <see cref="StreamingContext"/> for this deserialization.</param>
        protected UnixTimeTag(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Number of ticks since 1/1/1970.
        /// </summary>
        /// <remarks>
        /// Unix dates are measured as the number of seconds since 1/1/1970.
        /// </remarks>
        public static readonly Ticks BaseTicks = (new DateTime(1970, 1, 1, 0, 0, 0)).Ticks;

        #endregion
    }
}