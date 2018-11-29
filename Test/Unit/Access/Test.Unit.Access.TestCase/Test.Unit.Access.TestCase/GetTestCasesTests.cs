using Moq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Test.Unit.Access.TestCase
{
    [TestClass]
    public class GetTestCasesTests
    {
        // GetTestCases
        [TestMethod]
        public void TestCaseAccess_GetTestCases_EmptyTestCases()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>();
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>();

            const string testName = "";
            const string projectName = "";

            var testCases = new List<AssociateTestsToTestCases.Access.TestCase.TestCase>();

            var target = new TestCaseAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, testName, projectName).Create();

            // Act
            var actual = target.ListDuplicateTestCases(testCases);

            // Assert
        }

        [TestMethod]
        public void TestCaseAccess_GetTestCases_NotEmptyTestCases()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>();
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>();

            const string testName = "";
            const string projectName = "";

            var testCases = new List<AssociateTestsToTestCases.Access.TestCase.TestCase>();

            var target = new TestCaseAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, testName, projectName).Create();

            // Act
            var actual = target.ListDuplicateTestCases(testCases);

            // Assert
        }
    }
}
