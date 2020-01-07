﻿using Moq;
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

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var azureDevOpsHttpClients = new AzureDevOpsHttpClients()
            {
                TestManagementHttpClient = testManagementHttpClient.Object,
                WorkItemTrackingHttpClient = workItemTrackingHttpClient.Object
            };

            var target = new DevOpsAccessFactory(azureDevOpsHttpClients, messages, outputAccess.Object, options, counter).Create();

            testManagementHttpClient
                .Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default))
                .ReturnsAsync(new List<TestPoint>());

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

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var azureDevOpsHttpClients = new AzureDevOpsHttpClients()
            {
                TestManagementHttpClient = testManagementHttpClient.Object,
                WorkItemTrackingHttpClient = workItemTrackingHttpClient.Object
            };

            var target = new DevOpsAccessFactory(azureDevOpsHttpClients, messages, outputAccess.Object, options, counter).Create();

            testManagementHttpClient
                .Setup(x => x.GetPointsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null, null, null, null, default))
                .ReturnsAsync(testPoints);

            // Act
            var actual = target.GetTestCasesId();

            // Assert
            actual.Length.Should().Be(testPoints.Count);
        }
    }
}
