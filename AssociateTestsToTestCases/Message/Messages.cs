namespace AssociateTestsToTestCases.Message
{
    public class Messages
    {
        public readonly StageMessages Stage;
        public readonly AssociationMessages Association;
        public readonly MessageType Types;
        public readonly MessageReason Reasons;

        public Messages()
        {
            Types = new MessageType();
            Stage = new StageMessages();
            Reasons = new MessageReason();
            Association = new AssociationMessages();
        }
    }
}
