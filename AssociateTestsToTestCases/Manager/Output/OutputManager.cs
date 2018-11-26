using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Message;

namespace AssociateTestsToTestCases.Manager.Output
{
    public class OutputManager
    {
        private readonly Messages _messages;
        private readonly WriteToConsoleEventLogger _writeToConsoleEventLogger;
        
        public OutputManager(Messages messages, WriteToConsoleEventLogger writeToConsoleEventLogger)
        {
            _messages = messages;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public void OutputSummary(MethodInfo[] testMethods, List<Access.TestCase.TestCase> testCases)
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.Summary.Status, _messages.Types.Stage);
            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Summary.Detailed, Counter.Success, Counter.FixedReference, Counter.Error, Counter.OperationFailed, Counter.TestCaseNotFound, Counter.Warning, Counter.TestMethodNotAvailable), _messages.Types.Summary);
            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Summary.Overview, testCases.Count, testMethods.Length, Counter.Total), _messages.Types.Summary);
        }
    }
}
