namespace AssociateTestsToTestCases.Message.Stage
{
    public class StageMessages
    {
        public readonly string Format = "[{0}]";

        public readonly ProjectMessage Project = new ProjectMessage();
        public readonly SummaryMessage Summary = new SummaryMessage();
        public readonly ArgumentMessage Argument = new ArgumentMessage();
        public readonly TestCaseMessage TestCase = new TestCaseMessage();
        public readonly TestMethodMessage TestMethod = new TestMethodMessage();
        public readonly HttpClientsMessage HttpClient = new HttpClientsMessage();
        public readonly AssociationMessage Association = new AssociationMessage();
        public readonly TestAssemblyPathMessage TestAssemblyPath = new TestAssemblyPathMessage();
        public readonly DevOpsCredentialsMessage DevOpsCredentials = new DevOpsCredentialsMessage();
    }
}
