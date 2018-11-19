namespace AssociateTestsToTestCases.Message.Stage
{
    public class TestCaseMessage
    {
        public readonly string Status = "Trying to retrieve the Azure DevOps Test Cases...";
        public readonly string Success = "Azure DevOps Test Cases have been obtained ({0}).\n";
        public readonly string Failure = "Could not retrieve the Azure DevOps Test Cases. Program has been terminated.\n";
        public readonly string Duplicate = "Duplicate Azure DevOps Test Cases have been found ({0}). Program has been terminated.";
    }
}
