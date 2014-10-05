//******************************************************************************************************
//  WriteProcessorSettings.cs - Gbtc
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
//  10/03/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************


namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// The settings for the Write Processor
    /// </summary>
    public class WriteProcessorSettings
    {
        /// <summary>
        /// The default write processor settings
        /// </summary>
        public WriteProcessorSettings()
        {
            PrebufferWriter = new PrebufferWriterSettings();
            FirstStageWriter = new FirstStageWriterSettings();
            Stage1Rollover = new CombineFilesSettings();
            Stage2Rollover = new CombineFilesSettings();
        }
        
        /// <summary>
        /// The settings for the prebuffer.
        /// </summary>
        public PrebufferWriterSettings PrebufferWriter;

        /// <summary>
        /// The settings for the first stage writer.
        /// </summary>
        public FirstStageWriterSettings FirstStageWriter;

        /// <summary>
        /// Rolls over Stage 1 files into Stage 2 files
        /// </summary>
        public CombineFilesSettings Stage1Rollover;

        /// <summary>
        /// Rolls over Stage 2 files into Stage 3 files
        /// </summary>
        public CombineFilesSettings Stage2Rollover;

        /// <summary>
        /// Creates a clone of this class.
        /// </summary>
        /// <returns></returns>
        public WriteProcessorSettings Clone()
        {
            var obj = (WriteProcessorSettings)MemberwiseClone();
            obj.PrebufferWriter = PrebufferWriter.Clone();
            obj.FirstStageWriter = FirstStageWriter.Clone();
            obj.Stage1Rollover = Stage1Rollover.Clone();
            obj.Stage2Rollover = Stage2Rollover.Clone();
            return obj;
        }

    }
}