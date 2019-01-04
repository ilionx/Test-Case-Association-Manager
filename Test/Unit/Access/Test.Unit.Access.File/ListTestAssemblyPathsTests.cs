using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.File
{
    [TestClass]
    public class ListTestAssemblyPathsTests
    {
        private static DirectoryInfo _tempDirListTestAssemblyPathsTestsDirectoryInfo;
        private static DirectoryInfo _netFrameworkDirectoryInfo;
        private static DirectoryInfo _debugDirectoryInfo;

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            var tempDirListTestAssemblyPathsTestsFullName = Path.Combine(Path.GetTempPath(), "TempDirListTestAssemblyPathsTests");

            if (Directory.Exists(tempDirListTestAssemblyPathsTestsFullName))
            {
                Directory.Delete(tempDirListTestAssemblyPathsTestsFullName, true);
            }

            _tempDirListTestAssemblyPathsTestsDirectoryInfo = Directory.CreateDirectory(tempDirListTestAssemblyPathsTestsFullName);
            _debugDirectoryInfo = Directory.CreateDirectory(Path.Combine(_tempDirListTestAssemblyPathsTestsDirectoryInfo.FullName, "Debug"));
            _netFrameworkDirectoryInfo = Directory.CreateDirectory(Path.Combine(_debugDirectoryInfo.FullName, "net461"));
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            Directory.Delete(_tempDirListTestAssemblyPathsTestsDirectoryInfo.FullName, true);
        }

        [TestCleanup]
        public void TestCaseCleanUp()
        {
            var directoryFiles = _netFrameworkDirectoryInfo.GetFiles().ToList();

            directoryFiles.ForEach(x => x.Delete());
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_NonExistingDirectoryPath()
        {
            // Arrange
            var directory = string.Empty;
            var minimatchPatterns = new string[0];

            var target = new FileAccessFactory().Create();

            // Act
            Action actual = () => target.ListTestAssemblyPaths(directory, minimatchPatterns.ToList().Select(x => x.ToLowerInvariant()).ToArray());

            // Assert
            actual.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_EmptyMatchingFilesToBeIncluded()
        {
            // Arrange
            var directory = _tempDirListTestAssemblyPathsTestsDirectoryInfo.FullName.ToLowerInvariant();
            var minimatchPatterns = new string[]
            {
                "**\\Debug\\**\\*Test.Integration*.dll",
                "**\\Debug\\**\\*Test.Unit*.dll",
                "!**\\obj\\**"
            };

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListTestAssemblyPaths(directory, minimatchPatterns.ToList().Select(x => x.ToLowerInvariant()).ToArray());

            // Assert
            actual.Length.Should().Be(0);
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_NotEmptyMatchingFilesToBeIncludedAndNotEmptyMatchingFilesToBeExcluded()
        {
            // Arrange
            var tempTestUnitFileName = Path.Combine(_netFrameworkDirectoryInfo.FullName, "Test.Unit.Access.Customer.dll");
            var tempTestIntegrationFileName = Path.Combine(_netFrameworkDirectoryInfo.FullName, "Test.Integration.Access.Customer.dll");
            using (var fs = new FileStream(tempTestUnitFileName, FileMode.Create)) { }
            using (var fs = new FileStream(tempTestIntegrationFileName, FileMode.Create)) { }

            var directory = _tempDirListTestAssemblyPathsTestsDirectoryInfo.FullName.ToLowerInvariant();
            var minimatchPatterns = new string[]
            {
                "**\\Debug\\**\\*Test.Integration*.dll",
                "**\\Debug\\**\\*Test.Unit*.dll",
                "!**\\Debug\\**"
            };

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListTestAssemblyPaths(directory, minimatchPatterns.ToList().Select(x => x.ToLowerInvariant()).ToArray());

            // Assert
            actual.Length.Should().Be(0);
        }

        [TestMethod]
        public void FileAccess_ListTestAssemblyPaths_NotEmptyMatchingFilesToBeIncludedAndEmptyMatchingFilesToBeExcluded()
        {
            // Arrange
            var tempTestUnitFileName = Path.Combine(_netFrameworkDirectoryInfo.FullName, "Test.Unit.Access.Customer.dll");
            var tempTestIntegrationFileName = Path.Combine(_netFrameworkDirectoryInfo.FullName, "Test.Integration.Access.Customer.dll");
            using (var fs = new FileStream(tempTestUnitFileName, FileMode.Create)) { }
            using (var fs = new FileStream(tempTestIntegrationFileName, FileMode.Create)) { }

            var directory = _tempDirListTestAssemblyPathsTestsDirectoryInfo.FullName;
            var minimatchPatterns = new string[]
            {
                "**\\Debug\\**\\*Test.Integration*.dll",
                "**\\Debug\\**\\*Test.Unit*.dll",
                "!**\\obj\\**"
            };

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListTestAssemblyPaths(directory, minimatchPatterns.ToList().Select(x => x.ToLowerInvariant()).ToArray());

            // Assert
            actual.Length.Should().Be(2);
            actual.Should().Contain(tempTestUnitFileName);
            actual.Should().Contain(tempTestIntegrationFileName);
        }
    }
}
