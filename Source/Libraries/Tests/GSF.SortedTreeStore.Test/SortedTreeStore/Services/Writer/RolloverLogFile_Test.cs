using System;
using System.Collections.Generic;
using System.Linq;
using GSF.Diagnostics;
using NUnit.Framework;

namespace GSF.Snap.Services.Writer.Test
{
    [TestFixture]
    public class RolloverLogFile_Test
    {
        [Test]
        public void TestLongTerm()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            string file = @"C:\Temp\LogFileTest.log";

            var source = new List<Guid>();
            source.Add(Guid.NewGuid());
            var dest = Guid.NewGuid();

            var rolloverFile = new RolloverLogFile(file, source, dest);

            var rolloverFile2 = new RolloverLogFile(file);

            Assert.AreEqual(rolloverFile.IsValid, rolloverFile2.IsValid);
            Assert.AreEqual(rolloverFile.DestinationFile, rolloverFile2.DestinationFile);
            Assert.AreEqual(rolloverFile.SourceFiles.Count, rolloverFile2.SourceFiles.Count);

            if (!rolloverFile2.SourceFiles.SequenceEqual(rolloverFile.SourceFiles))
                throw new Exception("Expecting equals.");



        }


    }
}

