using System;
using GSF;
using GSF.Collections;
using NUnit.Framework;

namespace GSF.Collections
{
    /// <summary>
    ///This is a test class for ISupportsReadonlyTest and is intended
    ///to contain all ISupportsReadonlyTest Unit Tests
    ///</summary>
    public class ISupportsReadonlyTest
    {
        public static void Test<T>(ISupportsReadonly<T> obj)
        {
            bool origional = obj.IsReadOnly;

            ISupportsReadonly<T> ro = (ISupportsReadonly<T>)obj.CloneReadonly();
            Assert.AreEqual(true, ro.IsReadOnly);
            Assert.AreEqual(origional,obj.IsReadOnly);

            ISupportsReadonly<T> rw = (ISupportsReadonly<T>)obj.CloneEditable();
            Assert.AreEqual(false, rw.IsReadOnly);
            Assert.AreEqual(origional, obj.IsReadOnly);
            rw.IsReadOnly = true;
            Assert.AreEqual(origional, obj.IsReadOnly);

            Assert.AreEqual(true, rw.IsReadOnly);
            HelperFunctions.ExpectError(() => rw.IsReadOnly = false);
            Assert.AreEqual(origional, obj.IsReadOnly);

            HelperFunctions.ExpectError(() => ro.IsReadOnly = false);

        }
    }
}
