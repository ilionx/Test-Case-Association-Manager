using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.File
{
    [TestClass]
    public class ListTestAssemblyPathsTests
    {
        // Todo
        // Alternative: illegal directory path | empty matchingFilesToBeIncluded | not empty matchingFilesToBeIncluded & not empty matchingFilesToBeExcluded
        // Main: matchingFilesToBeIncluded is not empty & empty matchingFilesToBeExcluded 
        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_NonExistingDirectoryPath()
        {
            // Arrange
            var directory = string.Empty;
            var minimatchPatterns = new string[0];

            var target = new FileAccessFactory().Create();

            // Act
            Action actual = () => target.ListTestAssemblyPaths(directory, minimatchPatterns);

            // Assert
            actual.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_EmptyMatchingFilesToBeIncluded()
        {
            // Arrange
            // create directory
            string path = Path.GetRandomFileName();
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), path));
            var directory = Directory.GetCurrentDirectory();
            var minimatchPatterns = new string[0];

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListTestAssemblyPaths(directory, minimatchPatterns);

            // Assert
            actual.Length.Should().Be(0);

            // result = empty string[]
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_NotEmptyMatchingFilesToBeIncludedAndNotEmptyMatchingFilesToBeExcluded()
        {
            // Arrange

            // Act

            // Assert

            // result = empty string[]
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_NotEmptyMatchingFilesToBeIncludedAndEmptyMatchingFilesToBeExcluded()
        {
            // Arrange

            // Act

            // Assert

            // result = not empty string[]
        }
    }
}
