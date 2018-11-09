namespace AssociateTestsToTestCases.Message
{
    public class Messages
    {
        public readonly StageMessages Stage;
        public readonly AssociationMessages Association;

        public Messages()
        {
            Stage = new StageMessages();
            Association = new AssociationMessages();
        }
    }
}
