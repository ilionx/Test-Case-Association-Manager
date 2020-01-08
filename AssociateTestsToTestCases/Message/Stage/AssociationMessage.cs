namespace AssociateTestsToTestCases.Message.Stage
{
    public class AssociationMessage
    {
        public readonly string Status = "Associating Test Methods with Test Cases...";
        public readonly string Success = "Test Methods and Test Cases have been associated ({0}).";
        public readonly string Failure = "Could not associate every Test Method with its corresponding Test Case ({0}).";
    }
}
