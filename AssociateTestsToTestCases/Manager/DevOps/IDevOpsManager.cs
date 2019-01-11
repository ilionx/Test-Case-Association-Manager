using System.Collections.Generic;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public interface IDevOpsManager
    {
        bool TestPlanIsEmpty();
        List<TestCase> GetTestCases();
        void Associate(TestMethod[] testMethods, List<TestCase> testCases);
    }
}
