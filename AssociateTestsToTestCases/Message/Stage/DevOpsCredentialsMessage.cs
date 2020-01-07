using System;

namespace AssociateTestsToTestCases.Message.Stage
{
    public class DevOpsCredentialsMessage
    {
        public readonly string Status = "Validating AzureDevOps credentials...";
        public readonly string Success = "Azure DevOps credentials have been validated.\n";
        public readonly string FailureUserNotAuthorized = "You are not authorized to access '{0}'.";
        public readonly string FailureNonExistingProject = "Project '{0}' does not exist.";
        public readonly string FailureNonExistingTestPlan = "Test Plan with Id '{0}' does not exist.";
        public readonly string FailureNonExistingTestSuite = "Test Suite with Id '{0}' does not exist.";
    }
}
