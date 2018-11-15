using AssociateTestsToTestCases.Message.Type;
using AssociateTestsToTestCases.Message.Stage;
using AssociateTestsToTestCases.Message.Reason;

namespace AssociateTestsToTestCases.Message
{
    public class Messages
    {
        public readonly string Indention = "  * ";
        public readonly string Output = "{0}{1}{2} {3}{4}{5}";

        public readonly TypeMessage Types;
        public readonly StageMessages Stages;
        public readonly ReasonMessage Reasons;
        public readonly Association.AssociationMessage Associations;

        public Messages()
        {
            Types = new TypeMessage();
            Stages = new StageMessages();
            Reasons = new ReasonMessage();
            Associations = new Association.AssociationMessage();
        }
    }
}
