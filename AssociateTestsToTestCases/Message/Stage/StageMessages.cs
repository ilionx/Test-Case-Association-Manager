namespace AssociateTestsToTestCases.Message.Stage
{
    public class StageMessages
    {
        public readonly SummaryMessage Summary = new SummaryMessage();
        public readonly ArgumentMessage Argument = new ArgumentMessage();
        public readonly TestCaseMessage TestCase = new TestCaseMessage();
        public readonly TestMethodMessage TestMethod = new TestMethodMessage();
        public readonly AssociationMessage Association = new AssociationMessage();
    }
}
