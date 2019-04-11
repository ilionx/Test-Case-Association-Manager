using System.Collections.Generic;
using AssociateTestsToTestCases.Manager.File;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public interface IDevOpsAccess
    {
        int[] GetTestCasesId();
        WorkItem[] GetTestCaseWorkItems();
        int Associate(TestMethod[] testMethods, Dictionary<string, TestCase> testCases);
        List<DuplicateTestCase> ListDuplicateTestCases(TestCase[] testCases);
        List<TestCase> ListTestCasesWithNotAvailableTestMethods(TestMethod[] testMethods, TestCase[] testCases);
    }
}
