using AssociateTestsToTestCases.Access.TestCase;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Test.Unit.Access.TestCase
{
    public class TestCaseAccessFactory
    {
        private readonly TestManagementHttpClient _testManagementHttpClient;
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;

        private readonly string _testName;
        private readonly string _projectName;
        public TestCaseAccessFactory(TestManagementHttpClient testManagementHttpClient, WorkItemTrackingHttpClient workItemTrackingHttpClient, string testName, string projectName)
        {
            _testManagementHttpClient = testManagementHttpClient;
            _workItemTrackingHttpClient = workItemTrackingHttpClient;

            _testName = testName;
            _projectName = projectName;
        }

        public ITestCaseAccess Create()
        {
            return new TestCaseAccess(_testManagementHttpClient, _workItemTrackingHttpClient, _testName, _projectName);
        }
    }
}
