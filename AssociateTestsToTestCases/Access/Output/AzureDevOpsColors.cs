using System.Linq;

namespace AssociateTestsToTestCases.Access.Output
{
    public class AzureDevOpsColors
    {
        // Supported ANSI Colors: https://github.com/Microsoft/azure-pipelines-agent/issues/1569
        public readonly string FailureColor = "\u001b[31m";  // Red

        public readonly string DefaultColor = "\u001b[36m";  // Cyan
        public readonly string SuccessColor = "\u001b[32m";  // Green
        public readonly string StageColor = "\u001b[37m";    // White
        public readonly string WarningColor = "##[warning]"; // Yellow

        public int LongestTypeCount { get; }

        public AzureDevOpsColors()
        {
            var fields = GetType().GetFields().Select(s => s.GetValue(this).ToString()).ToArray();
            LongestTypeCount = fields.OrderByDescending(s => s.Length).First().Count();
        }
    }
}
