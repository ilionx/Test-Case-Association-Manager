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
            var testMethodsLength = testMethods?.Length ?? 0;
            var testCasesCount = testCases?.Length ?? 0;

            OutputSummaryStage();
            OutputSummaryDetailed();
            OutputSummaryOverview(testMethodsLength, testCasesCount);
        }

        private void OutputSummaryStage()
        {
            var message = GetOutputSummaryStage();

            _outputAccess.WriteToConsole(message, _messages.Types.Stage);
        }

        private void OutputSummaryDetailed()
        {
            var message = GetOutputSummaryDetailed();

            _outputAccess.WriteToConsole(message, _messages.Types.Summary);
        }

        private void OutputSummaryOverview(int testMethodsLength, int testCasesCount)
        {
            var message = GetOutputSummaryOverview(testMethodsLength, testCasesCount);
        
            _outputAccess.WriteToConsole(message, _messages.Types.Summary);
        }

        #region Formattings

        private string GetOutputSummaryStage()
        {
            return _messages.Stages.Summary.Status;
        }

        private string GetOutputSummaryDetailed()
        {
            var detailedSuccess = FormatString(_messages.Stages.Summary.DetailedSuccess, _counter.Success.Total, _counter.Success.FixedReference);
            var detailedError = FormatString(_messages.Stages.Summary.DetailedError, _counter.Error.Total, _counter.Error.OperationFailed, _counter.Error.TestCaseNotFound);
            var detailedWarning = FormatString(_messages.Stages.Summary.DetailedWarning, _counter.Warning.Total, _counter.Warning.TestMethodNotAvailable);
            var detailedUnaffected = FormatString(_messages.Stages.Summary.DetailedUnaffected, _counter.Unaffected.Total, _counter.Unaffected.AlreadyAutomated);

            return $"{detailedSuccess} | {detailedError} | {detailedWarning} | {detailedUnaffected}";
        }

        private string GetOutputSummaryOverview(int testMethodsLength, int testCasesCount)
        {
            return FormatString(_messages.Stages.Summary.Overview, testCasesCount, testMethodsLength, _counter.Total); ;
        }

        private string FormatString(string baseString, params object[] counters)
        {
            return string.Format(baseString, counters);
        }

        #endregion
    }
}
