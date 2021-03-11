//******************************************************************************************************
//  NetworkBinaryStreamTest.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
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
using System.Net;
using System.Net.Sockets;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.Net
{
    [TestFixture]
    class NetworkBinaryStreamTest
    {
        [Test]
        public void Test1()
        {
            MemoryPoolTest.TestMemoryLeak();
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42134));
            listener.Start();
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42134));
            TcpClient server = listener.AcceptTcpClient();

            NetworkBinaryStream c = new NetworkBinaryStream(client.Client);
            NetworkBinaryStream s = new NetworkBinaryStream(server.Client);

            Random r = new Random();
            int seed = r.Next();
            Random sr = new Random(seed);
            Random cr = new Random(seed);

            for (int x = 0; x < 2000; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    int val = sr.Next();
                    c.Write(val);
                    c.Write((byte)val);
                }
                c.Flush();
                for (int y = 0; y < x; y++)
                {
                    int val = cr.Next();
                    if (val != s.ReadInt32())
                        throw new Exception("Error");
                    if ((byte)val != s.ReadUInt8())
                        throw new Exception("Error");
                }
            }
            
            server.Close();
            client.Close();
            listener.Stop();
            MemoryPoolTest.TestMemoryLeak();

        }
    }
}
