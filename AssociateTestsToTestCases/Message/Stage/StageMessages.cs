namespace AssociateTestsToTestCases.Message.Stage
{
    public class StageMessages
    {
        public readonly SummaryMessage Summary = new SummaryMessage();
        public readonly ArgumentsMessage Arguments = new ArgumentsMessage();
        public readonly TestCasesMessage TestCases = new TestCasesMessage();
        public readonly AssociationMessage Association = new AssociationMessage();
        public readonly DllTestMethodsMessage DllTestMethods = new DllTestMethodsMessage();
    }
}
