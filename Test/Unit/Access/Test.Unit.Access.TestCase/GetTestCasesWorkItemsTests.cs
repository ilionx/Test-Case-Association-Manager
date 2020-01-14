using Moq;
using System;
using AutoFixture;
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
    public class GetTestCasesWorkItemsTests
    {
        private const string SystemTitle = "System.Title";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";

        [TestMethod]
        public void DevOpsAccess_GetTestCasesWorkItems_EmptyWorkItems()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            const int testPlanId = 51;
            const string projectName = "projectNameWithNoTestPlans";

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true,
                ProjectName = projectName,
                TestPlanId = testPlanId
            };
            var counter = new Counter();

            testManagementHttpClient
                .Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default))
                .ReturnsAsync(new List<TestPoint>());

            var azureDevOpsHttpClients = new AzureDevOpsHttpClients()
            {
                TestManagementHttpClient = testManagementHttpClient.Object,
                WorkItemTrackingHttpClient = workItemTrackingHttpClient.Object
            };

            var target = new DevOpsAccessFactory(azureDevOpsHttpClients, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.GetTestCaseWorkItems();

            // Assert
            actual.Length.Should().Be(0);
        }

        [TestMethod]
        public void DevOpsAccess_GetTestCaseWorkItems_NotEmptyTestCaseWorkItems()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            var fixture = new Fixture();
            var testPoints = new List<TestPoint>()
            {
                fixture.Build<TestPoint>()
                       .Without(x => x.LastUpdatedBy)
                       .Without(x => x.AssignedTo)
                       .Without(x=> x.LastResultDetails)
                       .With(x => x.TestCase, fixture.Build<WorkItemReference>().With(y => y.Id, fixture.Create<int>().ToString()).Create())
                       .Create(),
                fixture.Build<TestPoint>()
                       .Without(x => x.LastUpdatedBy)
                       .Without(x => x.AssignedTo)
                       .Without(x=> x.LastResultDetails)
                       .With(x => x.TestCase, fixture.Build<WorkItemReference>().With(y => y.Id, fixture.Create<int>().ToString()).Create())
                       .Create(),
                fixture.Build<TestPoint>()
                       .Without(x => x.LastUpdatedBy)
                       .Without(x => x.AssignedTo)
                       .Without(x=> x.LastResultDetails)
                       .With(x => x.TestCase, fixture.Build<WorkItemReference>().With(y => y.Id, fixture.Create<int>().ToString()).Create())
                       .Create()
            };
            var workItems = new List<WorkItem>()
            {
                fixture.Build<WorkItem>().With(x => x.Id, fixture.Create<int>())
                       .With(y => y.Fields, new Dictionary<string, object>(){ { SystemTitle , fixture.Create<string>() }, { AutomationStatusName, fixture.Create<string>() }, { AutomatedTestName, fixture.Create<string>() } })
                       .Create(),
                fixture.Build<WorkItem>()
                       .With(x => x.Id, fixture.Create<int>())
                       .With(y => y.Fields, new Dictionary<string, object>(){ { SystemTitle , fixture.Create<string>() }, { AutomationStatusName, fixture.Create<string>() }, { AutomatedTestName, fixture.Create<string>() } })
                       .Create(),
                fixture.Build<WorkItem>()
                       .With(x => x.Id, fixture.Create<int>())
                       .With(y => y.Fields, new Dictionary<string, object>(){ { SystemTitle , fixture.Create<string>() }, { AutomationStatusName, fixture.Create<string>() }, { AutomatedTestName, fixture.Create<string>() } })
                       .Create(),
            };

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            testManagementHttpClient
                .Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default))
                .ReturnsAsync(testPoints);
            workItemTrackingHttpClient
                .Setup(x => x.GetWorkItemsAsync(It.IsAny<int[]>(), null, null, null, null, null, default))
                .ReturnsAsync(workItems);

            var azureDevOpsHttpClients = new AzureDevOpsHttpClients()
            {
                TestManagementHttpClient = testManagementHttpClient.Object,
                WorkItemTrackingHttpClient = workItemTrackingHttpClient.Object
            };

            var target = new DevOpsAccessFactory(azureDevOpsHttpClients, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.GetTestCaseWorkItems();

            // Assert
            actual.Length.Should().Be(3);
        }
    }
}
