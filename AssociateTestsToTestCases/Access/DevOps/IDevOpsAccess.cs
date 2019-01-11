using System.Collections.Generic;
using AssociateTestsToTestCases.Manager.File;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public interface IDevOpsAccess
    {
        int[] GetTestCasesId();
        TestCase[] GetTestCases();
        int Associate(TestMethod[] testMethods, Dictionary<string, TestCase> testCases);
        List<DuplicateTestCase> ListDuplicateTestCases(TestCase[] testCases);
        TestCase[] ListTestCasesWithNotAvailableTestMethods(TestCase[] testCases, TestMethod[] testMethods); // todo: switch test parameters
    }
}
