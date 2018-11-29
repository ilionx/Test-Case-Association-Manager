using System.Reflection;
using System.Collections.Generic;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public interface IDevOpsManager
    {
        void Associate(MethodInfo[] methods, List<Access.TestCase.TestCase> testCases, bool validationOnly, string testType);
    }
}
