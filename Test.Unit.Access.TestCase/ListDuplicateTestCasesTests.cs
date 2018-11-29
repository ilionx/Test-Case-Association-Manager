using System;
using System.Collections.Generic;
using AutoFixture;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test.Unit.Access.TestCase
{
    [TestClass]
    public class ListDuplicateTestCasesTests
    {
        [TestMethod]
        public void TestCaseAccess_ListDuplicateTestCases_EmptyDuplicateTestCases()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            const string testName = "";
            const string projectName = "";

            var fixture = new Fixture();
            var testCases = fixture.Create<List<AssociateTestsToTestCases.Access.TestCase.TestCase>>();

            var abc = workItemTrackingHttpClient.Object;

            var f = 0;
            //var target = new TestCaseAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, testName, projectName).Create();

            //// Act
            //var actual = target.ListDuplicateTestCases(testCases);

            //// Assert
            //actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void TestCaseAccess_ListDuplicateTestCases_NotEmptyDuplicateTestCases()
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
