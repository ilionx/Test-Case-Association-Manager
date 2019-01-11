﻿using Moq;
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
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using WorkItemReference = Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class GetTestCasesTests
    {
        private const string SystemTitle = "System.Title";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";

        [TestMethod]
        public void DevOpsAccess_GetTestCases_EmptyTestCases()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            const string testPlanName = "testPlanName";
            const string projectName = "projectNameWithNoTestPlans";

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true,
                ProjectName = projectName,
                TestPlanName = testPlanName
            };
            var counter = new Counter();

            testManagementHttpClient.Setup(x => x.GetPlansAsync(It.IsAny<string>(), null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(new List<TestPlan>());

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            Action act = () => target.GetTestCases();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void DevOpsAccess_GetTestCases_EmptyTestCasesNoMatchTestPlans()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            const string testPlanName = "testPlanName";
            const string notTestPlanName = "NotTestPlanName";
            const string projectName = "projectNameWithTestPlans";

            var fixture = new Fixture();
            var testPlans = new List<TestPlan>()
            {
                fixture.Build<TestPlan>().With(x => x.Name, notTestPlanName).Create(),
                fixture.Build<TestPlan>().With(x => x.Name, notTestPlanName).Create()
            };

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true,
                ProjectName = projectName,
                TestPlanName = testPlanName
            };
            var counter = new Counter();

            testManagementHttpClient.Setup(x => x.GetPlansAsync(It.IsAny<string>(), null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testPlans);

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            Action act = () => target.GetTestCases();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void DevOpsAccess_GetTestCases_NotEmptyTestCases()
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
            var testCases = new List<WorkItem>()
            {
                fixture.Build<WorkItem>().With(x => x.Id, fixture.Create<int>()).With(y => y.Fields, new Dictionary<string, object>(){ { SystemTitle , fixture.Create<string>() }, { AutomationStatusName, fixture.Create<string>() }, { AutomatedTestName, fixture.Create<string>() } }).Create(),
                fixture.Build<WorkItem>().With(x => x.Id, fixture.Create<int>()).With(y => y.Fields, new Dictionary<string, object>(){ { SystemTitle , fixture.Create<string>() }, { AutomationStatusName, fixture.Create<string>() }, { AutomatedTestName, fixture.Create<string>() } }).Create(),
                fixture.Build<WorkItem>().With(x => x.Id, fixture.Create<int>()).With(y => y.Fields, new Dictionary<string, object>(){ { SystemTitle , fixture.Create<string>() }, { AutomationStatusName, fixture.Create<string>() }, { AutomatedTestName, fixture.Create<string>() } }).Create(),
            };

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true,
                ProjectName = projectName,
                TestPlanName = testPlanName
            };
            var counter = new Counter();

            testManagementHttpClient.Setup(x => x.GetPlansAsync(It.IsAny<string>(), null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testPlans);
            testManagementHttpClient.Setup(x => x.GetTestSuitesForPlanAsync(It.IsAny<string>(), It.IsAny<int>(), null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testSuites);
            testManagementHttpClient.Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testPoints);
            workItemTrackingHttpClient.Setup(x => x.GetWorkItemsAsync(It.IsAny<int[]>(), null, null, null, null, null, default(CancellationToken))).ReturnsAsync(testCases);

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.GetTestCases();

            // Assert
            actual.Length.Should().Be(3);
        }
    }
}
