using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.DevOps
{
    [TestClass]
    public class GetTestCasesTests
    {
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
            var testCases = new TestCase[0];

            var counter = new Counter();

            devOpsAccessMock.Setup(x => x.GetTestCases()).Returns(testCases);
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
            var testCases = fixture.Create<TestCase[]>();

            var counter = new Counter();

            devOpsAccessMock.Setup(x => x.GetTestCases()).Returns(testCases);
            devOpsAccessMock.Setup(x => x.ListDuplicateTestCases(testCases)).Returns(duplicateTestCases);
            outputManagerMock.Setup(x => x.WriteToConsole(message, messageType, messageReason));

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            Action actual = () =>  target.GetTestCases();

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
            var testCases = fixture.Create<TestCase[]>();

            var counter = new Counter();

            devOpsAccessMock.Setup(x => x.GetTestCases()).Returns(testCases);
            outputManagerMock.Setup(x => x.WriteToConsole(message, messageType, messageReason));
            devOpsAccessMock.Setup(x => x.ListDuplicateTestCases(testCases)).Returns(new List<DuplicateTestCase>());

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            var actual = target.GetTestCases();

            // Assert
            actual.Should().NotBeNullOrEmpty();
            actual.Length.Should().Be(testCases.Length);
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }
    }
}
