//******************************************************************************************************
//  IExporter.cs - Gbtc
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
//  06/12/2007 - Pinal C. Patel
//       Original version of source code generated.
//  06/05/2008 - Pinal C. Patel
//       Modified the ExportProcessException event definition.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using TVA;

namespace TimeSeriesArchiver.Exporters
{
    /// <summary>
    /// Defines an exporter of real-time time-series data.
    /// </summary>
    /// <seealso cref="Export"/>
    /// <seealso cref="DataListener"/>
    public interface IExporter : IDisposable
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when the exporter want to provide a status update.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the status update message.
        /// </remarks>
        event EventHandler<EventArgs<string>> StatusUpdate;

        /// <summary>
        /// Occurs when the exporter finishes processing an <see cref="Export"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="Export"/> that the exporter finished processing.
        /// </remarks>
        event EventHandler<EventArgs<Export>> ExportProcessed;

        /// <summary>
        /// Occurs when the exporter fails to process an <see cref="Export"/> due to an <see cref="Exception"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the <see cref="Export"/> that the exporter failed to process.
        /// </remarks>
        event EventHandler<EventArgs<Export>> ExportProcessException;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name of the exporter.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="Export"/>s associated with the exporter.
        /// </summary>
        IList<Export> Exports { get; }

        /// <summary>
        /// Gets the <see cref="DataListener"/>s providing real-time time-series data to the exporter.
        /// </summary>
        IList<DataListener> Listeners { get; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Causes the <see cref="Export"/> with the specified <paramref name="exportName"/> to be processed.
        /// </summary>
        /// <param name="exportName"><see cref="Export.Name"/> of the <see cref="Export"/> to be processed.</param>
        void ProcessExport(string exportName);

        #endregion
    }
}
