using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.Output;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public class AzureDevOpsManager : IDevOpsManager
    {
        private readonly Messages _messages;
        private readonly IOutputManager _outputManager;
        private readonly IDevOpsAccess _devOpsAccess;
        private readonly Counter.Counter _counter;

        public AzureDevOpsManager(Messages messages, IOutputManager outputManager, IDevOpsAccess devOpsAccess, Counter.Counter counter)
        {
            _messages = messages;
            _outputManager = outputManager;
            _devOpsAccess = devOpsAccess;
            _counter = counter;
        }

        public bool TestPlanIsEmpty()
        {
            return _devOpsAccess.GetTestCasesId().IsNullOrEmpty();
        }

        public List<TestCase> GetTestCases()
        {
            _outputManager.WriteToConsole(_messages.Stages.TestCase.Status, _messages.Types.Stage);
            var testCases = _devOpsAccess.GetTestCases();
            if (testCases.IsNullOrEmpty())
            {
                _outputManager.WriteToConsole(_messages.Stages.TestCase.Failure, _messages.Types.Error);

                throw new InvalidOperationException(_messages.Stages.TestCase.Failure);
            }

            var duplicateTestCases = _devOpsAccess.ListDuplicateTestCases(testCases);
            if (duplicateTestCases.Count != 0)
            {
                _outputManager.WriteToConsole(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count), _messages.Types.Error);
                duplicateTestCases.ForEach(x => _outputManager.WriteToConsole(string.Format(_messages.TestCases.Duplicate, x.Name, _messages.TestCases.GetDuplicateTestCaseNamesString(x.TestCases)), _messages.Types.Info));

                throw new InvalidOperationException(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count));
            }

            _outputManager.WriteToConsole(string.Format(_messages.Stages.TestCase.Success, testCases.Count), _messages.Types.Success);
            return testCases;
        }

        public void Associate(MethodInfo[] methods, List<TestCase> testCases, string testType)
        {
            _outputManager.WriteToConsole(_messages.Stages.Association.Status, _messages.Types.Stage);

            var testMethods = methods.Select(x => new TestMethod(x.Name, x.Module.Name, x.DeclaringType.FullName, Guid.NewGuid())).ToList();

            var testMethodsNotAvailable = _devOpsAccess.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);
            if (testMethodsNotAvailable.Count != 0)
            {
                testMethodsNotAvailable.ForEach(x => _outputManager.WriteToConsole(string.Format(_messages.Associations.TestCaseWithNotAvailableTestMethod, x.Title, x.Id, x.AutomatedTestName), _messages.Types.Warning, _messages.Reasons.AssociatedTestMethodNotAvailable));
            }

            var testMethodsAssociationErrorCount = _devOpsAccess.Associate(testMethods, testCases, testType);
            if (testMethodsAssociationErrorCount != 0)
            {
                _outputManager.WriteToConsole(string.Format(_messages.Stages.Association.Failure, testMethodsAssociationErrorCount), _messages.Types.Error);
                _outputManager.OutputSummary(methods, testCases);

                throw new InvalidOperationException(string.Format(_messages.Stages.Association.Failure, testMethodsAssociationErrorCount));
            }

            _outputManager.WriteToConsole(string.Format(_messages.Stages.Association.Success, _counter.Success.Total), _messages.Types.Success);
        }
    }
}
