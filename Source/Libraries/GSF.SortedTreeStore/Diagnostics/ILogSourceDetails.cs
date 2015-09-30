//******************************************************************************************************
//  ILogSourceDetails.cs - Gbtc
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
//  05/24/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

namespace GSF.Diagnostics
{
    /// <summary>
    /// An optional interface that the source of a <see cref="LogSource"/> can implement to supply 
    /// additional information about the log event.
    /// </summary>
    public interface ILogSourceDetails
    {
        /// <summary>
        /// Gets details associated with this source so they can be logged with the log message.
        /// </summary>
        /// <returns>The string representation of the details.</returns>
        string GetSourceDetails();
    }
}
