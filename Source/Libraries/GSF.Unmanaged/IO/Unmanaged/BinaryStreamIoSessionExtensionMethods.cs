////******************************************************************************************************
////  BinaryStreamIoSessionExtensionMethods.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  2/22/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;

//namespace GSF.IO.Unmanaged
//{
//    public static class BinaryStreamIoSessionExtensionMethods
//    {
//        /// <summary>
//        /// Reads from the underlying stream the requested set of data. 
//        /// This function is more user friendly than calling GetBlock().
//        /// </summary>
//        /// <param name="session">The session to do the reading on</param>
//        /// <param name="position">the starting position of the read</param>
//        /// <param name="pointer">an output pointer to <see cref="position"/>.</param>
//        /// <param name="validLength">the number of bytes that are valid after this position.</param>
//        /// <returns></returns>
//        public static void ReadBlock(this IBinaryStreamIoSession session, long position, out IntPtr pointer, out int validLength)
//        {
//            long firstPosition;
//            bool supportsWriting;
//            session.GetBlock(position, false, out pointer, out firstPosition, out validLength, out supportsWriting);
//            int seekDistance = (int)(position - firstPosition);
//            validLength -= seekDistance;
//            pointer += seekDistance;
//        }
//        public static IBinaryStreamIoSession CacheReads(this IBinaryStreamIoSession session)
//        {
//            return new BinaryStreamIoSessionCache(session);
//        }
//    }
//}

