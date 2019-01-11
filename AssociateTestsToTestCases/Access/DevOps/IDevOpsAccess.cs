using System.Collections.Generic;
using AssociateTestsToTestCases.Manager.File;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public interface IDevOpsAccess
    {
        int[] GetTestCasesId();
        List<TestCase> GetTestCases();
        int Associate(TestMethod[] testMethods, List<TestCase> testCases);
        List<DuplicateTestCase> ListDuplicateTestCases(List<TestCase> testCases);
        List<TestCase> ListTestCasesWithNotAvailableTestMethods(List<TestCase> testCases, TestMethod[] testMethods); // todo: switch test parameters
    }
}
