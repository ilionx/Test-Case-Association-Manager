namespace AssociateTestsToTestCases.Message.Association
{
    public class AssociationMessage
    {
        public readonly string TestCaseInfo = "Test Case '{0}' (Id: {1})";
        public readonly string TestMethodInfo = "Test Method '{0}' ({1})";
        public readonly string TestMethodMapped = "Test Method '{0}' (Test Case Id: {1})";
        public readonly string TestCaseWithNotAvailableTestMethod = "Test Case '{0}' (Id: {1}) (Test Method: {2})";
    }
}
