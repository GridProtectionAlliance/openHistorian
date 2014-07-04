//******************************************************************************************************
//  ServerSocketDatabase`2.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.Net;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Services.Reader;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// This is a single server socket database that is owned by a remote client.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class ServerSocketDatabase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
        
    {
        private NetworkBinaryStream m_stream;
        private ServerDatabase<TKey, TValue>.ClientDatabase m_sortedTreeEngine;
        StreamEncodingBase<TKey, TValue> m_encodingMethod;

        public ServerSocketDatabase(NetworkBinaryStream netStream, ServerDatabase<TKey, TValue>.ClientDatabase engine)
        {
            m_stream = netStream;
            m_sortedTreeEngine = engine;
            m_encodingMethod = Library.Streaming.CreateStreamEncoding<TKey, TValue>(SortedTree.FixedSizeNode);
        }

        /// <summary>
        /// This function will verify the connection, create all necessary streams, set timeouts, and catch any exceptions and terminate the connection
        /// </summary>
        /// <returns>True if successful, false if needing to exit the socket.</returns>
        public bool RunDatabaseLevel()
        {
            while (true)
            {
                ServerCommand command = (ServerCommand)m_stream.ReadUInt8();
                switch (command)
                {
                    case ServerCommand.SetEncodingMethod:
                        try
                        {
                            m_encodingMethod = Library.Streaming.CreateStreamEncoding<TKey, TValue>(new EncodingDefinition(m_stream));
                        }
                        catch
                        {
                            m_stream.Write((byte)ServerResponse.UnknownEncodingMethod);
                            m_stream.Flush();
                            return false;
                        }
                        m_stream.Write((byte)ServerResponse.EncodingMethodAccepted);
                        m_stream.Flush();
                        break;
                    case ServerCommand.Read:
                        if (!ProcessRead())
                            return false;
                        break;
                    case ServerCommand.DisconnectDatabase:
                        m_sortedTreeEngine.Dispose();
                        m_sortedTreeEngine = null;
                        m_stream.Write((byte)ServerResponse.DatabaseDisconnected);
                        m_stream.Flush();
                        return true;
                    case ServerCommand.Write:
                        ProcessWrite();
                        break;
                    case ServerCommand.CancelRead:
                        break;
                    default:
                        m_stream.Write((byte)ServerResponse.UnknownDatabaseCommand);
                        m_stream.Write((byte)command);
                        m_stream.Flush();
                        return false;
                }
            }
        }

        private bool ProcessRead()
        {
            SeekFilterBase<TKey> key1Parser = null;
            MatchFilterBase<TKey, TValue> key2Parser = null;
            SortedTreeEngineReaderOptions readerOptions = null;

            if (m_stream.ReadBoolean())
            {
                try
                {
                    key1Parser = Library.Filters.GetSeekFilter<TKey>(m_stream.ReadGuid(), m_stream);
                }
                catch
                {
                    m_stream.Write((byte)ServerResponse.UnknownOrCorruptSeekFilter);
                    m_stream.Flush();
                    return false;
                }
            }
            if (m_stream.ReadBoolean())
            {
                try
                {
                    key2Parser = Library.Filters.GetMatchFilter<TKey, TValue>(m_stream.ReadGuid(), m_stream);
                }
                catch
                {
                    m_stream.Write((byte)ServerResponse.UnknownOrCorruptMatchFilter);
                    m_stream.Flush();
                    return false;
                }
            }
            if (m_stream.ReadBoolean())
            {
                try
                {
                    readerOptions = new SortedTreeEngineReaderOptions(m_stream);
                }
                catch
                {
                    m_stream.Write((byte)ServerResponse.UnknownOrCorruptReaderOptions);
                    m_stream.Flush();
                    return false;
                }
            }

            bool needToFinishStream = false;

            try
            {
                using (TreeStream<TKey, TValue> scanner = m_sortedTreeEngine.Read(readerOptions, key1Parser, key2Parser))
                {
                    m_stream.Write((byte)ServerResponse.SerializingPoints);


                    m_encodingMethod.ResetEncoder();
                    //if (m_encodingMethod.SupportsPointerSerialization)
                    //    ProcessReadWithPointers(scanner);
                    //else

                    needToFinishStream = true;
                    bool wasCanceled = !ProcessRead(scanner);
                    m_encodingMethod.WriteEndOfStream(m_stream);
                    needToFinishStream = false;

                    if (wasCanceled)
                        m_stream.Write((byte)ServerResponse.CanceledRead);
                    else
                        m_stream.Write((byte)ServerResponse.ReadComplete);
                    m_stream.Flush();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (needToFinishStream)
                    m_encodingMethod.WriteEndOfStream(m_stream);
                m_stream.Write((byte)ServerResponse.ErrorWhileReading);
                m_stream.Write(ex.ToString());
                m_stream.Flush();
                return false;
            }
        }

        bool ProcessRead(TreeStream<TKey, TValue> scanner)
        {
            TKey key = new TKey();
            TValue value = new TValue();
            int loop = 0;
            while (scanner.Read(key, value))
            {
                m_encodingMethod.Encode(m_stream, key, value);
                loop++;
                if (loop > 1000)
                {
                    loop = 0;
                    if (m_stream.AvailableReadBytes > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //unsafe private void ProcessReadWithPointers(TreeStream<TKey, TValue> scanner)
        //{
        //    int bytesForSerialization = m_encodingMethod.MaxCompressedSize;
        //    byte[] buffer;
        //    int position;
        //    int bufferSize;
        //    int origPosition;
        //    m_stream.UnsafeGetInternalSendBuffer(out buffer, out position, out bufferSize);
        //    origPosition = position;
        //    fixed (byte* lp = buffer)
        //    {
        //        TKey key = new TKey();
        //        TValue value = new TValue();
        //        int loop = 0;
        //        while (scanner.Read(key, value))
        //        {
        //            if (bufferSize - position < bytesForSerialization)
        //            {
        //                m_stream.UnsafeAdvanceSendPosition(position - origPosition);
        //                m_stream.Flush();
        //                position = 0;
        //                origPosition = 0;
        //            }
        //            position += m_encodingMethod.Encode(lp + position, key, value);
        //            loop++;
        //            if (loop > 1000)
        //            {
        //                loop = 0;
        //                if (m_stream.AvailableReadBytes > 0)
        //                {
        //                    m_stream.UnsafeAdvanceSendPosition(position - origPosition);
        //                    return;
        //                }
        //            }
        //        }
        //    }
        //    m_stream.UnsafeAdvanceSendPosition(position - origPosition);
        //}

        private void ProcessWrite()
        {
            TKey key = new TKey();
            TValue value = new TValue();
            m_encodingMethod.ResetEncoder();
            while (m_encodingMethod.TryDecode(m_stream, key, value))
            {
                m_sortedTreeEngine.Write(key, value);
            }
        }

    }
}