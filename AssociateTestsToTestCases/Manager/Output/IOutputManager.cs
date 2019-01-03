using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.Output
{
    public interface IOutputManager
    {
        void WriteToConsole(string message, string messageType = "", string reason = "");
        void OutputSummary(MethodInfo[] testMethods, List<TestCase> testCases);
    }
}
