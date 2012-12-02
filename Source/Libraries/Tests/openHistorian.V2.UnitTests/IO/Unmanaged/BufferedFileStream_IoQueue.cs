using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using MemS = System.IO.MemoryStream;

namespace openHistorian.V2.IO.Unmanaged.Test
{
    [TestFixture()]
    public unsafe class BufferedFileStream_IoQueue
    {
        class MemoryFS : FileStream
        {
            MemS ms;
            long m_position;
            string MyName;
            public MemoryFS() :
                base(Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp", FileMode.OpenOrCreate)
            {
                ms = new MemS();
                MyName = Name;
            }

            public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
            {
                return ms.BeginRead(array, offset, numBytes, userCallback, stateObject);
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                return ms.EndRead(asyncResult);
            }

            public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
            {
                return ms.BeginWrite(array, offset, numBytes, userCallback, stateObject);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                ms.EndWrite(asyncResult);
            }

            public override long Position
            {
                get
                {
                    return ms.Position;
                }
                set
                {
                    ms.Position = value;
                }
            }

            public override void Write(byte[] array, int offset, int count)
            {
                ms.Write(array, offset, count);
            }
            public override bool CanRead
            {
                get
                {
                    return ms.CanRead;
                }
            }
            public override bool CanSeek
            {
                get
                {
                    return ms.CanSeek;
                }
            }
            public override bool CanTimeout
            {
                get
                {
                    return ms.CanTimeout;
                }
            }
            public override bool CanWrite
            {
                get
                {
                    return ms.CanWrite;
                }
            }

            public override void Close()
            {
                base.Close();
            }

            public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
            {
                return ms.CreateObjRef(requestedType);
            }
            protected override System.Threading.WaitHandle CreateWaitHandle()
            {
                throw new NotImplementedException();
            }
            public override bool Equals(object obj)
            {
                return ms.Equals(obj);
            }
            public override void Flush()
            {
                ms.Flush();
            }
            public override void Flush(bool flushToDisk)
            {
                ms.Flush();
            }
            public override int GetHashCode()
            {
                return ms.GetHashCode();
            }
            public override IntPtr Handle
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            public override object InitializeLifetimeService()
            {
                return ms.InitializeLifetimeService();
            }
            public override bool IsAsync
            {
                get
                {
                    return true;
                }
            }
            public override long Length
            {
                get
                {
                    return ms.Length;
                }
            }

            public override void Lock(long position, long length)
            {
                throw new NotImplementedException();

            }

            protected override void ObjectInvariant()
            {
                throw new NotImplementedException();

            }

            public override int Read(byte[] array, int offset, int count)
            {
                return ms.Read(array, offset, count);
            }

            public override int ReadByte()
            {
                return ms.ReadByte();
            }

            public override int ReadTimeout
            {
                get
                {
                    return ms.ReadTimeout;
                }
                set
                {
                    ms.ReadTimeout = value;
                }
            }

            public override Microsoft.Win32.SafeHandles.SafeFileHandle SafeFileHandle
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return ms.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                ms.SetLength(value);
            }

            public override string ToString()
            {
                return ms.ToString();
            }

            public override void Unlock(long position, long length)
            {
                throw new NotImplementedException();
            }

            public override void WriteByte(byte value)
            {
                ms.WriteByte(value);
            }

            public override int WriteTimeout
            {
                get
                {
                    return ms.WriteTimeout;
                }
                set
                {
                    ms.WriteTimeout = value;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    ms.Dispose();
                }
                base.Dispose(disposing);
                File.Delete(MyName);
            }

        }

        [Test()]
        public void Constructor()
        {
            using (var fs = new MemoryFS())
            {
                var io = new BufferedFileStream.IoQueue(fs, 65536, 4096, null);
                byte[] tmpForward = new byte[65536];
                for (int x = 0; x < tmpForward.Length; x++)
                    tmpForward[x] = (byte)(x / 4096 + 1);

                byte[] tmpReverse = new byte[65536];
                for (int x = 0; x < tmpReverse.Length; x++)
                    tmpReverse[x] = (byte)(255 - x / 4096);

                Random R = new Random();
                int seed = R.Next();
                R = new Random(seed);

                fixed (byte* lpReverse = tmpReverse, lpForward = tmpForward)
                {

                    for (int cnt = 0; cnt < 10; cnt++)
                    {

                        ulong dirtyFlags = (uint)R.Next();

                        var page1 = new PageMetaDataList.PageMetaData()
                            {
                                ArrayIndex = R.Next(),
                                IsDirtyFlags = BitMath.CreateBitMask(32),
                                LocationOfPage = lpForward,
                                PositionIndex = cnt
                            };
                        var page2 = new PageMetaDataList.PageMetaData()
                            {
                                ArrayIndex = R.Next(),
                                IsDirtyFlags = dirtyFlags,
                                LocationOfPage = lpReverse,
                                PositionIndex = cnt
                            };

                        var lst = new PageMetaDataList.PageMetaData[] { page1, page2 };
                        io.Write(lst, false);

                        for (int x = 0; x < 16; x++)
                        {
                            if ((dirtyFlags & (1ul << x)) == 0)
                            {
                                Assert.AreEqual(x + 1, CheckPage(fs, cnt * 65536 + x * 4096, 4096));
                            }
                            else
                            {
                                Assert.AreEqual(255 - x, CheckPage(fs, cnt * 65536 + x * 4096, 4096));
                            }

                        }

                    }

                    //recheck

                    R = new Random(seed);

                    for (int cnt = 0; cnt < 10; cnt++)
                    {

                        ulong dirtyFlags = (uint)R.Next();
                        R.Next();
                        R.Next();

                        for (int x = 0; x < 16; x++)
                        {
                            if ((dirtyFlags & (1ul << x)) == 0)
                            {
                                Assert.AreEqual(x + 1, CheckPage(fs, cnt * 65536 + x * 4096, 4096));
                            }
                            else
                            {
                                Assert.AreEqual(255 - x, CheckPage(fs, cnt * 65536 + x * 4096, 4096));
                            }
                        }
                    }

                    

                    R = new Random(seed);

                    for (int cnt = 0; cnt < 10; cnt++)
                    {
                        ulong dirtyFlags = (uint)R.Next();
                        R.Next();
                        R.Next();

                        byte[] page = null;

                        io.Read(cnt * 65536, (x) => page = x);

                        for (int x = 0; x < 16; x++)
                        {
                            if ((dirtyFlags & (1ul << x)) == 0)
                            {
                                Assert.AreEqual(x + 1, CheckPage(page, x * 4096, 4096));
                            }
                            else
                            {
                                Assert.AreEqual(255 - x, CheckPage(page, x * 4096, 4096));
                            }
                        }
                    }

                    //recheck

                    R = new Random(seed);

                    for (int cnt = 0; cnt < 10; cnt++)
                    {

                        ulong dirtyFlags = (uint)R.Next();
                        R.Next();
                        R.Next();

                        for (int x = 0; x < 16; x++)
                        {
                            if ((dirtyFlags & (1ul << x)) == 0)
                            {
                                Assert.AreEqual(x + 1, CheckPage(fs, cnt * 65536 + x * 4096, 4096));
                            }
                            else
                            {
                                Assert.AreEqual(255 - x, CheckPage(fs, cnt * 65536 + x * 4096, 4096));
                            }
                        }
                    }
                }

            }

        }

        public int CheckPage(Stream str, int pos, int length)
        {
            str.Position = pos;
            int firstValue = str.ReadByte();
            for (int x = 1; x < length; x++)
            {
                if (firstValue != str.ReadByte())
                    throw new Exception("Numbers are not all the same");
            }
            return firstValue;
        }
        public int CheckPage(byte[] str, int pos, int length)
        {
            int firstValue = str[pos];
            for (int x = pos; x < pos + length; x++)
            {
                if (firstValue != str[x])
                    throw new Exception("Numbers are not all the same");
            }
            return firstValue;
        }
    }
}
