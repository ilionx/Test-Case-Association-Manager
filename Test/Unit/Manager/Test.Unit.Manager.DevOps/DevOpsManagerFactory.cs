using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using AssociateTestsToTestCases.Access.TestCase;

namespace Test.Unit.Manager.DevOps
{
    public class DevOpsManagerFactory
    {
        private readonly Counter _counter;
        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        private readonly IDevOpsAccess _devOpsAccess;
        private readonly IOutputManager _outputManager;
        private readonly ITestCaseAccess _testCaseAccess;

        public DevOpsManagerFactory(IDevOpsAccess devOpsAccess, IOutputManager outputManager, IOutputAccess outputAccess, ITestCaseAccess testCaseAccess, Counter counter , Messages messages = null)
        {
            _counter = counter;
            _devOpsAccess = devOpsAccess;
            _outputAccess = outputAccess;
            _outputManager = outputManager;
            _testCaseAccess = testCaseAccess;
            _messages = messages ?? new Messages();
        }

        public IDevOpsManager Create()
        {
            return new AzureDevOpsManager(_messages, _outputAccess, _outputManager, _testCaseAccess, _devOpsAccess, _counter);
        }
    }
}
