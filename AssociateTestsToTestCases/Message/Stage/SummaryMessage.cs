namespace AssociateTestsToTestCases.Message.Stage
{
    public class SummaryMessage
    {
        public readonly string Status = "Summary";
        public readonly string Overview = "Total Azure DevOps Test Cases: {0} | Total Test Methods: {1} | Total Associations: {2}";
        public readonly string Detailed = "Success: {0} [Fixed: {1}] | Errors: {2} [Association operation failed: {3} - Test Case not found: {4}] | Warnings: {5} [Test Method N\\A anymore: {6}] | Unaffected: {7} [Already Automated: {8}]";
    }
}
