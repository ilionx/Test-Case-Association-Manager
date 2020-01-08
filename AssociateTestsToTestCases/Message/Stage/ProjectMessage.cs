namespace AssociateTestsToTestCases.Message.Stage
{
    public class ProjectMessage
    {
        public readonly string Status = "Validating Project Test Methods and Test Cases...";
        public readonly string Success = "Project Test Methods and Test Cases have been validated.";
        public readonly string Failure = "Project has neither any Test Methods nor Test Cases.";
    }
}
