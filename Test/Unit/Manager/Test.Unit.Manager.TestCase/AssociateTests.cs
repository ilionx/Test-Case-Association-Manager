using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.DevOps
{
    [TestClass]
    public class AssociateTests
    {
        private const int DefaultWriteCount = 2;

        [TestMethod]
        public void DevOpsManager_Associate_TestMethodsNotAvailableCountIsNotEqualToZero()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var methods = fixture.Create<TestMethod[]>();
            var testCases = fixture.Create<TestCase[]>();
            var testMethodsNotAvailable = fixture.Create<List<TestCase>>();

            devOpsAccessMock.Setup(x => x.ListTestCasesWithNotAvailableTestMethods(It.IsAny<TestMethod[]>(), It.IsAny<TestCase[]>())).Returns(testMethodsNotAvailable);
            devOpsAccessMock.Setup(x => x.Associate(It.IsAny<TestMethod[]>(), It.IsAny<Dictionary<string,TestCase>>())).Returns(0);

            var counter = new Counter();

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            Action actual = () => target.Associate(methods, testCases);

            // Assert
            actual.Should().NotThrow();
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount + testMethodsNotAvailable.Count));
        }

        [TestMethod]
        public void DevOpsManager_Associate_TestMethodsAssociationErrorCountIsNotEqualToZero()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var methods = fixture.Create<TestMethod[]>();
            var testCases = fixture.Create<TestCase[]>();

            devOpsAccessMock.Setup(x => x.ListTestCasesWithNotAvailableTestMethods(It.IsAny<TestMethod[]>(), It.IsAny<TestCase[]>())).Returns(new List<TestCase>());
            devOpsAccessMock.Setup(x => x.Associate(It.IsAny<TestMethod[]>(), It.IsAny<Dictionary<string, TestCase>>())).Returns(3);

            var counter = new Counter();

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            Action actual = () => target.Associate(methods, testCases);

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void DevOpsManager_Associate_Success()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var methods = fixture.Create<TestMethod[]>();
            var testCases = fixture.Create<TestCase[]>();

            devOpsAccessMock.Setup(x => x.ListTestCasesWithNotAvailableTestMethods(It.IsAny<TestMethod[]>(), It.IsAny<TestCase[]>())).Returns(new List<TestCase>());
            devOpsAccessMock.Setup(x => x.Associate(It.IsAny<TestMethod[]>(), It.IsAny<Dictionary<string, TestCase>>())).Returns(0);

            var counter = new Counter();

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object, counter).Create();

            // Act
            Action actual = () => target.Associate(methods, testCases);

            // Assert
            actual.Should().NotThrow();
            outputManagerMock.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
