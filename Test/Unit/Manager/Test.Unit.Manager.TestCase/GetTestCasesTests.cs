using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Test.Unit.Manager.DevOps
{
    [TestClass]
    public class GetTestCasesTests
    {
        private const string SystemTitle = "System.Title";
        private const string AutomatedTestName = "Microsoft.VSTS.TCM.AutomatedTestName";
        private const string AutomationStatusName = "Microsoft.VSTS.TCM.AutomationStatus";

        private const int DefaultWriteCount = 2;

        [TestMethod]
        public void DevOpsManager_GetTestCases_TestCasesIsNullOrEmpty()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var message = fixture.Create<string>();
            var messageType = fixture.Create<string>();
            var messageReason = fixture.Create<string>();
            var workItems = new WorkItem[0];

            var counter = new Counter();

            devOpsAccessMock.Setup(x => x.GetTestCaseWorkItems()).Returns(workItems);
            outputManagerMock.Setup(x => x.WriteToConsole(message, messageType, messageReason));

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            Action actual = () => target.GetTestCases();

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }

        [TestMethod]
        public void DevOpsManager_GetTestCases_DuplicateTestCasesCountIsNotEqualToZero()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var message = fixture.Create<string>();
            var messageType = fixture.Create<string>();
            var messageReason = fixture.Create<string>();
            var duplicateTestCases = fixture.Create<List<DuplicateTestCase>>();
            var workItems = fixture.Create<List<WorkItem>>();
            workItems.ForEach(x =>
            {
                x.Fields[SystemTitle] = fixture.Create<string>();
                x.Fields[AutomatedTestName] = fixture.Create<string>();
                x.Fields[AutomationStatusName] = fixture.Create<string>();
            });

            var counter = new Counter();

            devOpsAccessMock.Setup(x => x.GetTestCaseWorkItems()).Returns(workItems.ToArray);
            devOpsAccessMock.Setup(x => x.ListDuplicateTestCases(It.IsAny<TestCase[]>())).Returns(duplicateTestCases);
            outputManagerMock.Setup(x => x.WriteToConsole(message, messageType, messageReason));

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            Action actual = () => target.GetTestCases();

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount + duplicateTestCases.Count));
        }

        [TestMethod]
        public void DevOpsManager_GetTestCases_Success()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var message = fixture.Create<string>();
            var messageType = fixture.Create<string>();
            var messageReason = fixture.Create<string>();
            var duplicateTestCases = new List<DuplicateTestCase>();
            var workItems = fixture.Create<List<WorkItem>>();
            workItems.ForEach(x =>
            {
                x.Fields[SystemTitle] = fixture.Create<string>();
                x.Fields[AutomatedTestName] = fixture.Create<string>();
                x.Fields[AutomationStatusName] = fixture.Create<string>();
            });

            var counter = new Counter();

            devOpsAccessMock.Setup(x => x.GetTestCaseWorkItems()).Returns(workItems.ToArray());
            devOpsAccessMock.Setup(x => x.ListDuplicateTestCases(It.IsAny<TestCase[]>())).Returns(duplicateTestCases);
            outputManagerMock.Setup(x => x.WriteToConsole(message, messageType, messageReason));

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            var actual = target.GetTestCases();

            // Assert
            actual.Should().NotBeNullOrEmpty();
            actual.Length.Should().Be(workItems.Count);
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }
    }
}
