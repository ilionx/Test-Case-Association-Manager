using System.Linq;

namespace AssociateTestsToTestCases.Access.Output
{
    public class AzureDevOpsColors
    {
        public readonly string FailureColor = "##[error]";
        public readonly string DefaultColor = "##[command]";
        public readonly string SuccessColor = "##[section]";
        public readonly string WarningColor = "##[warning]";

        public int LongestTypeCount { get; }

        public AzureDevOpsColors()
        {
            var fields = GetType().GetFields().Select(s => s.GetValue(this).ToString()).ToArray();
            LongestTypeCount = fields.OrderByDescending(s => s.Length).First().Count();
        }
    }
}
