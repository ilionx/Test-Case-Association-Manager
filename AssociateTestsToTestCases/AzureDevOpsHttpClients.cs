using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace AssociateTestsToTestCases
{
    public class AzureDevOpsHttpClients
    {
        public WorkItemTrackingHttpClient WorkItemTrackingHttpClient { get; set; }
        public TestManagementHttpClient TestManagementHttpClient { get; set; }
    }
}
