using Moq;
using System;
using AutoFixture;
using System.Linq;
using FluentAssertions;
using AssociateTestsToTestCases;
using System.Collections.Generic;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using Microsoft.VisualStudio.Services.Common;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class ListTestCasesWithNotAvailableTestMethodsTests
    {
        private const string AutomatedName = "Automated";
        private const string NotAutomatedName = "Not Automated";

        [TestMethod]
        public void DevOpsAccess_ListTestCasesWithNotAvailableTestMethods_EmptyListTestCasesWithNotAvailableTestMethodsWhereAutomationStatusIsEqualToAutomatedName()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var messages = new Messages();

            fixture.Customize<TestCase>(c => c.With(x => x.AutomationStatus, AutomatedName));
            var testCases =  fixture.Create<List<TestCase>>();
            var testMethods = testCases.Select(x => new AssociateTestsToTestCases.Access.DevOps.TestMethod(x.Title, string.Empty, string.Empty, Guid.NewGuid())).ToList();

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);

            // Assert
            actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void DevOpsAccess_ListTestCasesWithNotAvailableTestMethods_EmptyListTestCasesWithNotAvailableTestMethodsWhereAutomationStatusIsNotEqualToAutomatedName()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var messages = new Messages();

            fixture.Customize<TestCase>(c => c.With(x => x.AutomationStatus, NotAutomatedName));
            var testCases = fixture.Create<List<TestCase>>();
            var testMethods = fixture.Create<List<AssociateTestsToTestCases.Access.DevOps.TestMethod>>();

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);

            // Assert
            actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void DevOpsAccess_ListTestCasesWithNotAvailableTestMethods_ListTestCasesWithNotAvailableTestMethods()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            var outputAccess = new Mock<IOutputAccess>();

            var messages = new Messages();
            var fixture = new Fixture();

            fixture.Customize<TestCase>(c => c.With(x => x.AutomationStatus, AutomatedName));
            var testCases = fixture.Create<List<TestCase>>();
            var testMethods = testCases.Select(x => new AssociateTestsToTestCases.Access.DevOps.TestMethod(string.Empty, string.Empty, string.Empty, Guid.NewGuid())).ToList();

            var options = new InputOptions()
            {
                ValidationOnly = true,
                VerboseLogging = true
            };
            var counter = new Counter();

            var target = new DevOpsAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, messages, outputAccess.Object, options, counter).Create();

            // Act
            var actual = target.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);

            // Assert
            actual.Count.Should().Be(3);
        }
    }
}
