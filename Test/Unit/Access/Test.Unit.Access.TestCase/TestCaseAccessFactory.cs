using AssociateTestsToTestCases;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;

namespace Test.Unit.Access.DevOps
{
    public class DevOpsAccessFactory
    {
        private readonly AzureDevOpsHttpClients _azureDevOpsHttpClients;

        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        private readonly InputOptions _options;
        private readonly Counter _counter;

        public DevOpsAccessFactory(AzureDevOpsHttpClients azureDevOpsHttpClients, Messages messages, IOutputAccess outputAccess, InputOptions options, Counter counter)
        {
            _azureDevOpsHttpClients = azureDevOpsHttpClients;

            _messages = messages;
            _outputAccess = outputAccess;
            _options = options;
            _counter = counter;
        }

        public IDevOpsAccess Create()
        {

            return new AzureDevOpsAccess(_azureDevOpsHttpClients, _messages, _outputAccess, _options, _counter);
        }
    }
}
