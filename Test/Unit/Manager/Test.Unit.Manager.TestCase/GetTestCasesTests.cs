using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.TestCase
{
    [TestClass]
    public class GetTestCasesTests
    {
        private const int DefaultWriteCount = 2;

        [TestMethod]
        public void Program_GetTestCases_TestCasesIsNullOrEmpty()
        {
            // Arrange
            var outputAccess = new Mock<IOutputAccess>();
            var testCaseAccessMock = new Mock<ITestCaseAccess>();

            var fixture = new Fixture();
            var message = fixture.Create<string>();
            var messageType = fixture.Create<string>();
            var messageReason = fixture.Create<string>();
            var testCases = new List<AssociateTestsToTestCases.Access.TestCase.TestCase>();

            testCaseAccessMock.Setup(x => x.GetTestCases()).Returns(testCases);
            outputAccess.Setup(x => x.WriteToConsole(message, messageType, messageReason));

            var target = new TestCaseManagerFactory(testCaseAccessMock.Object, outputAccess.Object).Create();

            // Act
            Action actual = () => target.GetTestCases();

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }

        [TestMethod]
        public void Program_GetTestCases_DuplicateTestCasesCountIsNotEqualToZero()
        {
            // Arrange
            var outputAccess = new Mock<IOutputAccess>();
            var testCaseAccessMock = new Mock<ITestCaseAccess>();

            var fixture = new Fixture();
            var message = fixture.Create<string>();
            var messageType = fixture.Create<string>();
            var messageReason = fixture.Create<string>();
            var duplicateTestCases = fixture.Create<List<DuplicateTestCase>>();
            var testCases = fixture.Create<List<AssociateTestsToTestCases.Access.TestCase.TestCase>>();

            testCaseAccessMock.Setup(x => x.GetTestCases()).Returns(testCases);
            testCaseAccessMock.Setup(x => x.ListDuplicateTestCases(testCases)).Returns(duplicateTestCases);
            outputAccess.Setup(x => x.WriteToConsole(message, messageType, messageReason));

            var target = new TestCaseManagerFactory(testCaseAccessMock.Object, outputAccess.Object).Create();

            // Act
            Action actual = () =>  target.GetTestCases();

            // Assert
            actual.Should().Throw<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount + duplicateTestCases.Count));
        }

        [TestMethod]
        public void Program_GetTestCases_Success()
        {
            // Arrange
            var outputAccess = new Mock<IOutputAccess>();
            var testCaseAccessMock = new Mock<ITestCaseAccess>();

            var fixture = new Fixture();
            var message = fixture.Create<string>();
            var messageType = fixture.Create<string>();
            var messageReason = fixture.Create<string>();
            var testCases = fixture.Create<List<AssociateTestsToTestCases.Access.TestCase.TestCase>>();

            testCaseAccessMock.Setup(x => x.GetTestCases()).Returns(testCases);
            outputAccess.Setup(x => x.WriteToConsole(message, messageType, messageReason));
            testCaseAccessMock.Setup(x => x.ListDuplicateTestCases(testCases)).Returns(new List<DuplicateTestCase>());

            var target = new TestCaseManagerFactory(testCaseAccessMock.Object, outputAccess.Object).Create();

            // Act
            var actual = target.GetTestCases();

            // Assert
            actual.Should().NotBeNullOrEmpty();
            actual.Count.Should().Be(testCases.Count);
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(DefaultWriteCount));
        }
    }
}
