using ApprovalTests;

namespace PeterDoStuff.Test.Extensions
{
    public static class ApprovalTestsExtensions
    {
        public static void Verify(this string @this)
            => Approvals.Verify(@this);
    }
}