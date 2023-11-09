using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Access.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.File
{
    [TestClass]
    public class TestMethodsPathIsEmpty
    {
        [TestMethod]
        public void FileManager_TestMethodsPathIsEmpty_ReturnsFalse()
        {
            // Arrange
            var fileAccessMock = new Mock<IFileAccess>();
            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var testAssemblyPaths = fixture.Create<string[]>();

            fileAccessMock.Setup(x => x.ListTestMethods(It.IsAny<string[]>())).Returns(new MethodInfo[2]);

            var target = new FileManagerFactory(fileAccessMock.Object, outputAccess.Object).Create();

            // Act
            var actual = target.TestMethodAssembliesContainNoTestMethods(testAssemblyPaths);

            // Assert
            actual.Should().Be(false);
        }

        [TestMethod]
        public void FileManager_TestMethodsPathIsEmpty_ReturnsTrue()
        {
            // Arrange
            var fileAccessMock = new Mock<IFileAccess>();
            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var testAssemblyPaths = fixture.Create<string[]>();

            fileAccessMock.Setup(x => x.ListTestMethods(It.IsAny<string[]>())).Returns(new MethodInfo[0]);

            var target = new FileManagerFactory(fileAccessMock.Object, outputAccess.Object).Create();

            // Act
            var actual = target.TestMethodAssembliesContainNoTestMethods(testAssemblyPaths);

            // Assert
            actual.Should().Be(true);
        }
    }
}
