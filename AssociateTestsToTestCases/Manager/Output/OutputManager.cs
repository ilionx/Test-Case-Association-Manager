using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.Output
{
    public class OutputManager : IOutputManager
    {
        private readonly Counter.Counter _counter;
        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        
        public OutputManager(Messages messages, IOutputAccess outputAccess, Counter.Counter counter)
        {
            _counter = counter;
            _messages = messages;
            _outputAccess = outputAccess;
        }

        public void WriteToConsole(string message, string messageType = "", string reason = "")
        {
            _outputAccess.WriteToConsole(message, messageType, reason);
        }

        public void OutputSummary(TestMethod[] testMethods, TestCase[] testCases)
        {
            var testMethodsLength = testMethods == null ? 0 : testMethods.Length;
            var testCasesCount = testCases == null ? 0 : testCases.Length;

            _outputAccess.WriteToConsole(_messages.Stages.Summary.Status, _messages.Types.Stage);
            _outputAccess.WriteToConsole(string.Format(_messages.Stages.Summary.Detailed, _counter.Success.Total, _counter.Success.FixedReference, _counter.Error.Total, _counter.Error.OperationFailed, _counter.Error.TestCaseNotFound, _counter.Warning.Total, _counter.Warning.TestMethodNotAvailable, _counter.Unaffected.Total, _counter.Unaffected.AlreadyAutomated), _messages.Types.Summary);
            _outputAccess.WriteToConsole(string.Format(_messages.Stages.Summary.Overview, testCasesCount, testMethodsLength, _counter.Total), _messages.Types.Summary);
        }
    }
}
