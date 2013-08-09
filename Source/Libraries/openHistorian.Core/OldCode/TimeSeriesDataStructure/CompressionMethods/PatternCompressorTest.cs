//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TVA.IO.Compression;

//namespace TVA.Core.Tests
//{
//    /// <summary>
//    /// Summary description for PatternCompressorTest
//    /// </summary>
//    [TestClass]
//    public class PatternCompressorTest
//    {
//        public PatternCompressorTest()
//        {
//            //
//            // TODO: Add constructor logic here
//            //
//        }

//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        //
//        // You can use the following additional attributes as you write your tests:
//        //
//        // Use ClassInitialize to run code before running the first test in the class
//        // [ClassInitialize()]
//        // public static void MyClassInitialize(TestContext testContext) { }
//        //
//        // Use ClassCleanup to run code after all tests in a class have run
//        // [ClassCleanup()]
//        // public static void MyClassCleanup() { }
//        //
//        // Use TestInitialize to run code before running each test 
//        // [TestInitialize()]
//        // public void MyTestInitialize() { }
//        //
//        // Use TestCleanup to run code after each test has run
//        // [TestCleanup()]
//        // public void MyTestCleanup() { }
//        //
//        #endregion

//        private const int TotalTestSampleSize = int.MaxValue / 500;

//        [TestMethod]
//        // Sequential data seems to compress ~67% with this algorithm
//        public void TestArrayOfFloatCompressionOnSequentialData()
//        {
//            StringBuilder results = new StringBuilder();
//            MemoryStream buffer = new MemoryStream();
//            Random rnd = new Random();

//            float value = (float)(rnd.NextDouble() * 99999.99D);

//            for (int i = 0; i < TotalTestSampleSize; i++)
//            {
//                value += 0.055F;

//                if (i % 10 == 0)
//                    value = (float)(rnd.NextDouble() * 99999.99D);

//                buffer.Write(BitConverter.GetBytes(value), 0, 4);
//            }

//            // Add one byte of extra space to accomodate compression algorithm
//            buffer.WriteByte(0xff);

//            byte[] arrayOfFloats = buffer.ToArray();
//            int bufferLen = arrayOfFloats.Length;
//            int dataLen = bufferLen - 1;

//            Ticks stopTime, startTime = PrecisionTimer.UtcNow.Ticks;
//            int compressedLen = arrayOfFloats.CompressFloatEnumeration(0, dataLen, bufferLen);
//            stopTime = PrecisionTimer.UtcNow.Ticks;

//            // Publish results to debug window
//            results.AppendFormat("Results of floating point compression algorithm over sequential data:\r\n\r\n");
//            results.AppendFormat("Total number of samples: \t{0:#,##0}\r\n", TotalTestSampleSize);
//            results.AppendFormat("Total number of bytes:   \t{0:#,##0}\r\n", dataLen);
//            results.AppendFormat("Total Calculation time:  \t{0}\r\n", (stopTime - startTime).ToElapsedTimeString(4));
//            results.AppendFormat("Calculation speed:       \t{0:#,##0.0000} Mb/sec\r\n", (dataLen / (double)SI2.Mega) / (stopTime - startTime).ToSeconds());
//            results.AppendFormat("Compression strength:    \t{0:0.00%}", (dataLen - compressedLen) / (double)dataLen);
//            Debug.WriteLine(results.ToString());

//            Assert.AreNotEqual(compressedLen, dataLen);
//        }

//        [TestMethod]
//        // Random data seems to compress ~25%
//        public void TestArrayOfFloatCompressionOnRandomData()
//        {
//            StringBuilder results = new StringBuilder();
//            MemoryStream buffer = new MemoryStream();
//            Random rnd = new Random();

//            float value;

//            for (int i = 0; i < TotalTestSampleSize; i++)
//            {
//                value = (float)(rnd.NextDouble() * 99999.99D);
//                buffer.Write(BitConverter.GetBytes(value), 0, 4);
//            }

//            // Add one byte of extra space to accomodate compression algorithm
//            buffer.WriteByte(0xff);

//            byte[] arrayOfFloats = buffer.ToArray();
//            int bufferLen = arrayOfFloats.Length;
//            int dataLen = bufferLen - 1;

//            Ticks stopTime, startTime = PrecisionTimer.UtcNow.Ticks;
//            int compressedLen = arrayOfFloats.CompressFloatEnumeration(0, dataLen, bufferLen);
//            stopTime = PrecisionTimer.UtcNow.Ticks;

//            // Publish results to debug window
//            results.AppendFormat("Results of floating point compression algorithm over random data:\r\n\r\n");
//            results.AppendFormat("Total number of samples: \t{0:#,##0}\r\n", TotalTestSampleSize);
//            results.AppendFormat("Total number of bytes:   \t{0:#,##0}\r\n", dataLen);
//            results.AppendFormat("Total Calculation time:  \t{0}\r\n", (stopTime - startTime).ToElapsedTimeString(4));
//            results.AppendFormat("Calculation speed:       \t{0:#,##0.0000} Mb/sec\r\n", (dataLen / (double)SI2.Mega) / (stopTime - startTime).ToSeconds());
//            results.AppendFormat("Compression strength:    \t{0:0.00%}", (dataLen - compressedLen) / (double)dataLen);
//            Debug.WriteLine(results.ToString());

//            Assert.AreNotEqual(compressedLen, dataLen);
//        }

//        [TestMethod]
//        public void TestArrayOfDoubleCompression()
//        {
//            MemoryStream buffer = new MemoryStream();
//            Random rnd = new Random();

//            double value = rnd.NextDouble() * 99999.99D;

//            for (int i = 0; i < TotalTestSampleSize; i++)
//            {
//                value += 0.0011;

//                if (i % 10 == 0)
//                    value = rnd.NextDouble() * 99999.99D;

//                buffer.Write(BitConverter.GetBytes(value), 0, 8);
//            }

//            // Add one byte of extra space to accomodate compression algorithm
//            buffer.WriteByte(0xff);

//            byte[] arrayOfDoubles = buffer.ToArray();
//            int bufferLen = arrayOfDoubles.Length;
//            int dataLen = bufferLen - 1;
//            int compressedLen = arrayOfDoubles.CompressDoubleEnumeration(0, dataLen, bufferLen);

//            Assert.AreNotEqual(compressedLen, dataLen);
//        }
//    }
//}

