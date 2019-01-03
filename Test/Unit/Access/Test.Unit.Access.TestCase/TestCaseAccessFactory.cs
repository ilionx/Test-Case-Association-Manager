using AssociateTestsToTestCases;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Test.Unit.Access.DevOps
{
    public class DevOpsAccessFactory
    {
        private readonly TestManagementHttpClient _testManagementHttpClient;
        private readonly WorkItemTrackingHttpClient _workItemTrackingHttpClient;


        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        private readonly InputOptions _options;
        private readonly Counter _counter;

        public DevOpsAccessFactory(TestManagementHttpClient testManagementHttpClient, WorkItemTrackingHttpClient workItemTrackingHttpClient, Messages messages, IOutputAccess outputAccess, InputOptions options, Counter counter)
        {
            _testManagementHttpClient = testManagementHttpClient;
            _workItemTrackingHttpClient = workItemTrackingHttpClient;

            _messages = messages;
            _outputAccess = outputAccess;
            _options = options;
            _counter = counter;
        }

        public IDevOpsAccess Create()
        {

            return new AzureDevOpsAccess(_testManagementHttpClient, _workItemTrackingHttpClient, _messages, _outputAccess, _options, _counter);
        }
    }
}
