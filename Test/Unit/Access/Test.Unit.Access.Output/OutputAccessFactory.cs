using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;

namespace Test.Unit.Access.Output
{
    public class OutputAccessFactory
    {
        private readonly bool _isLocal;
        private readonly Messages _messages;
        private readonly AzureDevOpsColors _azureDevOpsColors;

        public OutputAccessFactory(bool isLocal, Messages messages, AzureDevOpsColors azureDevOpsColors)
        {
            _isLocal = isLocal;
            _messages = messages;
            _azureDevOpsColors = azureDevOpsColors;
        }

        public IOutputAccess Create()
        {
            return new CommandLineAccess(_isLocal, _messages, _azureDevOpsColors);
        }
    }
}
