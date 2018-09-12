﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AssociateTestsToTestCases
{
    public class TestCaseAccess
    {
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedName = "Automated";

        private const string AutomationStatusPatchName = "/fields/Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestIdPatchName = "/fields/Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomationTestStoragePatchName = "/fields/Microsoft.VSTS.TCM.AutomatedTestStorage";
        private const string AutomationTestNamePatchName = "/fields/Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedTestTypePatchName = "/fields/Microsoft.VSTS.TCM.AutomatedTestType";

        public TestCaseAccess(string collectionUri, string personalAccessToken)
        {
            var connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(string.Empty, personalAccessToken));
            _workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
        }

        public List<TestCase> GetVstsTestCases()
        {
            var workItemQuery = _workItemTrackingHttpClient.QueryByWiqlAsync(new Wiql()
            {
                Query =
                    "SELECT * From WorkItems Where [System.WorkItemType] = 'Test Case'"
            }).Result;

            var testCasesId = workItemQuery.WorkItems?.Select(x => x.Id).ToArray();
            var testCases = _workItemTrackingHttpClient.GetWorkItemsAsync(testCasesId).Result;

            return CreateTestCaseList(testCases);
        }

        public bool AssociateTestCaseWithTestMethod(int workItemId, string methodName, string assemblyName, string automatedTestId, string testType = "")
        {
            var patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = AutomationTestNamePatchName,
                    Value = methodName
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = AutomationTestStoragePatchName,
                    Value = assemblyName
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = AutomatedTestIdPatchName,
                    Value = automatedTestId
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = AutomatedTestTypePatchName,
                    Value = testType
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = AutomationStatusPatchName,
                    Value = AutomatedName
                }
            };

            var result = _workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemId, true).Result;

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
                var workItemTitle = workItem.Fields?.FirstOrDefault(x => x.Key == "System.Title").Value.ToString();

                if (workItem.Fields[AutomationStatusName].ToString() == AutomatedName)
                {
                    continue;
                }

                testCases.Add(new TestCase()
                {
                    Id = workItem.Id,
                    Title = workItemTitle
                });
            }

            return testCases;
        }
    }
}
