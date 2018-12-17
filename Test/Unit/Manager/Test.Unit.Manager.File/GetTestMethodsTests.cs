using Moq;
using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Message.TestCase;
using AssociateTestsToTestCases.Message.TestMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.File
{
    [TestClass]
    public class GetTestMethodsTests
    {
        private const int DefaultWriteCount = 2;

        [TestMethod]
        public void FileManager_GetTestMethods_TestMethodsIsNullOrEmpty()
        {
            // Arrange
            var fileAccessMock = new Mock<IFileAccess>();
            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var testAssemblyPaths = fixture.Create<string[]>();

            fileAccessMock.Setup(x => x.ListTestMethods(It.IsAny<string[]>())).Returns(new MethodInfo[0]);

            var target = new FileManagerFactory(fileAccessMock.Object, outputAccess.Object).Create();

            // Act
            Action actual = () => target.GetTestMethods(testAssemblyPaths);

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }

        [TestMethod]
        public void FileManager_GetTestMethods_DuplicateTestMethodsCountIsNotEqualToZero()
        {
            // Arrange
            var fileAccessMock = new Mock<IFileAccess>();
            var outputAccess = new Mock<IOutputAccess>();
            var testCaseMessageMock = new Mock<TestCaseMessage>() { CallBase = true };
            var testMethodMessageMock = new Mock<TestMethodMessage>() { CallBase = true };

            var fixture = new Fixture();
            var testMethods = new MethodInfo[20];
            var testAssemblyPaths = fixture.Create<string[]>();
            var duplicateTestMethodName = fixture.Create<string>();
            var duplicateTestMethods = fixture.Create<List<string>>().Select(x => new DuplicateTestMethod(x, new MethodInfo[2])).ToList();

            fileAccessMock.Setup(x => x.ListTestMethods(It.IsAny<string[]>())).Returns(testMethods);
            fileAccessMock.Setup(x => x.ListDuplicateTestMethods(It.IsAny<MethodInfo[]>())).Returns(duplicateTestMethods);
            testMethodMessageMock.Setup(x => x.GetDuplicateTestMethodNamesString(It.IsAny<MethodInfo[]>())).Returns(duplicateTestMethodName);

            var messagesMock = new Mock<Messages>(MockBehavior.Default, testCaseMessageMock.Object, testMethodMessageMock.Object);

            var target = new FileManagerFactory(fileAccessMock.Object, outputAccess.Object, messagesMock.Object).Create();

            // Act
            Action actual = () => target.GetTestMethods(testAssemblyPaths);

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount + duplicateTestMethods.Count));
        }

        [TestMethod]
        public void FileManager_GetTestMethods_Success()
        {
            // Arrange
            var fileAccessMock = new Mock<IFileAccess>();
            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var testAssemblyPaths = fixture.Create<string[]>();
            var testMethods = new MethodInfo[20];

            fileAccessMock.Setup(x => x.ListTestMethods(It.IsAny<string[]>())).Returns(testMethods);
            fileAccessMock.Setup(x => x.ListDuplicateTestMethods(It.IsAny<MethodInfo[]>())).Returns(new List<DuplicateTestMethod>());

            var target = new FileManagerFactory(fileAccessMock.Object, outputAccess.Object).Create();

            // Act
            var actual = target.GetTestMethods(testAssemblyPaths);

            // Assert
            actual.Length.Should().Be(testMethods.Length);
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }
    }
}
