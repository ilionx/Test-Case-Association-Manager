using Moq;
using System;
using AutoFixture;
using System.Threading;
using FluentAssertions;
using AssociateTestsToTestCases;
using System.Collections.Generic;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using Microsoft.VisualStudio.Services.Common;
using AssociateTestsToTestCases.Access.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using WorkItemReference = Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class GetTestCasesId
    {
        [TestMethod]
        public void DevOpsAccess_GetTestCasesId_EmptyArrayTestCasesId()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            const string testPlanName = "TestPlanName";
            const string notTestPlanName = "NotTestPlanName";
            const string projectName = "projectNameWithTestPlans";

            var fixture = new Fixture();
            var testPlans = new List<TestPlan>()
            {
                fixture.Build<TestPlan>().With(x => x.Name, testPlanName).Create(),
                fixture.Build<TestPlan>().With(x => x.Name, notTestPlanName).Create()
            };
            var testSuites = new List<TestSuite>()
            {
                fixture.Build<TestSuite>().With(x => x.Children, null).Create(),
                fixture.Build<TestSuite>().With(x => x.Children, null).Create(),
                fixture.Build<TestSuite>().With(x => x.Children, null).Create()
            };

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true,
                ProjectName = projectName,
                TestPlanName = testPlanName
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            testManagementHttpClient.Setup(x => x.GetPlansAsync(It.IsAny<string>(),null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testPlans);
            testManagementHttpClient.Setup(x => x.GetTestSuitesForPlanAsync(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testSuites);
            testManagementHttpClient.Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(new List<TestPoint>());

            // Act
            var actual = target.GetTestCasesId();

            // Assert
            actual.Length.Should().Be(0);
        }

        [TestMethod]
        public void DevOpsAccess_GetTestCasesId_NotEmptyArrayTestCasesId()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            const string testPlanName = "TestPlanName";
            const string notTestPlanName = "NotTestPlanName";
            const string projectName = "projectNameWithTestPlans";

            var fixture = new Fixture();
            var testPlans = new List<TestPlan>()
            {
                fixture.Build<TestPlan>().With(x => x.Name, testPlanName).Create(),
                fixture.Build<TestPlan>().With(x => x.Name, notTestPlanName).Create()
            };
            var testSuites = new List<TestSuite>()
            {
                fixture.Build<TestSuite>().With(x => x.Children, null).Create(),
                fixture.Build<TestSuite>().With(x => x.Children, null).Create(),
                fixture.Build<TestSuite>().With(x => x.Children, null).Create()
            };
            var testPoints = new List<TestPoint>()
            {
                fixture.Build<TestPoint>().With(x => x.TestCase, fixture.Build<WorkItemReference>().With(y => y.Id, fixture.Create<int>().ToString()).Create()).Create(),
                fixture.Build<TestPoint>().With(x => x.TestCase, fixture.Build<WorkItemReference>().With(y => y.Id, fixture.Create<int>().ToString()).Create()).Create(),
                fixture.Build<TestPoint>().With(x => x.TestCase, fixture.Build<WorkItemReference>().With(y => y.Id, fixture.Create<int>().ToString()).Create()).Create()
            };

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true,
                ProjectName = projectName,
                TestPlanName = testPlanName
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            testManagementHttpClient.Setup(x => x.GetPlansAsync(It.IsAny<string>(), null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testPlans);
            testManagementHttpClient.Setup(x => x.GetTestSuitesForPlanAsync(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testSuites);
            testManagementHttpClient.Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testPoints);

            // Act
            var actual = target.GetTestCasesId();

            // Assert
            actual.Length.Should().Be(testPoints.Count);
        }
    }
}
