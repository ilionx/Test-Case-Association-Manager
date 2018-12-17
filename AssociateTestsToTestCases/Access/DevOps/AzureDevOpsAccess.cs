using System;
using System.Linq;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public class AzureDevOpsAccess : IDevOpsAccess
    {
        private const string AutomatedName = "Automated";

        private readonly Messages _messages;
        private readonly bool _verboseLogging;
        private readonly IOutputAccess _outputAccess;

        public AzureDevOpsAccess(Messages messages, IOutputAccess outputAccess, bool verboseLogging)
        {
            _messages = messages;
            _outputAccess = outputAccess;
            _verboseLogging = verboseLogging;
        }

        public List<TestCase.TestCase> ListTestCasesWithNotAvailableTestMethods(List<TestCase.TestCase> testCases, List<TestMethod> testMethods)
        {
            var testCasesWithNotAvailableTestMethods = testCases.Where(x => x.AutomationStatus == AutomatedName & testMethods.SingleOrDefault(y => y.Name.Equals(x.Title)) == null).ToList();

            Counter.Warning += testCasesWithNotAvailableTestMethods.Count;
            Counter.TestMethodNotAvailable += testCasesWithNotAvailableTestMethods.Count;

            return testCasesWithNotAvailableTestMethods;
        }

        public int Associate(List<TestMethod> testMethods, List<TestCase.TestCase> testCases, ITestCaseAccess testCaseAccess, bool validationOnly, string testType)
        {
            foreach (var testMethod in testMethods)
            {
                var testCase = testCases.SingleOrDefault(x => x.Title.Equals(testMethod.Name));

                if (testCase == null)
                {
                   _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestMethodInfo, testMethod.Name, $"{testMethod.FullClassName}.{testMethod.Name}"), _messages.Types.Error, _messages.Reasons.MissingTestCase);

                    Counter.TestCaseNotFound += 1;
                    continue;
                }

                if (testCase.AutomationStatus.Equals(AutomatedName) && testCase.Title.Equals(testMethod.Name) & testCase.AutomatedTestName.Equals($"{testMethod.FullClassName}.{testMethod.Name}"))
                {
                    Counter.Total += 1;
                    continue;
                }

                if (testCase.AutomationStatus.Equals(AutomatedName) && testCase.Title.Equals(testMethod.Name) & !testCase.AutomatedTestName.Equals($"{testMethod.FullClassName}.{testMethod.Name}"))
                {
                    if (_verboseLogging)
                    {
                       _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestCaseInfo, testCase.Title, testCase.Id), _messages.Types.Info, _messages.Reasons.FixedAssociationTestCase);
                    }

                    Counter.FixedReference += 1;
                }

                var operationSuccess = testCaseAccess.AssociateTestCaseWithTestMethod(testCase.Id, $"{testMethod.FullClassName}.{testMethod.Name}", testMethod.AssemblyName, Guid.NewGuid().ToString(), validationOnly, testType);

                if (!operationSuccess)
                {
                   _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestCaseInfo, testCase.Title, testCase.Id), _messages.Types.Failure, _messages.Reasons.Association);

                    Counter.OperationFailed += 1;
                    continue;
                }

                if (_verboseLogging)
                {
                   _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestMethodMapped, testMethod.Name, testCase.Id), _messages.Types.Success, _messages.Reasons.Association);
                }

                Counter.Success += 1;
            }

            Counter.Total += Counter.Success;
            Counter.Error += Counter.TestCaseNotFound + Counter.OperationFailed;

            return Counter.Error;
        }
    }
}
