namespace AssociateTestsToTestCases.Message.Stage
{
    public class AssociationMessage
    {
        public readonly string Status = "Trying to Associate Test Cases with Test Methods...";
        public readonly string Success = "Test Cases and Test Methods have been associated ({0}).\n";
        public readonly string Failure = "Could not associate every Test Method with its corresponding Test Case ({0}):";
    }
}
