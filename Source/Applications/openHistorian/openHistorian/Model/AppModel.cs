//******************************************************************************************************
//  AppModel.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/21/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Text;
using GSF.Data.Model;
using GSF.Web;
using GSF.Web.Model;

namespace openHistorian.Model
{
    /// <summary>
    /// Defines a base application model with convenient global settings and functions.
    /// </summary>
    /// <remarks>
    /// Custom view models should inherit from AppModel because the "Global" property is used by Layout.cshtml.
    /// </remarks>
    public class AppModel
    {
        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="AppModel"/>.
        /// </summary>
        public AppModel()
        {
            Global = (object)Program.Host.Model != null ? Program.Host.Model.Global : new GlobalSettings();
        }

        #endregion
        
        #region [ Properties ]

        /// <summary>
        /// Gets global settings for application.
        /// </summary>
        public GlobalSettings Global
        {
            get;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Renders client-side Javascript function for looking up single values from a table.
        /// </summary>
        /// <param name="dataContext">DataContext object to use for database operations.</param>
        /// <param name="valueFieldName">Table field name as defined in the table.</param>
        /// <param name="idFieldName">Name of primary key field, defaults to "ID".</param>
        /// <param name="lookupFunctionName">Name of lookup function, defaults to lookup + <paramref name="valueFieldName"/> + Value.</param>
        /// <param name="arrayName">Name of lookup function, defaults to lookup + <paramref name="valueFieldName"/> + Value.</param>
        /// <returns>Client-side Javascript lookup function.</returns>
        public string RenderLookupTable<T>(DataContext dataContext, string valueFieldName, string idFieldName = "ID", string lookupFunctionName = null, string arrayName = null) where T : class, new()
        {
            StringBuilder javascript = new StringBuilder();

            if (string.IsNullOrWhiteSpace(lookupFunctionName))
                lookupFunctionName = $"lookup{valueFieldName}Value";

            if (string.IsNullOrWhiteSpace(arrayName))
                arrayName = $"{valueFieldName}";

            TableOperations<T> operations = dataContext.Table<T>();

            javascript.AppendLine($"var {arrayName} = [];\r\n");

            foreach (T record in operations.QueryRecords())
            {
                var valueField = operations.GetFieldValue(record, valueFieldName);
                var idField = operations.GetFieldValue(record, idFieldName);

                javascript.AppendLine($"        {arrayName}[{idField.ToString().JavaScriptEncode()}] = \"{valueField?.ToString().JavaScriptEncode()}\";");
            }

            javascript.AppendLine($"\r\n        function {lookupFunctionName}(value) {{");
            javascript.AppendLine($"            return {arrayName}[value];");
            javascript.AppendLine("        }");

            return javascript.ToString();
        }

        #endregion
    }
}
