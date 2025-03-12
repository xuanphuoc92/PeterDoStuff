using ApprovalTests;

namespace PeterDoStuff.Test.Extensions
{
    public static class ApprovalTestsExtensions
    {
        /// <summary>
        /// Verify via Approval Tests
        /// </summary>
        /// <param name="this"></param>
        public static void Verify(this string @this, string tag = "")
        {
            @this.WriteToConsole(tag);
            Approvals.Verify(@this);
        }
    }
}