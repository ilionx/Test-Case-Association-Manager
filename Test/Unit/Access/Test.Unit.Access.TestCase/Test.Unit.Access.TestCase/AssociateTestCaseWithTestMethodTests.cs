using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Test.Unit.Access.TestCase
{
    [TestClass]
    public class AssociateTestCaseWithTestMethodTests
    {
        [TestMethod]
        public void TestCaseAccess_AssociateTestCaseWithTestMethod_Success()
        {
            // Arrange
            var testManagementHttpClient = new Mock<TestManagementHttpClient>(new Uri("http://dummy.url"), new VssCredentials());
            var workItemTrackingHttpClient = new Mock<WorkItemTrackingHttpClient>(new Uri("http://dummy.url"), new VssCredentials());

            const string testName = "";
            const string projectName = "";
            const bool validationOnly = true;
            const string automatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
            const string automatedTestIdName = "Microsoft.VSTS.TCM.AutomatedTestId";
            const string automationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";
            const string automatedTestStorageName = "Microsoft.VSTS.TCM.AutomatedTestStorage";

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
                    { automationStatusName, "Automated" },
                    { automatedTestIdName, automatedTestId },
                    { automatedTestStorageName, assemblyName },
                    { automatedTestName, methodName }
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
