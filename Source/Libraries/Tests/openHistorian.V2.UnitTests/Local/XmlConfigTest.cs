using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Local;

//ToDo: More comprehensive tests
namespace openHistorian.Server.Configuration
{
    [TestFixture()]
    public class XmlConfigTest
    {
        [Test()]
        public void TestConfig()
        {
            var xml = new XmlConfig();
            var instance = xml.RootNode.Add("Instance");
            instance.Add("Name", "OGE - Full Resolution Synchrophasor");
            var archiveList = instance.Add("ArchiveList");
            archiveList.Add("Name", "FinalRollover");
            archiveList.Add("FilePath", "c:\\archive\\2012.ohdb2");
            archiveList.Add("IsReadOnly", "true");
            archiveList = instance.Add("ArchiveList");
            archiveList.Add("Name", "FinalRollover");
            archiveList.Add("FilePath", "c:\\archive\\2011.ohdb2");
            archiveList.Add("IsReadOnly", "true");
            archiveList = instance.Add("ArchiveList");
            archiveList.Add("Name", "FinalRollover");
            archiveList.Add("FilePath", "c:\\archive\\2010.ohdb2");
            archiveList.Add("IsReadOnly", "true");

            MemoryStream ms = new MemoryStream();

            xml.Save(ms);
            ms.Position = 0;

            var xml2 = new XmlConfig(ms);
            AreNodesEqual(xml.RootNode, xml2.RootNode);

            Assert.AreEqual(xml2.RootNode.GetChild("iNstance").GetChild("NamE").Value,
                            "OGE - Full Resolution Synchrophasor");
            Assert.AreEqual(xml2.RootNode.GetChild("iNstance").GetChild("NamE").Parent.GetChild("ArchiveList").GetChild("FilePath").Value, 
                "c:\\archive\\2012.ohdb2");

        }
 
        void AreNodesEqual(ConfigNode a, ConfigNode b)
        {
            Assert.AreEqual(a.Id, b.Id);
            Assert.AreEqual(a.ParentId, b.ParentId);
            Assert.AreEqual(a.Field, b.Field);
            Assert.AreEqual(a.Value, b.Value);
            var ac = a.GetChildren();
            var bc = b.GetChildren();
            Assert.AreEqual(ac.Length, bc.Length);
            for (int x = 0; x < ac.Length; x++)
            {
                AreNodesEqual(ac[x], bc[x]);
            }
        }
    }
}
