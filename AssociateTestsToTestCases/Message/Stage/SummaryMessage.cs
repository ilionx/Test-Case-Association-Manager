namespace AssociateTestsToTestCases.Message.Stage
{
    public class SummaryMessage
    {
        public readonly string Status = "Summary";
        public readonly string Overview = "Total Azure DevOps Test Cases: {0} | Total Test Methods: {1} | Total Associations: {2}";
        public readonly string DetailedSuccess = "Success: {0} [Fixed: {1}]";
        public readonly string DetailedError = "Errors: {0} [Association operation failed: {1} - Test Case not found: {2}]";
        public readonly string DetailedWarning = "Warnings: {0} [Test Method N\\A anymore: {1}]";
        public readonly string DetailedUnaffected = "Unaffected: {0} [Already Automated: {1}]";
    }
}
