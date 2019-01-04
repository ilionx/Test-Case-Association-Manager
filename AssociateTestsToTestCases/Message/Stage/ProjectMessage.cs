namespace AssociateTestsToTestCases.Message.Stage
{
    public class ProjectMessage
    {
        public readonly string Status = "Validating Project Test Methods and Test Cases...";
        public readonly string Success = "Project Test Methods and Test Cases have been validated.\n";
        public readonly string Failure = "Project does not have any Test Methods or Test Cases.\n";
    }
}
