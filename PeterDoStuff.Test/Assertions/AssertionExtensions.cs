namespace PeterDoStuff.Test.Assertions
{
    public static class AssertionExtensions
    {
        public static CollectionAssertion<T> Should<T>(this IEnumerable<T> @this) => new CollectionAssertion<T>(@this);

        public static NullableDateTimeAssertion Should(this DateTime? @this) => new NullableDateTimeAssertion(@this);

        public static BooleanAssertion Should(this bool @this) => new BooleanAssertion(@this);

        public static IntegerAssertion Should(this int @this) => new IntegerAssertion(@this);
    }

    public class CollectionAssertion<T>(IEnumerable<T> value)
    {
        public void HaveCount(int expectedCount) => Assert.AreEqual(expectedCount, value.Count());
    }

    public class NullableDateTimeAssertion(DateTime? value)
    {
        public void BeNull() => Assert.IsNull(value);

        public void BeOnOrAfter(DateTime expectedValue)
        {
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Value >= expectedValue);
        }
    }

    public class BooleanAssertion(bool value)
    {
        public void BeFalse() => Assert.IsFalse(value);
        public void BeTrue() => Assert.IsTrue(value);
    }

    public class IntegerAssertion(int value)
    {
        public void Be(int expectedValue) => Assert.AreEqual(expectedValue, value);
    }
}
