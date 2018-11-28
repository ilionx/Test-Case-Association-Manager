using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Manager.Output;

namespace Test.Unit.Manager.Output
{
    public class OutputManagerFactory
    {
        private readonly Messages _messages;
        private readonly IWriteToConsoleEventLogger _writeToConsoleEventLogger;

        public OutputManagerFactory(Messages messages, IWriteToConsoleEventLogger writeToConsoleEventLogger )
        {
            _messages = messages;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public IOutputManager Create()
        {
            return new OutputManager(_messages, _writeToConsoleEventLogger);
        }
    }
}
