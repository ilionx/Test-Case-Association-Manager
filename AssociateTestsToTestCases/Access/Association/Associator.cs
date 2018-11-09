using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;

namespace AssociateTestsToTestCases.Access.Association
{
    public class Associator
    {
        private const string AutomationName = "Automated";

        private readonly Messages _messages;
        private readonly bool _verboseLogging;

        public Associator(Messages messages, bool verboseLogging)
        {
            _messages = messages;
            _verboseLogging = verboseLogging;
        }

        public void Associate(MethodInfo[] testMethods, List<TestCase.TestCase> testCases, TestCaseAccess vstsAccessor, bool validationOnly, string testType)
        {
            foreach (var testCase in testCases)
            {
                var testMethod = testMethods.SingleOrDefault(x => x.Name == testCase.Title);

                if (testCase.Id == null)
                {
                    Writer.WriteToConsole(_messages, _messages.Types.Warning, string.Format(_messages.Association.TestCaseSkipped, testCase.Title), _messages.Reasons.MissingTestCaseId);
                    Counter.WarningMissingId += 1;
                    continue;
                }

                if (testCase.AutomationStatus == AutomationName && testMethod == null)
                {
                    Writer.WriteToConsole(_messages, _messages.Types.Warning, string.Format(_messages.Association.TestCaseInfo, testCase.Title, testCase.Id), _messages.Reasons.AssociatedTestMethodNotAvailable);
                    Counter.WarningTestMethodNotAvailable += 1;
                    continue;
                }

                if (testMethod == null)
                {
                    Writer.WriteToConsole(_messages, _messages.Types.Warning, string.Format(_messages.Association.TestCaseInfo, testCase.Title, testCase.Id), _messages.Reasons.MissingTestMethod);
                    Counter.WarningNoCorrespondingTestMethod += 1;
                    continue;
                }

                if (testCase.AutomationStatus == AutomationName)
                {
                    Counter.Total += 1;
                    continue;
                }

                var operationSuccess = vstsAccessor.AssociateTestCaseWithTestMethod((int)testCase.Id, $"{testMethod.DeclaringType.FullName}.{testMethod.Name}", testMethod.Module.Name, Guid.NewGuid().ToString(), validationOnly, testType);

                if (!operationSuccess)
                {
                    Writer.WriteToConsole(_messages, _messages.Types.Failure, string.Format(_messages.Association.TestCaseInfo, testCase.Title, testCase.Id), _messages.Reasons.Association);
                    Counter.Error += 1;
                    return;
                }

                if (_verboseLogging)
                {
                    Writer.WriteToConsole(_messages, _messages.Types.Success, string.Format(_messages.Association.TestCaseInfo, testCase.Title, testCase.Id), _messages.Reasons.Association);
                }

                Counter.Total += 1;
                Counter.Success += 1;
            }
        }
    }
}
