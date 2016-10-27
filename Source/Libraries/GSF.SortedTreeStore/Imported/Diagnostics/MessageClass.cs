//******************************************************************************************************
//  MessageClass.cs - Gbtc
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

namespace GSF.Diagnostics
{
    /// <summary>
    /// Gets the classification of the message. 
    /// </summary>
    public enum MessageClass 
        : byte
    {
        /// <summary>
        /// Messages that come from core components. These messages are for classes that 
        /// are not working towards a specific framework or application, but can 
        /// generally be used. 
        /// </summary>
        Component = 0,
        /// <summary>
        /// Messages from higher level software components. A framework is what makes the underlying application work and 
        /// is the assimilation of components for a specific purpose. Use this if the intent is that this
        /// code will be shared among many different applications and may be implemented differently in those applications. 
        /// This is different from Component as it has a more specific and defined purpose.
        /// </summary>
        Framework = 1,
        /// <summary>
        /// These messages are for the highest layer of an application. 
        /// Messages like 'Application X is starting up, shutting down' would go here.
        /// </summary>
        Application = 2,
    }
}