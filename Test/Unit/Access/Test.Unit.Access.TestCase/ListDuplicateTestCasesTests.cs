using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using AssociateTestsToTestCases;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Counter;
using Microsoft.VisualStudio.Services.Common;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class ListDuplicateTestCasesTests
    {
        [TestMethod]
        public void DevOpsAccess_ListDuplicateTestCases_EmptyDuplicateTestCases()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            var fixture = new Fixture();
            var testCases = fixture.Create<TestCase[]>();

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.ListDuplicateTestCases(testCases);

            // Assert
            actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void DevOpsAccess_ListDuplicateTestCases_NotEmptyDuplicateTestCases()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();
            var messages = new Messages();

            var fixture = new Fixture();
            var testCases = new List<TestCase>();
            var testCasesToBeDuplicated = fixture.Create<TestCase[]>();
            testCases.AddRange(testCasesToBeDuplicated);
            testCases.AddRange(testCasesToBeDuplicated);

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.ListDuplicateTestCases(testCases.ToArray());

            // Assert
            actual.Count.Should().Be(testCases.Count / 2);
        }
    }
}
