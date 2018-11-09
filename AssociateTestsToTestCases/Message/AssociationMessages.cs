namespace AssociateTestsToTestCases.Message
{
    public class AssociationMessages
    {
        public readonly string Success = "[Association] Test Case '{0}' [Id: {1}]";
        public readonly Failure Failure;

        public AssociationMessages()
        {
            Failure = new Failure();
        }
    }

    public class Failure
    {
        public readonly string TestcaseCouldNotBeAssociated = "[Association] Test Case '{0}' [Id: {1}]";
        public readonly string TestcaseWithNoIdSkipped = "[Missing Test Case Id] Test Case '{0}' will be skipped.";
        public readonly string TestcaseNoCorrespondingTestMethod = "[Missing Test Method] Test Case '{0}' [Id: {1}]";
        public readonly string TestcaseCorrespondingTestMethodNotAvailable = "[Associated Test Method is N\\A] Test Case '{0}' [Id: {1}]";
    }
}
