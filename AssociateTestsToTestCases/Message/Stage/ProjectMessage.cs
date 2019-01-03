namespace AssociateTestsToTestCases.Message.Stage
{
    public class ProjectMessage
    {
        public readonly string Status = "Validating Project...";
        public readonly string Success = "Project does have Test Methods or Test Cases.";
        public readonly string Failure = "Project does not have any Test Methods or Test Cases.\n";
    }
}
