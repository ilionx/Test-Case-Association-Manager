using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public interface IDevOpsManager
    {
        bool TestPlanIsEmpty();
        List<TestCase> GetTestCases();
        void Associate(MethodInfo[] methods, List<TestCase> testCases);
    }
}
