using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Manager.Output;
using AssociateTestsToTestCases.Access.TestCase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.DevOps
{
    [TestClass]
    public class AssociateTests
    {
        private const int DefaultWriteCount = 2;

        [TestMethod]
        public void Program_Associate_TestMethodsNotAvailableCountIsNotEqualToZero()
        {
            // Arrange
            var devOpsAccessMock = new Mock<IDevOpsAccess>();
            var outputAccessMock = new Mock<IOutputAccess>();
            var outputManagerMock = new Mock<IOutputManager>();
            var testCaseAccessMock = new Mock<ITestCaseAccess>();

            const bool validateOnly = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var messages = new Messages();
            var methods = new MethodInfo[]
            {
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name)
            };

            var testCases = fixture.Create<List<TestCase>>();
            var testMethodsNotAvailable = fixture.Create<List<TestCase>>();

            devOpsAccessMock.Setup(x => x.ListTestCasesWithNotAvailableTestMethods(It.IsAny<List<TestCase>>(), It.IsAny<List<TestMethod>>())).Returns(testMethodsNotAvailable);
            devOpsAccessMock.Setup(x => x.Associate(It.IsAny<List<TestMethod>>(), It.IsAny<List<TestCase>>(), It.IsAny<ITestCaseAccess>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(0);

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, outputAccessMock.Object, testCaseAccessMock.Object, messages).Create();

            // Act
            Action actual = () => target.Associate(methods, testCases, validateOnly, testType);

            // Assert
            actual.Should().NotThrow<InvalidOperationException>();
            outputAccessMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount + testMethodsNotAvailable.Count));
        }

        [TestMethod]
        public void Program_Associate_TestMethodsAssociationErrorCountIsNotEqualToZero()
        {
            // Arrange
            var messages = new Messages();
            var outputAccess = new Mock<IOutputAccess>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();
            var outputManagerMock = new Mock<IOutputManager>();
            var testCaseAccessMock = new Mock<ITestCaseAccess>();

            const bool validateOnly = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var methods = new MethodInfo[]
            {
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name)
            };

            var testCases = fixture.Create<List<TestCase>>();

            devOpsAccessMock.Setup(x => x.ListTestCasesWithNotAvailableTestMethods(It.IsAny<List<TestCase>>(), It.IsAny<List<TestMethod>>())).Returns(new List<TestCase>());
            devOpsAccessMock.Setup(x => x.Associate(It.IsAny<List<TestMethod>>(), It.IsAny<List<TestCase>>(), It.IsAny<ITestCaseAccess>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(3);

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, outputAccess.Object, testCaseAccessMock.Object, messages).Create();

            // Act
            Action actual = () => target.Associate(methods, testCases, validateOnly, testType);

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Program_Associate_Success()
        {
            // Arrange
            var messages = new Messages();
            var outputAccess = new Mock<IOutputAccess>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();
            var outputManagerMock = new Mock<IOutputManager>();
            var testCaseAccessMock = new Mock<ITestCaseAccess>();

            const bool validateOnly = true;

            var fixture = new Fixture();
            var testType = string.Empty;
            var methods = new MethodInfo[]
            {
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name)
            };

            var testCases = fixture.Create<List<TestCase>>();

            devOpsAccessMock.Setup(x => x.ListTestCasesWithNotAvailableTestMethods(It.IsAny<List<TestCase>>(), It.IsAny<List<TestMethod>>())).Returns(new List<TestCase>());
            devOpsAccessMock.Setup(x => x.Associate(It.IsAny<List<TestMethod>>(), It.IsAny<List<TestCase>>(), It.IsAny<ITestCaseAccess>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(0);

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, outputAccess.Object, testCaseAccessMock.Object, messages).Create();

            // Act
            Action actual = () => target.Associate(methods, testCases, validateOnly, testType);

            // Assert
            actual.Should().NotThrow<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
