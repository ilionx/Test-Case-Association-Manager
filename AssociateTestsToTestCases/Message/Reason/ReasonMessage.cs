using System.Linq;

namespace AssociateTestsToTestCases.Message.Reason
{
    public class ReasonMessage
    {
        public readonly string Association = "Association";
        public readonly string MissingTestCase = "Missing Test Case";
        public readonly string FixedAssociationTestCase = "Fixed Association";
        public readonly string AssociatedTestMethodNotAvailable = "N\\A Associated Test Method";

        public int LongestReasonCount { get; }

        public ReasonMessage()
        {
            var fields = GetType().GetFields().Select(s => s.GetValue(this).ToString()).ToArray();
            LongestReasonCount = fields.OrderByDescending(s => s.Length).First().Count();
        }
    }
}
