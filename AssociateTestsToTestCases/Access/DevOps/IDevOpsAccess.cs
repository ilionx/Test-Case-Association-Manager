using System.Collections.Generic;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public interface IDevOpsAccess
    {
        int[] GetTestCasesId();
        List<TestCase> GetTestCases();
        int Associate(List<TestMethod> testMethods, List<TestCase> testCases, string testType);
        List<DuplicateTestCase> ListDuplicateTestCases(List<TestCase> testCases);
        List<TestCase> ListTestCasesWithNotAvailableTestMethods(List<TestCase> testCases, List<TestMethod> testMethods);
    }
}
