using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Test.Unit.Access.TestCase
{
    [TestClass]
    public class AssociateTestCaseWithTestMethodTests
    {
        private const string AutomatedName = "Automated";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
        private const string AutomatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";

        [TestMethod]
        public void TestCaseAccess_AssociateTestCaseWithTestMethod_Success()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            const string testName = "";
            const string projectName = "";
            const bool validationOnly = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var workItemId = fixture.Create<int>();
            var methodName = fixture.Create<string>();
            var assemblyName = fixture.Create<string>();
            var automatedTestId = fixture.Create<string>();
            var result = new WorkItem
            {
                Fields = new Dictionary<string, object>()
                {
                    { AutomationStatusName, AutomatedName },
                    { AutomatedTestIdName, automatedTestId },
                    { AutomatedTestStorageName, assemblyName },
                    { AutomatedTestName, methodName }
                }
            };

            workItemTrackingHttpClient.Setup(x => x.UpdateWorkItemAsync(It.IsAny<JsonPatchDocument>(), It.IsAny<int>(), It.IsAny<bool?>(), null, null, default(CancellationToken))).ReturnsAsync(result);

            var target = new TestCaseAccessFactory(testManagementHttpClient.Object, workItemTrackingHttpClient.Object, testName, projectName).Create();

            // Act
            var actual = target.AssociateTestCaseWithTestMethod(workItemId, methodName, assemblyName, automatedTestId, validationOnly, testType);

            // Assert
            actual.Should().Be(true);
        }
    }
}
