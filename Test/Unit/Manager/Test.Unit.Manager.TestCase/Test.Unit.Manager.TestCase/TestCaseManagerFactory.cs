using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Manager.TestCase;

namespace Test.Unit.Manager.TestCase
{
    public class TestCaseManagerFactory
    {
        private readonly ITestCaseAccess _testCaseAccess;
        private readonly IWriteToConsoleEventLogger _writeToConsoleEventLogger;

        public TestCaseManagerFactory(ITestCaseAccess testCaseAccess, IWriteToConsoleEventLogger writeToConsoleEventLogger)
        {
            _testCaseAccess = testCaseAccess;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public ITestCaseManager Create()
        {
            return new TestCaseManager(new Messages(), _testCaseAccess, _writeToConsoleEventLogger);
        }
    }
}
