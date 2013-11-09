using NUnit.Framework;

namespace GSF.SortedTreeStore.Tree
{
    [TestFixture]
    public unsafe class KeyValueMethodsTest
    {
        [Test]
        public void Test1()
        {
            //byte* temp1 = stackalloc byte[9];
            //byte* temp2 = stackalloc byte[9];

            //var kvm = new KeyValueMethods<uint, uint>(new BoxKeyMethodsUint32(), new BoxValueMethodsUint32());
            //Assert.AreEqual(4 + 4 + 1, kvm.MaxKeyValueSize);
            //var key = new TreeUInt32();
            //var value = new TreeUInt32();

            //key.Value = 5;
            //value.Value = 6;

            //Assert.AreEqual(3, kvm.Write(temp1, key, value));

            //Assert.AreEqual(3, kvm.Read(temp1, temp2, key, value));
            //Assert.AreEqual(5u, key.Value);
            //Assert.AreEqual(6u, value.Value);

            //key.Value = 0xAA00DD00u;
            //value.Value = 0x00CC0011u;

            //Assert.AreEqual(5, kvm.Write(temp1, key, value));

            //Assert.AreEqual(5, kvm.Read(temp1, temp2, key, value));
            //Assert.AreEqual(0xAA00DD00u, key.Value);
            //Assert.AreEqual(0x00CC0011u, value.Value);
        }
    }
}