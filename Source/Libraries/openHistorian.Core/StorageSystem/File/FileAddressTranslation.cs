//******************************************************************************************************
//  FileAddressTranslation.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  12/13/2011 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    /// This class manages the necessary functions in order to convert physical addresses into virtual addresses.
    /// </summary>
    internal class FileAddressTranslation
    {
        #region [ Members ]

        /// <summary>
        /// The address parser
        /// </summary>
        IndexParser m_parser;
        /// <summary>
        /// The shadow copier if the address translation allows for editing.
        /// </summary>
        ShadowCopyAllocator m_pager;
        /// <summary>
        /// Determines if the class was opened in read only mode.
        /// </summary>
        bool m_isReadOnly;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="FileAddressTranslation"/>
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="dataReader">Disk Reader</param>
        /// <param name="fileAllocationTable">FileAllocationTable</param>
        /// <param name="openReadOnly">Determines if the file will be opened allowing shadow copies or not.</param>
        public FileAddressTranslation(FileMetaData file, DiskIoEnhanced dataReader, FileAllocationTable fileAllocationTable, bool openReadOnly)
        {
            m_isReadOnly = openReadOnly;
            m_parser = new IndexParser(fileAllocationTable.SnapshotSequenceNumber, dataReader, file);

            if (!IsReadOnly)
            {
                m_pager = new ShadowCopyAllocator(dataReader, fileAllocationTable, file, m_parser);
            }

        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Contains the buffer that should be used for realtime data reads.
        /// </summary>
        internal IndexBufferPool.Buffer DataBuffer
        {
            get
            {
                return m_parser.BufferPool.Data;
            }
        }

        /// <summary>
        /// Determines if the class was opened in read only mode.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Converts a virtual address to a physical one.
        /// </summary>
        /// <param name="virtualPos"></param>
        /// <returns></returns>
        public PositionData VirtualToPhysical(long virtualPos)
        {
            return m_parser.GetPositionData(virtualPos);
        }

        /// <summary>
        /// Converts a virtual address to a physical one. Makes a shadow copy of the block or creates one if it does not exist.
        /// </summary>
        /// <param name="virtualPos"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown if the file is opened in readonly mode.</exception>
        public PositionData VirtualToShadowPagePhysical(long virtualPos)
        {
            if (IsReadOnly)
                throw new Exception("File is opened in readonly mode. Shadow blocks are not allowed.");
            m_pager.ShadowDataBlock(virtualPos);
            //m_parser.SetPosition(virtualPos);
            //m_parser.UpdateAddressesFromShadowCopy();
            return m_parser.GetPositionData(virtualPos);
        }

        #endregion
    }
}
