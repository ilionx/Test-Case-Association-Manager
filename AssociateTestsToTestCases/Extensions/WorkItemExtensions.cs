using System.Linq;
using AssociateTestsToTestCases.Access.DevOps;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AssociateTestsToTestCases.Extensions
{
    public static class WorkItemExtensions
    {
        private const string SystemTitle = "System.Title";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";

        public static TestCase[] ToTestCaseArray(this WorkItem[] workItems)
        {
            return workItems.Select(x => new TestCase(
                id: (int)x.Id,
                title: x.Fields[SystemTitle].ToString(),
                automationStatus: x.Fields[AutomationStatusName].ToString(),
                automatedTestName: x.Fields.ContainsKey(AutomatedTestName) ? x.Fields[AutomatedTestName].ToString() : null)
            ).ToArray();
        }
    }
}
