using Moq;
using System;
using System.IO;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using AssociateTestsToTestCases.Access.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.File
{
    [TestClass]
    public class ListTestMethodsTests
    {
        [TestMethod]
        public void FileAccess_ListTestMethods_EmptyTestAssemblyPaths()
        {
            // Arrange
            var testAssemblyPaths = new string[0];

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListTestMethods(testAssemblyPaths);

            // Assert
            actual.Length.Should().Be(0);
        }

        [TestMethod]
        public void FileAccess_ListTestMethods_TestAssemblyPathNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testAssemblyPaths = new string[] { fixture.Create<string>() };

            var target = new FileAccessFactory().Create();

            // Act
            Action actual = () => target.ListTestMethods(testAssemblyPaths);

            // Assert
            actual.Should().Throw<FileNotFoundException>();
        }

        [TestMethod]
        public void FileAccess_ListTestMethods_EmptyTestMethods()
        {
            // Arrange
            var assemblyAccessMock = new Mock<AssemblyHelper>();

            var fixture = new Fixture();
            var testAssembly = Assembly.GetCallingAssembly();
            var testAssemblyPaths = new string[] { fixture.Create<string>() };

            assemblyAccessMock.Setup(x => x.LoadFrom(It.IsAny<string>())).Returns(testAssembly);

            var target = new FileAccessFactory(assemblyAccessMock.Object).Create();

            // Act
            var actual = target.ListTestMethods(testAssemblyPaths);

            // Assert
            actual.Length.Should().Be(0);
        }

        [TestMethod]
        public void FileAccess_ListTestMethods_Success()
        {
            // Arrange
            var assemblyAccessMock = new Mock<AssemblyHelper>();

            var fixture = new Fixture();
            var testAssembly = Assembly.GetExecutingAssembly();
            var testAssemblyPaths = new string[] { fixture.Create<string>() };

            assemblyAccessMock.Setup(x => x.LoadFrom(It.IsAny<string>())).Returns(testAssembly);

            var target = new FileAccessFactory(assemblyAccessMock.Object).Create();

            // Act
            var actual = target.ListTestMethods(testAssemblyPaths);

            // Assert
            actual.Length.Should().BeGreaterThan(0);
        }
    }
}
