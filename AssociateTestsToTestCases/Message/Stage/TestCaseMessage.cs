namespace AssociateTestsToTestCases.Message.Stage
{
    public class TestCaseMessage
    {
        public readonly string Status = "Retrieving Azure DevOps Test Cases...";
        public readonly string Success = "Azure DevOps Test Cases have been retrieved ({0}).\n";
        public readonly string Failure = "Could not retrieve the Azure DevOps Test Cases (Test Plan is empty). Program has been terminated.\n";
        public readonly string Duplicate = "Duplicate Azure DevOps Test Cases have been found ({0}). Program has been terminated.";
    }
}
