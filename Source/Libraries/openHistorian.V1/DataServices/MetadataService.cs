//******************************************************************************************************
//  MetadataService.cs - Gbtc
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
//  08/28/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/10/2009 - Pinal C. Patel
//       Modified ReadMetadata() overloads to remove try-catch and check for null reference instead.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/15/2009 - Pinal C. Patel
//       Changed the default port for the service from 5151 to 6151.
//  03/30/2010 - Pinal C. Patel
//       Added check to verify that the service is enabled before processing incoming requests.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/07/2010 - Pinal C. Patel
//       Modified to fix breaking changes made to SelfHostingService.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using openHistorian.V1.Files;
using TVA.Parsing;

namespace openHistorian.V1.DataServices
{
    /// <summary>
    /// Represents a REST web service for historian metadata.
    /// </summary>
    /// <seealso cref="SerializableMetadata"/>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MetadataService : DataService, IMetadataService
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataService"/> class.
        /// </summary>
        public MetadataService()
            : base()
        {
            Endpoints = "http.rest://localhost:6151/historian";
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Writes <paramref name="metadata"/> received in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="metadata">An <see cref="SerializableMetadata"/> object.</param>
        public void WriteMetadataAsXml(SerializableMetadata metadata)
        {
            WriteMetadata(metadata);
        }

        /// <summary>
        /// Writes <paramref name="metadata"/> received in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="metadata">An <see cref="SerializableMetadata"/> object.</param>
        public void WriteMetadataAsJson(SerializableMetadata metadata)
        {
            WriteMetadata(metadata);
        }

        /// <summary>
        /// Reads all metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadAllMetadataAsXml()
        {
            return ReadMetadata();
        }

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadSelectMetadataAsXml(string idList)
        {
            return ReadMetadata(idList);
        }

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which metadata is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadRangeMetadataAsXml(string fromID, string toID)
        {
            return ReadMetadata(fromID, toID);
        }

        /// <summary>
        /// Reads all metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadAllMetadataAsJson()
        {
            return ReadMetadata();
        }

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadSelectMetadataAsJson(string idList)
        {
            return ReadMetadata(idList);
        }

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="System.ServiceModel.Web.WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which metadata is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadRangeMetadataAsJson(string fromID, string toID)
        {
            return ReadMetadata(fromID, toID);
        }

        private void WriteMetadata(SerializableMetadata metadata)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Write all metadata records to the archive.
                foreach (SerializableMetadataRecord record in metadata.MetadataRecords)
                {
                    Archive.WriteMetaData(record.HistorianID, record.Deflate().BinaryImage());
                }
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableMetadata ReadMetadata()
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read all metadata records from the archive.
                int id = 0;
                byte[] buffer = null;
                SerializableMetadata metadata = new SerializableMetadata();
                List<SerializableMetadataRecord> records = new List<SerializableMetadataRecord>();
                while (true)
                {
                    buffer = Archive.ReadMetaData(++id);
                    if (buffer == null)
                        // No more records.
                        break;
                    else
                        // Add to resultset.
                        records.Add(new SerializableMetadataRecord(new MetadataRecord(id, buffer, 0, buffer.Length)));
                }
                metadata.MetadataRecords = records.ToArray();

                return metadata;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableMetadata ReadMetadata(string idList)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read specified metadata records from the archive.
                int id = 0;
                byte[] buffer = null;
                SerializableMetadata metadata = new SerializableMetadata();
                List<SerializableMetadataRecord> records = new List<SerializableMetadataRecord>();
                foreach (string singleID in idList.Split(',', ';'))
                {
                    buffer = Archive.ReadMetaData(id = int.Parse(singleID));
                    if (buffer == null)
                        // ID is invalid.
                        continue;
                    else
                        // Add to resultset.
                        records.Add(new SerializableMetadataRecord(new MetadataRecord(id, buffer, 0, buffer.Length)));
                }
                metadata.MetadataRecords = records.ToArray();

                return metadata;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private SerializableMetadata ReadMetadata(string fromID, string toID)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read specified metadata records from the archive.
                byte[] buffer = null;
                SerializableMetadata metadata = new SerializableMetadata();
                List<SerializableMetadataRecord> records = new List<SerializableMetadataRecord>();
                for (int id = int.Parse(fromID); id <= int.Parse(toID); id++)
                {
                    buffer = Archive.ReadMetaData(id);
                    if (buffer == null)
                        // ID is invalid.
                        continue;
                    else
                        // Add to resultset.
                        records.Add(new SerializableMetadataRecord(new MetadataRecord(id, buffer, 0, buffer.Length)));
                }
                metadata.MetadataRecords = records.ToArray();

                return metadata;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        #endregion
    }
}
