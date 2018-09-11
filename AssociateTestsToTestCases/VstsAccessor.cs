using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AssociateTestsToTestCases
{
    public class VstsAccessor
    {
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

        public VstsAccessor(string collectionUri, string personalAccessToken)
        {
            var connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential(string.Empty, personalAccessToken));
            _workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
        }

        public List<VstsTestCase> GetVstsTestCases()
        {
            var workItemQuery = _workItemTrackingHttpClient.QueryByWiqlAsync(new Wiql()
            {
                Query =
                    "SELECT * From WorkItems Where [System.WorkItemType] = 'Test Case'"
            }).Result;

            var testcasesId = workItemQuery.WorkItems?.Select(x => x.Id).ToArray();
            var testcases = _workItemTrackingHttpClient.GetWorkItemsAsync(testcasesId).Result;

            return CreateListVstsTestCases(testcases);
        }

        public async Task AssociateWorkItemWithTestMethodAsync(int workItemId, string methodName, string assemblyName, string automatedTestId)
        {
            var patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestName",
                    Value = methodName
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestStorage",
                    Value = assemblyName
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestId",
                    Value = automatedTestId
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.AutomatedTestType",
                    Value = "Unit Test"
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.AutomationStatus",
                    Value = "Automated"
                }
            };

            _workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemId, false).Wait();
        }

        private List<VstsTestCase> CreateListVstsTestCases(List<WorkItem> workItems)
        {
            var vstsTestCases = new List<VstsTestCase>();

            foreach (var workItem in workItems)
            {
                var workItemTitle = workItem.Fields?.FirstOrDefault(x => x.Key == "System.Title").Value.ToString();

                vstsTestCases.Add(new VstsTestCase()
                {
                    Id = workItem.Id,
                    Title = workItemTitle
                });
            }

            return vstsTestCases;
        }
    }
}
