using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.Collections;

namespace openHistorian.V2.Test.Collections
{
    [TestClass()]
    public class ResourceQueueCollectionTest
    {

        [TestMethod()]
        public void Test()
        {
            ResourceQueueCollection<int, string> queue = new ResourceQueueCollection<int, string>((x) => () => x.ToString(), 3, 3);


        }

    }
}
