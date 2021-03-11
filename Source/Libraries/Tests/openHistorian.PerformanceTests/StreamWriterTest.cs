using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using GSF;
using NUnit.Framework;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    class StreamWriterTest
    {
        [Test]
        public void TestOrig()
        {

            using (StreamWriter csvStream = new StreamWriter("C:\\temp\\file.csv"))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (float x = 0.216421654f; x < 2000000; x++)
                {
                    csvStream.Write(x);
                    csvStream.Write(',');
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds);
            }
        }

        [Test]
        public void TestOpt1()
        {

            using (StreamWriter csvStream = new StreamWriter("C:\\temp\\file.csv"))
            {
                IFormatProvider format = csvStream.FormatProvider;
                NumberFormatInfo info = NumberFormatInfo.GetInstance(format);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (float x = 0.216421654f; x < 2000000; x++)
                {
                    csvStream.Write(x.ToString(format));// Number.FormatSingle(x, null, info));
                    csvStream.Write(',');
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds);
            }
        }

        [Test]
        public void TestOpt3()
        {
            using (StreamWriter csvStream = new StreamWriter("C:\\temp\\file.csv"))
            {
                UltraStreamWriter usw = new UltraStreamWriter(csvStream);
                IFormatProvider format = csvStream.FormatProvider;
                NumberFormatInfo info = NumberFormatInfo.GetInstance(format);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (float x = 0.216421654f; x < 2000000; x++)
                {
                    usw.Write(x);
                    usw.Write(',');
                }
                usw.Flush();
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalSeconds);
            }
        }



        //const uint IntToConvert = 3u;
        //const uint IntToConvert = 3463u;
        const uint IntToConvert = 2214352634u;
        [Test]
        public void TestWriteInt32()
        {
            char[] data = new char[30];

            for (int x = 0; x < 5000000; x++)
                IntToConvert.WriteToChars(data, 0);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < 50000000; x++)
                IntToConvert.WriteToChars(data, 0);

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }

        [Test]
        public void TestWriteInt322()
        {
            char[] data = new char[30];

            for (int x = 0; x < 5000000; x++)
                IntToConvert.WriteToChars2(data, 0);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < 50000000; x++)
                IntToConvert.WriteToChars2(data, 0);

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }

        const float FloatToConvert = 2263.1234f;

        [Test]
        public void TestWriteOrig()
        {
            _ = new char[300];

            for (int x = 0; x < 500000; x++)
                FloatToConvert.ToString();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < 5000000; x++)
                FloatToConvert.ToString();

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds / 5000000.0 * 1000000000.0);
        }

        [Test]
        public void TestWriteFloat2()
        {
            char[] data = new char[300];

            for (int x = 0; x < 5000000; x++)
                FloatToConvert.WriteToChars(data, 0);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < 50000000; x++)
                FloatToConvert.WriteToChars(data, 0);

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds / 50000000.0 * 1000000000.0);
        }

        [Test]
        public void TestWriteFloatConsistency()
        {
            char[] data = new char[300];
            CompareFloats(0, data);
            CompareFloats(12345678e-0f, data);
            CompareFloats(1234567e-0f, data);
            CompareFloats(1234567e-1f, data);
            CompareFloats(1234567e-2f, data);
            CompareFloats(1234567e-3f, data);
            CompareFloats(1234567e-4f, data);
            CompareFloats(1234567e-5f, data);
            CompareFloats(1234567e-6f, data);
            CompareFloats(1234567e-7f, data);
            CompareFloats(1234567e-8f, data);
            CompareFloats(1234567e-9f, data);
            CompareFloats(1234567e-10f, data);
            CompareFloats(1234567e-11f, data);

            CompareFloats(12345600e-0f, data);
            CompareFloats(1234560e-0f, data);
            CompareFloats(1234560e-1f, data);
            CompareFloats(1234560e-2f, data);
            CompareFloats(1234560e-3f, data);
            CompareFloats(1234560e-4f, data);
            CompareFloats(1234560e-5f, data);
            CompareFloats(1234560e-6f, data);
            CompareFloats(1234560e-7f, data);
            CompareFloats(1234560e-8f, data);
            CompareFloats(1234560e-9f, data);
            CompareFloats(1234560e-10f, data);
            CompareFloats(1234560e-11f, data);
            CompareFloats(12345600e-0f, data);

            CompareFloats(12345605e-0f, data);
            CompareFloats(12345605e-1f, data);
            CompareFloats(12345605e-2f, data);
            CompareFloats(12345605e-3f, data);
            CompareFloats(12345605e-4f, data);
            CompareFloats(12345605e-5f, data);
            CompareFloats(12345605e-6f, data);
            CompareFloats(12345605e-7f, data);
            CompareFloats(12345605e-8f, data);
            CompareFloats(12345605e-9f, data);
            CompareFloats(12345605e-10f, data);
            CompareFloats(12345605e-11f, data);
        }

        void CompareFloats(float value, char[] data)
        {
            int len = value.WriteToChars(data, 0);
            string str = value.ToString();
            if (len != str.Length)
            {
                value.WriteToChars(data, 0);
                throw new Exception();
            }

            for (int x = 0; x < len; x++)
            {
                if (str[x] != data[x])
                {
                    value.WriteToChars(data, 0);
                    throw new Exception();
                }
            }
            Console.WriteLine(str);
        }

        [Test]
        public void DisplayDefaultFormat()
        {
            Console.WriteLine(9999999f.ToString());
            Console.WriteLine(9999998.5f.ToString());

            Console.WriteLine(12345605e-1f.ToString());
            Console.WriteLine(12345615e-1f.ToString());
            Console.WriteLine(12345625e-1f.ToString());
            Console.WriteLine(12345635e-1f.ToString());
            Console.WriteLine(12345645e-1f.ToString());
            Console.WriteLine(12345655e-1f.ToString());
            Console.WriteLine(12345665e-1f.ToString());
            Console.WriteLine(12345675e-1f.ToString());
            Console.WriteLine(12345685e-1f.ToString());
            Console.WriteLine(12345695e-1f.ToString());

            Console.WriteLine(7234567890123456789e-0f.ToString());
            Console.WriteLine(7234567890123456789e-1f.ToString());
            Console.WriteLine(7234567890123456789e-2f.ToString());
            Console.WriteLine(7234567890123456789e-3f.ToString());
            Console.WriteLine(7234567890123456789e-4f.ToString());
            Console.WriteLine(7234567890123456789e-5f.ToString());
            Console.WriteLine(7234567890123456789e-6f.ToString());
            Console.WriteLine(7234567890123456789e-7f.ToString());
            Console.WriteLine(7234567890123456789e-8f.ToString());
            Console.WriteLine(7234567890123456789e-9f.ToString());
            Console.WriteLine(7234567890123456789e-10f.ToString());
            Console.WriteLine(7234567890123456789e-11f.ToString());
            Console.WriteLine(7234567890123456789e-12f.ToString());
            Console.WriteLine(7234567890123456789e-13f.ToString());
            Console.WriteLine(7234567890123456789e-14f.ToString());
            Console.WriteLine(7234567890123456789e-15f.ToString());
            Console.WriteLine(7234567890123456789e-16f.ToString());
            Console.WriteLine(7234567890123456789e-17f.ToString());
            Console.WriteLine(7234567890123456789e-18f.ToString());
            Console.WriteLine(7234567890123456789e-19f.ToString());
            Console.WriteLine(7234567890123456789e-20f.ToString());
            Console.WriteLine(7234567890123456789e-21f.ToString());
            Console.WriteLine(7234567890123456789e-22f.ToString());
            Console.WriteLine(7234567890123456789e-23f.ToString());
            Console.WriteLine(7234567890123456789e-24f.ToString());
            Console.WriteLine(7234567890123456789e-25f.ToString());
            Console.WriteLine(7234567890123456789e-26f.ToString());
            Console.WriteLine(7234567890123456789e-27f.ToString());
            Console.WriteLine(7234567890123456789e-28f.ToString());
            Console.WriteLine(7234567890123456789e-29f.ToString());
            Console.WriteLine((-1502345222199E-07F).ToString());
        }

        public class UltraStreamWriter
        {
            const int Size = 1024;
            const int FlushSize = 1024 - 40;
            readonly char[] m_buffer;
            int m_position;
            readonly StreamWriter m_stream;
            readonly string nl = Environment.NewLine;
            public UltraStreamWriter(StreamWriter stream)
            {
                m_buffer = new char[Size];
                m_stream = stream;
            }

            public void Write(char value)
            {
                if (m_position < FlushSize)
                    Flush();
                m_buffer[m_position] = value;
            }

            public void Write(float value)
            {
                if (m_position < FlushSize)
                    Flush();
                m_position += value.WriteToChars(m_buffer, m_position);
            }

            public void WriteLine()
            {
                if (m_position < FlushSize)
                    Flush();
                if (nl.Length == 2)
                {
                    m_buffer[m_position] = nl[0];
                    m_buffer[m_position + 1] = nl[1];
                    m_position += 2;
                }
                else
                {
                    m_buffer[m_position] = nl[0];
                    m_position += 2;
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public void Flush()
            {
                if (m_position > 0)
                    m_stream.Write(m_buffer, 0, m_position);
                m_position = 0;
            }

        }





    }
}
