using System;
using System.Linq;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.File
{
    [TestClass]
    public class ListDuplicateTestMethodsTests
    {
        [TestMethod]
        public void FileAccess_ListDuplicateTestMethods_EmptyTestMethods()
        {
            // Arrange
            var testMethods = new MethodInfo[1];

            var target = new FileAccessFactory().Create();

            // Act
            Action actual = () => target.ListDuplicateTestMethods(testMethods);

            // Assert
            actual.Should().Throw<NullReferenceException>();
        }

        [TestMethod]
        public void FileAccess_ListDuplicateTestMethods_EmptyDuplicateTestMethods()
        {
            // Arrange
            var testMethods = GetType().GetMethods().Where(x => x.Name.Contains("ListDuplicateTestMethods")).ToArray();

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListDuplicateTestMethods(testMethods);

            //// Assert
            actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void FileAccess_ListDuplicateTestMethods_NotEmptyDuplicateTestMethods()
        {
            // Arrange
            var testMethods = new List<MethodInfo>();
            var testMethodsToBeDuplicated = GetType().GetMethods().Where(x => x.Name.Contains("ListDuplicateTestMethods")).ToArray();
            testMethods.AddRange(testMethodsToBeDuplicated);
            testMethods.AddRange(testMethodsToBeDuplicated);

            var target = new FileAccessFactory().Create();

            // Act
            var actual = target.ListDuplicateTestMethods(testMethods.ToArray());

            //// Assert
            actual.Count.Should().Be(testMethods.Count / 2);
        }
    }
}
