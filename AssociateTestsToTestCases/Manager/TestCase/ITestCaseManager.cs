using System.Collections.Generic;

namespace AssociateTestsToTestCases.Manager.TestCase
{
    public interface ITestCaseManager
    {
        List<Access.TestCase.TestCase> GetTestCases();
    }
}
