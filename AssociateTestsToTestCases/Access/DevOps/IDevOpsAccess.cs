using System.Collections.Generic;
using AssociateTestsToTestCases.Access.TestCase;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public interface IDevOpsAccess
    {
        List<TestCase.TestCase> ListTestCasesWithNotAvailableTestMethods(List<TestCase.TestCase> testCases, List<TestMethod> testMethods);
        int Associate(List<TestMethod> testMethods, List<TestCase.TestCase> testCases, ITestCaseAccess testCaseAccess, bool validationOnly, string testType);
    }
}
