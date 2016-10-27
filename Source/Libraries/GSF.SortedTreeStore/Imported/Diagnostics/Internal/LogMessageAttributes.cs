//******************************************************************************************************
//  LogMessageAttributes.cs - Gbtc
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
using System.IO;
using GSF.IO;

namespace GSF.Diagnostics
{
    /// <summary>
    /// All of the attributes that can be assigned to a <see cref="LogMessage"/>.
    /// </summary>
    internal struct LogMessageAttributes
    {
        public readonly MessageClass Classification;
        public readonly MessageLevel Level;
        public readonly MessageSuppression MessageSuppression;
        public readonly MessageFlags Flags;

        public LogMessageAttributes(MessageClass classification, MessageLevel level, MessageSuppression messageSuppression, MessageFlags flags)
        {
            Classification = classification;
            Level = level;
            MessageSuppression = messageSuppression;
            Flags = flags;
        }

        public LogMessageAttributes(Stream stream)
        {
            Classification = (MessageClass)stream.ReadByte();
            Level = (MessageLevel)stream.ReadByte();
            MessageSuppression = (MessageSuppression)stream.ReadByte();
            Flags = (MessageFlags)stream.ReadByte();
        }

        public void Save(Stream stream)
        {
            stream.Write((byte)Classification);
            stream.Write((byte)Level);
            stream.Write((byte)MessageSuppression);
            stream.Write((byte)Flags);
        }

        public static LogMessageAttributes operator +(LogMessageAttributes a, MessageSuppression b)
        {
            return new LogMessageAttributes(a.Classification, a.Level, b, a.Flags);
        }
    }
}