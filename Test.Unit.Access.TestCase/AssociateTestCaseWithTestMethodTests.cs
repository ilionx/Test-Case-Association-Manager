using System.Collections.Generic;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test.Unit.Access.TestCase
{
    [TestClass]
    public class AssociateTestCaseWithTestMethodTests
    {
        [TestMethod]
        public void TestCaseAccess_AssociateTestCaseWithTestMethod_Fail()
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
        public void TestCaseAccess_AssociateTestCaseWithTestMethod_Success()
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
