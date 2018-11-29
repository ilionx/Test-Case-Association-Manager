using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AssociateTestsToTestCases.Access.TestCase
{
    public class TestCaseAccess : ITestCaseAccess
    {
        private readonly TestManagementHttpClient _testManagementHttpClient;
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

        private readonly string _projectName;
        private readonly string _testPlanName;

        private const int ChunkSize = 200;
        private const string FieldProperty = "fields";
        private const string AutomatedName = "Automated";
        private const string SystemTitle = "System.Title";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestTypePatchName = "Microsoft.VSTS.TCM.AutomatedTestType";
        private const string AutomatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";
        
        public TestCaseAccess(TestManagementHttpClient testManagementHttpClient, WorkItemTrackingHttpClient workItemTrackingHttpClient, string testName, string projectName)
        {
            _testManagementHttpClient = testManagementHttpClient;
            _workItemTrackingHttpClient = workItemTrackingHttpClient;

            _testPlanName = testName;
            _projectName = projectName;
        }

        public List<TestCase> GetTestCases()
        {
            var testCasesId = GetTestCasesId();

            var chunkedTestCases = ChunkTestCases(testCasesId);

            var testCases = chunkedTestCases.SelectMany(chunkedTestgroupCases => _workItemTrackingHttpClient.GetWorkItemsAsync(chunkedTestgroupCases).Result).ToList();

            return CreateTestCaseList(testCases);
        }

        public bool AssociateTestCaseWithTestMethod(int workItemId, string methodName, string assemblyName, string automatedTestId, bool validationOnly, string testType)
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

            var result = _workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemId, validationOnly).Result;

            return result.Fields[AutomationStatusName].ToString() == AutomatedName &&
                   result.Fields[AutomatedTestIdName].ToString() == automatedTestId &&
                   result.Fields[AutomatedTestStorageName].ToString() == assemblyName &&
                   result.Fields[AutomatedTestName].ToString() == methodName;
        }

        public List<DuplicateTestCase> ListDuplicateTestCases(List<TestCase> testCases)
        {
            var duplicateTestCases = new List<DuplicateTestCase>();

            var duplicates = testCases.Select(x => x.Title).GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            foreach (var duplicate in duplicates)
            {
                duplicateTestCases.Add(new DuplicateTestCase(duplicate, testCases.Where(y => y.Title.Equals(duplicate)).ToArray()));
            }

            return duplicateTestCases;
        }

        private int[] GetTestCasesId()
        {
            var testPlan = _testManagementHttpClient.GetPlansAsync(_projectName).Result
                .Single(x => x.Name.Equals(_testPlanName));

            var testSuites = _testManagementHttpClient.GetTestSuitesForPlanAsync(_projectName, testPlan.Id).Result;

            return _testManagementHttpClient.GetPointsAsync(_projectName, testPlan.Id, testSuites[0].Id).Result
                .Select(x => int.Parse(x.TestCase.Id)).ToArray();
        }

        private static int[][] ChunkTestCases(int[] testCasesId)
        {
            var i = 0;
            var chunkedTestCases = testCasesId.GroupBy(s => i++ / ChunkSize).Select(g => g.ToArray()).ToArray();
            return chunkedTestCases;
        }

        private List<TestCase> CreateTestCaseList(List<WorkItem> workItems)
        {
            var testCases = new List<TestCase>();

            foreach (var workItem in workItems)
            {
                testCases.Add(new TestCase(
                    id: (int)workItem.Id,
                    title: workItem.Fields[SystemTitle].ToString(),
                    automationStatus: workItem.Fields[AutomationStatusName].ToString(),
                    automatedTestName: workItem.Fields.ContainsKey(AutomatedTestName) ? workItem.Fields[AutomatedTestName].ToString() : null)
                );
            }

            return testCases;
        }
    }
}
