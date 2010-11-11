//******************************************************************************************************
//  ExportSetting.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  -----------------------------------------------------------------------------------------------------
//  06/13/2007 - Pinal C. Patel
//       Original version of source code generated.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated new header and license agreement.
//
//******************************************************************************************************

using System;

namespace TimeSeriesArchiver.Exporters
{
    /// <summary>
    /// A class that can be used to add custom settings to an <see cref="Export"/>.
    /// </summary>
    /// <seealso cref="Export"/>
    [Serializable()]
    public class ExportSetting
    {
        #region [ Members ]

        // Fields
        private string m_name;
        private string m_value;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportSetting"/> class.
        /// </summary>
        public ExportSetting()
            : this("Name", "Value")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportSetting"/> class.
        /// </summary>
        /// <param name="name">Name of the <see cref="ExportSetting"/>.</param>
        /// <param name="value">Value of the <see cref="ExportSetting"/>.</param>
        public ExportSetting(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the <see cref="ExportSetting"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="ExportSetting"/>.
        /// </summary>
        public string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        #endregion
    }
}