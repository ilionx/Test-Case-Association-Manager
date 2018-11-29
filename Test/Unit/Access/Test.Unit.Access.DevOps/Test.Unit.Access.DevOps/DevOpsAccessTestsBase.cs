using AssociateTestsToTestCases.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class DevOpsAccessTestsBase
    {
        [TestCleanup]
        public void CleanUp()
        {
            Counter.Total = 0;
            Counter.Error = 0;
            Counter.Success = 0;
            Counter.Warning = 0;
            Counter.FixedReference = 0;
            Counter.OperationFailed = 0;
            Counter.TestCaseNotFound = 0;
            Counter.TestMethodNotAvailable = 0;
        }
    }
}
