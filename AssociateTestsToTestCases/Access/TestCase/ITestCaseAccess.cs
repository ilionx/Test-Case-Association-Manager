using System.Collections.Generic;

namespace AssociateTestsToTestCases.Access.TestCase
{
    public interface ITestCaseAccess
    {
        List<TestCase> GetTestCases();
        bool AssociateTestCaseWithTestMethod(int workItemId, string methodName, string assemblyName, string automatedTestId, bool validationOnly, string testType);
        List<DuplicateTestCase> ListDuplicateTestCases(List<TestCase> testCases);
    }
}
