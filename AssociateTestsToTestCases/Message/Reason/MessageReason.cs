using System.Linq;

namespace AssociateTestsToTestCases.Message.Reason
{
    public class MessageReason
    {
        public readonly string Association = "Association";
        public readonly string MissingTestMethod = "Missing Test Method";
        public readonly string MissingTestCaseId = "Missing Test Case Id";
        public readonly string AssociatedTestMethodNotAvailable = "Associated Test Method is N\\A";

        public int LongestReasonCount { get; }

        public MessageReason()
        {
            var fields = GetType().GetFields().Select(s => s.GetValue(this).ToString()).ToArray();
            LongestReasonCount = fields.OrderByDescending(s => s.Length).First().Count();
        }
    }
}
