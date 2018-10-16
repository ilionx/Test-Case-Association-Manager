using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AssociateTestsToTestCases
{
    public class TestCaseAccess
    {
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;
        private readonly TestManagementHttpClient _testManagementHttpClient;

        private const string ProjectName = "GGR";
        private const string FieldProperty = "fields";
        private const string AutomatedName = "Automated";
        private const string SystemTitle = "System.Title";
        private const string SystemTestPlanName = "System Test Plan";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestTypePatchName = "Microsoft.VSTS.TCM.AutomatedTestType";
        private const string AutomatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";
        
        public TestCaseAccess(string collectionUri, string personalAccessToken)
        {
            var connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(string.Empty, personalAccessToken));
            _workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            _testManagementHttpClient = connection.GetClient<TestManagementHttpClient>();
        }

        public List<TestCase> GetVstsTestCases()
        {
            var testCasesId = GetTestCasesId();

            var chunkedTestCases = ChunkTestCases(testCasesId);

            var testCases = chunkedTestCases.SelectMany(chunkedTestgroupCases => _workItemTrackingHttpClient.GetWorkItemsAsync(chunkedTestgroupCases).Result).ToList();

            return CreateTestCaseList(testCases);
        }

        private int[] GetTestCasesId()
        {
            var testPlan = _testManagementHttpClient.GetPlansAsync(ProjectName).Result
                .Single(x => x.Name.Equals(SystemTestPlanName));

            var testSuites = _testManagementHttpClient.GetTestSuitesForPlanAsync(ProjectName, testPlan.Id).Result;

            return _testManagementHttpClient.GetPointsAsync(ProjectName, testPlan.Id, testSuites[0].Id).Result
                .Select(x => int.Parse(x.TestCase.Id)).ToArray();
        }

        private static int[][] ChunkTestCases(int[] testCasesId)
        {
            var i = 0;
            var chunkSize = 200;
            var chunkedTestCases = testCasesId.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();
            return chunkedTestCases;
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

        private List<TestCase> CreateTestCaseList(List<WorkItem> workItems)
        {
            var testCases = new List<TestCase>();

            foreach (var workItem in workItems)
            {
                testCases.Add(new TestCase()
                {
                    Id = workItem.Id,
                    Title = workItem.Fields[SystemTitle].ToString(),
                    AutomationStatus = workItem.Fields[AutomationStatusName].ToString()
                });
            }

            return testCases;
        }

        public bool ResetStatusTestCases()
        {
            //var testPlans = _testManagementHttpClient.GetPlansAsync(ProjectName).Result;
            //var testSuites = _testManagementHttpClient.GetTestSuitesForPlanAsync(ProjectName, testPlans[0].Id).Result;

            //// option 1
            //var pointUpdateModel = new PointUpdateModel(resetToActive: true); // if this doesn't work, then set Outcome to In progress?

            //// option 2

            ////var testPoints = _testManagementHttpClient.GetPointsAsync(ProjectName, testPlans[0].Id, testSuites[0].Id).Result;

            ////foreach (var testPoint in testPoints)
            ////{
            ////    testPoint.State = TestPointState.InProgress.ToString(); // InProgress = for testing purpose.
            ////}


            //// sets status to active for 1 testpoint
            //var updatedTestPoints = _testManagementHttpClient.UpdateTestPointsAsync(pointUpdateModel, ProjectName, testPlans[0].Id, testSuites[0].Id, "1").Result; // todo: for testing-purpose: set's outcome test point 1 linked to test case 53

            //if (updatedTestPoints)
            //{
                
            //}
            return true;
        }
    }
}
