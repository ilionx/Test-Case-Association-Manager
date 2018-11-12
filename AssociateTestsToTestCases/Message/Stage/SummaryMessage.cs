namespace AssociateTestsToTestCases.Message.Stage
{
    public class SummaryMessage
    {
        public readonly string Status = "Summary:";
        public readonly string Overview = "Total Azure DevOps Test Cases: {0} | Total Test Methods: {1} | Total Associations: {2}";
        public readonly string Detailed = "Success: {0} | Errors: {1} | Warnings: {2} [Missing Id: {3} - Test Method N\\A anymore: {4} - Test Method not found: {5}]";
    }
}
