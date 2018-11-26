using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Manager.Output;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public class AzureDevOpsManager
    {
        private readonly Messages _messages;
        private readonly OutputManager _outputManager;
        private readonly TestCaseAccess _testCaseAccess;
        private readonly AzureDevOpsAccess _azureDevOpsAccess;
        private readonly WriteToConsoleEventLogger _writeToConsoleEventLogger;

        public AzureDevOpsManager(Messages messages, OutputManager outputManager, TestCaseAccess testCaseAccess, AzureDevOpsAccess azureDevOpsAccess, WriteToConsoleEventLogger writeToConsoleEventLogger)
        {
            _messages = messages;
            _outputManager = outputManager;
            _testCaseAccess = testCaseAccess;
            _azureDevOpsAccess = azureDevOpsAccess;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public void Associate(MethodInfo[] methods, List<Access.TestCase.TestCase> testCases, bool validationOnly, string testType)
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.Association.Status, _messages.Types.Stage);

            var testMethods = methods.Select(x => new TestMethod(x.Name, x.Module.Name, x.DeclaringType.FullName)).ToList();

            var testMethodsNotAvailable = _azureDevOpsAccess.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);
            if (testMethodsNotAvailable.Count != 0)
            {
                testMethodsNotAvailable.ForEach(x => _writeToConsoleEventLogger.Write(string.Format(_messages.Associations.TestCaseWithNotAvailableTestMethod, x.Title, x.Id, x.AutomatedTestName), _messages.Types.Warning, _messages.Reasons.AssociatedTestMethodNotAvailable));
            }

            var testMethodsAssociationErrorCount = _azureDevOpsAccess.Associate(testMethods, testCases, _testCaseAccess, validationOnly, testType);
            if (testMethodsAssociationErrorCount != 0)
            {
                _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Association.Failure, testMethodsAssociationErrorCount), _messages.Types.Error);

                _outputManager.OutputSummary(methods, testCases);

                Environment.Exit(-1);
            }

            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.Association.Success, Counter.Success), _messages.Types.Success);
        }
    }
}
