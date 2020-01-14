using System.Linq;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using TestMethod = AssociateTestsToTestCases.Manager.File.TestMethod;

namespace AssociateTestsToTestCases.Access.DevOps
{
    public class AzureDevOpsAccess : IDevOpsAccess
    {
        private const int ChunkSize = 200;
        private const string FieldProperty = "fields";
        private const string AutomatedName = "Automated";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestTypePatchName = "Microsoft.VSTS.TCM.AutomatedTestType";
        private const string AutomatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";

        private readonly AzureDevOpsHttpClients _azureDevOpsHttpClients;

        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;

        private readonly InputOptions _inputOptions;
        private readonly Counter.Counter _counter;

        public AzureDevOpsAccess(AzureDevOpsHttpClients azureDevOpsHttpClients, Messages messages, IOutputAccess outputAccess, InputOptions options, Counter.Counter counter)
        {
            _azureDevOpsHttpClients = azureDevOpsHttpClients;

            _messages = messages;
            _outputAccess = outputAccess;

            _inputOptions = options;
            _counter = counter;
        }

        public WorkItem[] GetTestCaseWorkItems()
        {
            var chunkedTestCases = ChunkTestCases(GetTestCasesId());

            return chunkedTestCases
                .SelectMany(chunkedTestgroupCases => _azureDevOpsHttpClients.WorkItemTrackingHttpClient.GetWorkItemsAsync(chunkedTestgroupCases).Result)
                .ToArray();
        }

        public int Associate(TestMethod[] testMethods, Dictionary<string, TestCase> testCases)
        {
            foreach (var testMethod in testMethods)
            {
                var testCase = GetTestCase(testCases, testMethod);
                var testCaseNotFound = testCase == null;
                if (testCaseNotFound)
                {
                    _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestMethodInfo, testMethod.Name, $"{testMethod.FullClassName}.{testMethod.Name}"), _messages.Types.Error, _messages.Reasons.MissingTestCase);
                    _counter.Error.TestCaseNotFound++;
                    continue;
                }

                bool? testCaseHasIncorrectAssociation = null;
                var testCaseHasAutomatedStatus = testCase.AutomationStatus.Equals(AutomatedName);
                if (testCaseHasAutomatedStatus)
                {
                    var testCaseIsAlreadyAutomated = testCase.AutomatedTestName.Equals($"{testMethod.FullClassName}.{testMethod.Name}");
                    if (testCaseIsAlreadyAutomated)
                    {
                        _counter.Unaffected.AlreadyAutomated++;
                        continue;
                    }

                    testCaseHasIncorrectAssociation = !testCaseIsAlreadyAutomated;
                    if (testCaseHasIncorrectAssociation.Value && _inputOptions.VerboseLogging)
                    {
                        _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestCaseInfo, testCase.Title, testCase.Id), _messages.Types.Info, _messages.Reasons.FixedAssociationTestCase);
                    }
                }

                var operationSuccess = AssociateTestCaseWithTestMethod(testCase.Id, $"{testMethod.FullClassName}.{testMethod.Name}", testMethod.AssemblyName, testMethod.TempId.ToString(), _inputOptions.TestType);
                if (!operationSuccess)
                {
                    _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestCaseInfo, testCase.Title, testCase.Id), _messages.Types.Failure, _messages.Reasons.Association);
                    _counter.Error.OperationFailed++;
                    continue;
                }

                if (_inputOptions.VerboseLogging)
                {
                    _outputAccess.WriteToConsole(string.Format(_messages.Associations.TestMethodMapped, testMethod.Name, testCase.Id), _messages.Types.Success, _messages.Reasons.Association);
                }

                if (testCaseHasIncorrectAssociation.HasValue && testCaseHasIncorrectAssociation.Value)
                {
                    _counter.Success.FixedReference++;
                }
                _counter.Success.Total++;
            }

            return _counter.Error.Total;
        }

        public List<DuplicateTestCase> ListDuplicateTestCases(TestCase[] testCases)
        {
            var duplicates = testCases.Select(x => x.Title).GroupBy(x => x).Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            return duplicates
                .Select(x => new DuplicateTestCase(x, testCases
                .Where(y => y.Title.Equals(x)).ToArray()))
                .ToList();
        }

        public List<TestCase> ListTestCasesWithNotAvailableTestMethods(TestMethod[] testMethods, TestCase[] testCases)
        {
            return testCases
                .Where(x => x.AutomationStatus == AutomatedName & testMethods.SingleOrDefault(y => y.Name.Equals(x.Title)) == null)
                .ToList();
        }

        public int[] GetTestCasesId()
        {
            var testPoints = _azureDevOpsHttpClients.TestManagementHttpClient.GetPointsAsync(_inputOptions.ProjectName, _inputOptions.TestPlanId, _inputOptions.TestSuiteId).Result;

            return testPoints.Select(x => int.Parse(x.TestCase.Id)).ToArray();
        }

        private static int[][] ChunkTestCases(int[] testCasesId)
        {
            var i = 0;
            var chunkedTestCases = testCasesId
                .GroupBy(s => i++ / ChunkSize)
                .Select(g => g.ToArray())
                .ToArray();

            return chunkedTestCases;
        }

        private TestCase GetTestCase(Dictionary<string, TestCase> testCases, TestMethod testMethod)
        {
            testCases.TryGetValue(testMethod.Name, out var testCase);

            return testCase;
        }

        private bool AssociateTestCaseWithTestMethod(int workItemId, string methodName, string assemblyName, string automatedTestId, string testType)
        {
            var patchDocument = CreatePatchDocument(methodName, assemblyName, automatedTestId, testType);
            var result = _azureDevOpsHttpClients.WorkItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemId, _inputOptions.ValidationOnly).Result;

            return OperationIsSuccess(methodName, assemblyName, automatedTestId, result);
        }

        private JsonPatchDocument CreatePatchDocument(string methodName, string assemblyName, string automatedTestId, string testType)
        {
            return new JsonPatchDocument
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
        }

        private bool OperationIsSuccess(string methodName, string assemblyName, string automatedTestId, WorkItem result)
        {
            return result.Fields[AutomationStatusName].ToString() == AutomatedName &&
                   result.Fields[AutomatedTestIdName].ToString() == automatedTestId &&
                   result.Fields[AutomatedTestStorageName].ToString() == assemblyName &&
                   result.Fields[AutomatedTestName].ToString() == methodName;
        }
    }
}
