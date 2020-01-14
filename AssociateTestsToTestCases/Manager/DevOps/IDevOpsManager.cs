using System.Collections.Generic;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public interface IDevOpsManager
    {
        bool TestPlanIsEmpty();
        TestCase[] GetTestCases();
        void Associate(TestMethod[] testMethods, TestCase[] testCases);
    }
}
