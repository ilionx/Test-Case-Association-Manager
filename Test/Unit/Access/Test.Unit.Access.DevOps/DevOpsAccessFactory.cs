using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.DevOps;

namespace Test.Unit.Access.DevOps
{
    public class DevOpsAccessFactory
    {
        private readonly Messages _messages;
        private readonly bool _verboseLogging;
        private readonly IWriteToConsoleEventLogger _writeToConsoleEventLogger;

        public DevOpsAccessFactory(IWriteToConsoleEventLogger writeToConsoleEventLogger, Messages messages, bool verboseLogging)
        {
            _messages = messages;
            _verboseLogging = verboseLogging;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public IDevOpsAccess Create()
        {
            return new AzureDevOpsAccess(_writeToConsoleEventLogger, _messages, _verboseLogging);
        }
    }
}
