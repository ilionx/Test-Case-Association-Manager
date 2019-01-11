using System.Collections.Generic;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.Output
{
    public interface IOutputManager
    {
        void WriteToConsole(string message, string messageType = "", string reason = "");
        void OutputSummary(TestMethod[] testMethods, List<TestCase> testCases);
    }
}
