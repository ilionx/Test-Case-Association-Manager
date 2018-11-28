using System.Reflection;
using System.Collections.Generic;

namespace AssociateTestsToTestCases.Manager.Output
{
    public interface IOutputManager
    {
        void OutputSummary(MethodInfo[] testMethods, List<Access.TestCase.TestCase> testCases);
    }
}
