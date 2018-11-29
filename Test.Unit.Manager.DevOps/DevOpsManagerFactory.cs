using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using AssociateTestsToTestCases.Access.TestCase;

namespace Test.Unit.Manager.DevOps
{
    public class DevOpsManagerFactory
    {
        private readonly Messages _messages;
        private readonly IDevOpsAccess _devOpsAccess;
        private readonly IOutputManager _outputManager;
        private readonly ITestCaseAccess _testCaseAccess;
        private readonly IWriteToConsoleEventLogger _writeToConsoleEventLogger;

        public DevOpsManagerFactory(IOutputManager outputManager, ITestCaseAccess testCaseAccess, IDevOpsAccess devOpsAccess, IWriteToConsoleEventLogger writeToConsoleEventLogger, Messages messages = null)
        {
            _devOpsAccess = devOpsAccess;
            _outputManager = outputManager;
            _testCaseAccess = testCaseAccess;
            _messages = messages ?? new Messages();
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public IDevOpsManager Create()
        {
            return new AzureDevOpsManager(_messages, _outputManager, _testCaseAccess, _devOpsAccess, _writeToConsoleEventLogger);
        }
    }
}
