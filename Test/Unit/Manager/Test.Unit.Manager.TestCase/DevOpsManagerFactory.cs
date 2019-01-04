using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.DevOps;
using AssociateTestsToTestCases.Manager.Output;

namespace Test.Unit.Manager.DevOps
{
    public class DevOpsManagerFactory
    {
        private readonly IDevOpsAccess _devOpsAccess;
        private readonly IOutputManager _outputManager;
        private readonly Counter _counter;

        public DevOpsManagerFactory(IDevOpsAccess devOpsAccess, IOutputManager outputManager, Counter counter = null)
        {
            _devOpsAccess = devOpsAccess;
            _outputManager = outputManager;
            _counter = counter ?? new Counter();
        }

        public IDevOpsManager Create()
        {
            return new AzureDevOpsManager(new Messages(), _outputManager, _devOpsAccess, _counter);
        }
    }
}
