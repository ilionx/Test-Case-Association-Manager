using Moq;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Utility;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Access.TestCase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class AssociateTests : DevOpsAccessTestsBase
    {
        private const string AutomatedName = "Automated";
        private const string NotAutomatedName = "Not Automated";

        [TestMethod]
        public void DevOpsAccess_Associate_TestCaseIsNull()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testCases = fixture.Create<List<TestCase>>();
            var testMethods = fixture.Create<List<TestMethod>>();

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(testMethods.Count);
            Counter.Error.Should().Be(testMethods.Count);
            Counter.TestCaseNotFound.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
        }

        [TestMethod]
        public void DevOpsAccess_Associate_AlreadyAssociated()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, $"{x.FullClassName}.{x.Name}")).ToList();

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(0);
            Counter.Total.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsFalseAndVerboseLoggingIsFalse()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = false;
            const bool operationSuccess = false;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(testMethods.Count);
            Counter.Total.Should().Be(0);
            Counter.FixedReference.Should().Be(testMethods.Count);
            Counter.OperationFailed.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
        }

        [TestMethod]
        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsFalseAndVerboseLoggingIsTrue()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = true;
            const bool operationSuccess = false;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(testMethods.Count);
            Counter.Total.Should().Be(0);
            Counter.FixedReference.Should().Be(testMethods.Count);
            Counter.OperationFailed.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count * 2));
        }

        [TestMethod]
        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsTrueAndVerboseLoggingIsFalse()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = false;
            const bool operationSuccess = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(0);
            Counter.OperationFailed.Should().Be(0);
            Counter.Total.Should().Be(testMethods.Count);
            Counter.FixedReference.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsTrueAndVerboseLoggingIsTrue()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = true;
            const bool operationSuccess = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(0);
            Counter.OperationFailed.Should().Be(0);
            Counter.Total.Should().Be(testMethods.Count);
            Counter.FixedReference.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count * 2));
        }

        [TestMethod]
        public void DevOpsAccess_Associate_OperationSuccessIsFalse()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = false;
            const bool operationSuccess = false;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, NotAutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(testMethods.Count);
            Counter.Total.Should().Be(0);
            Counter.FixedReference.Should().Be(0);
            Counter.TestCaseNotFound.Should().Be(0);
            Counter.OperationFailed.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
        }

        [TestMethod]
        public void DevOpsAccess_Associate_OperationSuccessIsTrueWhereVerboseLoggingIsFalse()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = false;
            const bool operationSuccess = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, NotAutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(0);
            Counter.FixedReference.Should().Be(0);
            Counter.OperationFailed.Should().Be(0);
            Counter.TestCaseNotFound.Should().Be(0);
            Counter.Total.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void DevOpsAccess_Associate_OperationSuccessIsTrueWhereVerboseLoggingIsTrue()
        {
            // Arrange
            var testCaseAccessMock = new Mock<ITestCaseAccess>();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            const bool validationOnly = true;
            const bool verboseLogging = true;
            const bool operationSuccess = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var testMethods = fixture.Create<List<TestMethod>>();
            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, NotAutomatedName, string.Empty)).ToList();

            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(operationSuccess);

            var target = new DevOpsAccessFactory(writeToConsoleEventLoggerMock.Object, messages, verboseLogging).Create();

            // Act
            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, validationOnly, testType);

            // Assert
            errorCount.Should().Be(0);
            Counter.FixedReference.Should().Be(0);
            Counter.OperationFailed.Should().Be(0);
            Counter.TestCaseNotFound.Should().Be(0);
            Counter.Total.Should().Be(testMethods.Count);
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
        }
    }
}
