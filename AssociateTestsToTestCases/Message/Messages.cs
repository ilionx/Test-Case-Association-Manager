using AssociateTestsToTestCases.Message.Type;
using AssociateTestsToTestCases.Message.Stage;
using AssociateTestsToTestCases.Message.Reason;
using AssociateTestsToTestCases.Message.Association;

namespace AssociateTestsToTestCases.Message
{
    public class Messages
    {
        public readonly MessageType Types;
        public readonly StageMessages Stage;
        public readonly MessageReason Reasons;
        public readonly Association.AssociationMessage Association;

        public Messages()
        {
            Types = new MessageType();
            Stage = new StageMessages();
            Reasons = new MessageReason();
            Association = new Association.AssociationMessage();
        }
    }
}
