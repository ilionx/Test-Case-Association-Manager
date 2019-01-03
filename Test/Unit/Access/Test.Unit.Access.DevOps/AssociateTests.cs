//using Moq;
//using System.Linq;
//using AutoFixture;
//using FluentAssertions;
//using System.Collections.Generic;
//using AssociateTestsToTestCases.Counter;
//using AssociateTestsToTestCases.Message;
//using AssociateTestsToTestCases.Access.Output;
//using AssociateTestsToTestCases.Access.DevOps;
//using AssociateTestsToTestCases.Access.TestCase;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Test.Unit.Access.DevOps
//{
//    [TestClass]
//    public class AssociateTests : DevOpsAccessTestsBase
//    {
//        private const string AutomatedName = "Automated";
//        private const string NotAutomatedName = "Not Automated";

//        [TestMethod]
//        public void DevOpsAccess_Associate_TestCaseIsNull()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = true;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testCases = fixture.Create<List<TestCase>>();
//            var testMethods = fixture.Create<List<TestMethod>>();

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(testMethods.Count);
//            counter.Error.Total.Should().Be(testMethods.Count);
//            counter.Error.TestCaseNotFound.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_AlreadyAssociated()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = true;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, $"{x.FullClassName}.{x.Name}")).ToList();

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(0);
//            counter.Total.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsFalseAndVerboseLoggingIsFalse()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = false;
//            const bool operationSuccess = false;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(testMethods.Count);
//            counter.Total.Should().Be(0);
//            counter.Success.FixedReference.Should().Be(testMethods.Count);
//            counter.Error.OperationFailed.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsFalseAndVerboseLoggingIsTrue()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = true;
//            const bool operationSuccess = false;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(testMethods.Count);
//            counter.Total.Should().Be(0);
//            counter.Success.FixedReference.Should().Be(testMethods.Count);
//            counter.Error.OperationFailed.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count * 2));
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsTrueAndVerboseLoggingIsFalse()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = false;
//            const bool operationSuccess = true;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(0);
//            counter.Error.OperationFailed.Should().Be(0);
//            counter.Total.Should().Be(testMethods.Count);
//            counter.Success.FixedReference.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_WrongAssociationWhereOperationSuccessIsTrueAndVerboseLoggingIsTrue()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = true;
//            const bool operationSuccess = true;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, AutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(0);
//            counter.Error.OperationFailed.Should().Be(0);
//            counter.Total.Should().Be(testMethods.Count);
//            counter.Success.FixedReference.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count * 2));
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_OperationSuccessIsFalse()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = false;
//            const bool operationSuccess = false;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, NotAutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(testMethods.Count);
//            counter.Total.Should().Be(0);
//            counter.Success.FixedReference.Should().Be(0);
//            counter.Error.TestCaseNotFound.Should().Be(0);
//            counter.Error.OperationFailed.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_OperationSuccessIsTrueWhereVerboseLoggingIsFalse()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = false;
//            const bool operationSuccess = true;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, NotAutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(0);
//            counter.Success.FixedReference.Should().Be(0);
//            counter.Error.OperationFailed.Should().Be(0);
//            counter.Error.TestCaseNotFound.Should().Be(0);
//            counter.Total.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
//        }

//        [TestMethod]
//        public void DevOpsAccess_Associate_OperationSuccessIsTrueWhereVerboseLoggingIsTrue()
//        {
//            // Arrange
//            var outputAccess = new Mock<IOutputAccess>();
//            var testCaseAccessMock = new Mock<ITestCaseAccess>();

//            const bool validationOnly = true;
//            const bool verboseLogging = true;
//            const bool operationSuccess = true;

//            var fixture = new Fixture();
//            var testType = string.Empty;
//            var messages = new Messages();
//            var testMethods = fixture.Create<List<TestMethod>>();
//            var testCases = testMethods.Select(x => new TestCase(fixture.Create<int>(), x.Name, NotAutomatedName, string.Empty)).ToList();

//            testCaseAccessMock.Setup(x => x.AssociateTestCaseWithTestMethod(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(operationSuccess);

//            var counter = new Counter();

//            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging, counter).Create();

//            // Act
//            var errorCount = target.Associate(testMethods, testCases, testCaseAccessMock.Object, testType);

//            // Assert
//            errorCount.Should().Be(0);
//            counter.Success.FixedReference.Should().Be(0);
//            counter.Error.OperationFailed.Should().Be(0);
//            counter.Error.TestCaseNotFound.Should().Be(0);
//            counter.Total.Should().Be(testMethods.Count);
//            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(testMethods.Count));
//        }
//    }
//}
