using System.Linq;

namespace AssociateTestsToTestCases.Message.Type
{
    public class TypeMessage
    {
        public readonly string Info = "INFO";
        public readonly string Stage = "STAGE";
        public readonly string Error = "ERROR";
        public readonly string Failure = "FAILURE";
        public readonly string Success = "SUCCESS";
        public readonly string Summary = "SUMMARY";
        public readonly string Warning = "WARNING";

        public int LongestTypeCount { get; }

        public TypeMessage()
        {
            var fields = GetType().GetFields().Select(s => s.GetValue(this).ToString()).ToArray();
            LongestTypeCount = fields.OrderByDescending(s => s.Length).First().Count();
        }
    }
}
