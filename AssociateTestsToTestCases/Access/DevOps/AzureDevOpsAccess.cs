using System.Linq;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using TestMethod = AssociateTestsToTestCases.Manager.File.TestMethod;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public class AzureDevOpsAccess : IDevOpsAccess
    {
        private readonly TestManagementHttpClient _testManagementHttpClient;
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;

        private readonly InputOptions _inputOptions;
        private readonly Counter.Counter _counter;

        private const int ChunkSize = 200;
        private const string FieldProperty = "fields";
        private const string AutomatedName = "Automated";
        private const string SystemTitle = "System.Title";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestTypePatchName = "Microsoft.VSTS.TCM.AutomatedTestType";
        private const string AutomatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";

        public AzureDevOpsAccess(TestManagementHttpClient testManagementHttpClient, WorkItemTrackingHttpClient workItemTrackingHttpClient, Messages messages, IOutputAccess outputAccess, InputOptions options, Counter.Counter counter)
        {
            _testManagementHttpClient = testManagementHttpClient;
            _workItemTrackingHttpClient = workItemTrackingHttpClient;

            _messages = messages;
            _outputAccess = outputAccess;

            _inputOptions = options;
            _counter = counter;
        }

        public TestCase[] GetTestCases()
        {
            var testCasesId = GetTestCasesId();

            var chunkedTestCases = ChunkTestCases(testCasesId);

            var testCases = chunkedTestCases.SelectMany(chunkedTestgroupCases => _workItemTrackingHttpClient.GetWorkItemsAsync(chunkedTestgroupCases).Result).ToList();

            return CreateTestCaseArray(testCases);
        }

        public int Associate(TestMethod[] testMethods, Dictionary<string,TestCase> testCases)
        {
            foreach (var testMethod in testMethods)
            {
                if (!testCases.ContainsKey(testMethod.Name))
                {
                    _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestMethodInfo, testMethod.Name, $"{testMethod.FullClassName}.{testMethod.Name}"), _messages.Types.Error, _messages.Reasons.MissingTestCase);

                    _counter.Error.TestCaseNotFound++;
                    continue;
                }

                var testCase = testCases[testMethod.Name];

                var testCaseHasAutomatedStatus = TestCaseHasAutomatedStatus(testCase, testMethod);
                var testCaseIsAlreadyAutomated = testCaseHasAutomatedStatus && TestCaseIsAlreadyAutomated(testCase, testMethod);
                var testCaseHasIncorrectAssociation = testCaseHasAutomatedStatus && !testCaseIsAlreadyAutomated;
                if (testCaseIsAlreadyAutomated)
                {
                    _counter.Unaffected.AlreadyAutomated += 1;
                    continue;
                }
                else if (testCaseHasIncorrectAssociation)
                {
                    if (_inputOptions.VerboseLogging)
                    {
                        _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestCaseInfo, testCase.Title, testCase.Id), _messages.Types.Info, _messages.Reasons.FixedAssociationTestCase);
                    }

                    _counter.Success.FixedReference += 1;
                }

                var operationSuccess = AssociateTestCaseWithTestMethod(testCase.Id, $"{testMethod.FullClassName}.{testMethod.Name}", testMethod.AssemblyName, testMethod.TempId.ToString(), _inputOptions.TestType);

                if (!operationSuccess)
                {
                    _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestCaseInfo, testCase.Title, testCase.Id), _messages.Types.Failure, _messages.Reasons.Association);

                    _counter.Error.OperationFailed += 1;

                    if (testCaseHasIncorrectAssociation)
                    {
                        _counter.Success.FixedReference -= 1;
                    }

                    continue;
                }

                if (_inputOptions.VerboseLogging)
                {
                    _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestMethodMapped, testMethod.Name, testCase.Id), _messages.Types.Success, _messages.Reasons.Association);
                }

                _counter.Success.Total += 1;
            }

            return _counter.Error.Total;
        }

        public List<DuplicateTestCase> ListDuplicateTestCases(TestCase[] testCases)
        {
            var duplicateTestCases = new List<DuplicateTestCase>();

            var duplicates = testCases.Select(x => x.Title).GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            foreach (var duplicate in duplicates)
            {
                duplicateTestCases.Add(new DuplicateTestCase(duplicate, testCases.Where(y => y.Title.Equals(duplicate)).ToArray()));
            }

            return duplicateTestCases;
        }

        public TestCase[] ListTestCasesWithNotAvailableTestMethods(TestCase[] testCases, TestMethod[] testMethods)
        {
            return testCases.Where(x => x.AutomationStatus == AutomatedName & testMethods.SingleOrDefault(y => y.Name.Equals(x.Title)) == null).ToArray();
        }

        public int[] GetTestCasesId()
        {
            var testPlan = _testManagementHttpClient.GetPlansAsync(_inputOptions.ProjectName).Result.Single(x => x.Name.Equals(_inputOptions.TestPlanName));
            var testSuites = _testManagementHttpClient.GetTestSuitesForPlanAsync(_inputOptions.ProjectName, testPlan.Id).Result;
            var testPoints = _testManagementHttpClient.GetPointsAsync(_inputOptions.ProjectName, testPlan.Id, testSuites[0].Id).Result;

            return testPoints.Select(x => int.Parse(x.TestCase.Id)).ToArray();
        }

        #region GetTestCases

        private static int[][] ChunkTestCases(int[] testCasesId)
        {
            var i = 0;
            var chunkedTestCases = testCasesId.GroupBy(s => i++ / ChunkSize).Select(g => g.ToArray()).ToArray();
            return chunkedTestCases;
        }

        private TestCase[] CreateTestCaseArray(List<WorkItem> workItems)
        {
            return workItems.Select(x => new TestCase(
                    id: (int)x.Id,
                    title: x.Fields[SystemTitle].ToString(),
                    automationStatus: x.Fields[AutomationStatusName].ToString(),
                    automatedTestName: x.Fields.ContainsKey(AutomatedTestName) ? x.Fields[AutomatedTestName].ToString() : null)
                ).ToArray();
        }

        #endregion

        #region Validations

        private bool TestCaseHasAutomatedStatus(TestCase testCase, TestMethod testMethod)
        {
            return testCase.AutomationStatus.Equals(AutomatedName) && testCase.Title.Equals(testMethod.Name);
        }

        private bool TestCaseIsAlreadyAutomated(TestCase testCase, TestMethod testMethod)
        {
            return testCase.AutomatedTestName.Equals($"{testMethod.FullClassName}.{testMethod.Name}");
        }

        #endregion

        #region Assocation

        private bool AssociateTestCaseWithTestMethod(int workItemId, string methodName, string assemblyName, string automatedTestId, string testType)
        {
            var patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"/{FieldProperty}/{AutomatedTestName}",
                    Value = methodName
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path =  $"/{FieldProperty}/{AutomatedTestStorageName}",
                    Value = assemblyName
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"/{FieldProperty}/{AutomatedTestIdName}",
                    Value = automatedTestId
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"/{FieldProperty}/{AutomatedTestTypePatchName}",
                    Value = testType
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"/{FieldProperty}/{AutomationStatusName}",
                    Value = AutomatedName
                }
            };

            var result = _workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemId, _inputOptions.ValidationOnly).Result;

            return result.Fields[AutomationStatusName].ToString() == AutomatedName &&
                   result.Fields[AutomatedTestIdName].ToString() == automatedTestId &&
                   result.Fields[AutomatedTestStorageName].ToString() == assemblyName &&
                   result.Fields[AutomatedTestName].ToString() == methodName;
        }

        #endregion
    }
}
