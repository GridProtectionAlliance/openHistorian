using GSF.Immutable;
using NUnit.Framework;
using openHistorian;

namespace GSF.Collections.Test
{
    /// <summary>
    ///This is a test class for ISupportsReadonlyTest and is intended
    ///to contain all ISupportsReadonlyTest Unit Tests
    ///</summary>
    public class ISupportsReadonlyTest
    {
        public static void Test<T>(IImmutableObject<T> obj)
        {
            bool original = obj.IsReadOnly;

            IImmutableObject<T> ro = (IImmutableObject<T>)obj.CloneReadonly();
            Assert.AreEqual(true, ro.IsReadOnly);
            Assert.AreEqual(original, obj.IsReadOnly);

            IImmutableObject<T> rw = (IImmutableObject<T>)obj.CloneEditable();
            Assert.AreEqual(false, rw.IsReadOnly);
            Assert.AreEqual(original, obj.IsReadOnly);
            rw.IsReadOnly = true;
            Assert.AreEqual(original, obj.IsReadOnly);

            Assert.AreEqual(true, rw.IsReadOnly);
            HelperFunctions.ExpectError(() => rw.IsReadOnly = false);
            Assert.AreEqual(original, obj.IsReadOnly);

            HelperFunctions.ExpectError(() => ro.IsReadOnly = false);
        }
    }
}