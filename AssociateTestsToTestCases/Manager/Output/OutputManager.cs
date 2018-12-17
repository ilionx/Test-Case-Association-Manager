using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;

namespace AssociateTestsToTestCases.Manager.Output
{
    public class OutputManager : IOutputManager
    {
        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        
        public OutputManager(Messages messages, IOutputAccess outputAccess)
        {
            _messages = messages;
            _outputAccess = outputAccess;
        }

        public void Write(string message, string messageType = "", string reason = "")
        {
            _outputAccess.WriteToConsole(message, messageType, reason);
        }

        public void OutputSummary(MethodInfo[] testMethods, List<Access.TestCase.TestCase> testCases)
        {
            _outputAccess.WriteToConsole(_messages.Stages.Summary.Status, _messages.Types.Stage);
            _outputAccess.WriteToConsole(string.Format(_messages.Stages.Summary.Detailed, Counter.Success, Counter.FixedReference, Counter.Error, Counter.OperationFailed, Counter.TestCaseNotFound, Counter.Warning, Counter.TestMethodNotAvailable), _messages.Types.Summary);
            _outputAccess.WriteToConsole(string.Format(_messages.Stages.Summary.Overview, testCases.Count, testMethods.Length, Counter.Total), _messages.Types.Summary);
        }
    }
}
