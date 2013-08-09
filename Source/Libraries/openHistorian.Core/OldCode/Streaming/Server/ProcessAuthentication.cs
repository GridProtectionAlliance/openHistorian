////******************************************************************************************************
////  ProcessAuthentication.cs - Gbtc
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
////  5/18/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;

//namespace openHistorian.Streaming.Server
//{
//    /// <summary>
//    /// Handles the authentication of the client and grants/revokes permissions accordingly
//    /// </summary>
//    class ProcessAuthentication : ITransportCommand
//    {
//        Guid m_applicationId;
//        ITransportHost m_host;
//        bool m_connected;
//        bool m_authenticated;

//        public ProcessAuthentication(ITransportHost host, Guid applicationId)
//        {
//            m_authenticated = false;
//            m_applicationId = applicationId;
//            m_host = host;
//        }

//        public byte CommandCode
//        {
//            get
//            {
//                return 2;
//            }
//        }

//        public void Execute()
//        {
//            m_connected = true;
//            while (m_connected)
//            {
//                switch (m_host.Receive.ReadByte())
//                {
//                    case 0: //exit to root
//                        return;
//                    case 1: //authenticate
//                        Authenticate();
//                        break;
//                    default: //unknown command
//                        m_host.Terminate();
//                        return;
//                }
//            }
//        }

//        void Authenticate()
//        {
//            if (m_host.Receive.ReadGuid() != m_applicationId)
//            {
//                m_host.Send.Write((byte)0); //Wrong Application
//                m_host.Send.Flush();
//                m_host.Terminate();
//                m_connected = false;
//                return;
//            }
//            byte[] clientChallenge = new byte[20];
//            m_host.Receive.Read(clientChallenge, 0, 20);
//            m_host.Send.Write((byte)1); //authentication successful

//            if (m_authenticated)
//            {
//                m_host.Send.Write((byte)0); //RevokeCount
//                m_host.Send.Write((byte)0); //GrantCount
//                m_host.Send.Flush();
//            }
//            else
//            {

//                ITransportCommand command2 = new ProcessDatabaseInstance(m_host);
//                m_host.CommandAdd(command2);

//                ITransportCommand command1 = new ProcessArchivePoints(m_host, (ProcessDatabaseInstance)command2);
//                m_host.CommandAdd(command1);


//                m_host.Send.Write((byte)0); //RevokeCount
//                m_host.Send.Write((byte)2); //GrantCount
//                m_host.Send.Write(command1.CommandCode); //GrantCount
//                m_host.Send.Write(command2.CommandCode); //GrantCount
//                m_host.Send.Flush();
//            }

//        }

//    }
//}

