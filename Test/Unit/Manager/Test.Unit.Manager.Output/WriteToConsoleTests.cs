using Moq;
using System;
using FluentAssertions;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.Output
{
    [TestClass]
    public class WriteToConsoleTests
    {
        [TestMethod]
        public void OutputManager_WriteToConsoleTests_Success()
        {
            // Arrange
            var messages = new Messages();
            var outputAccess = new Mock<IOutputAccess>();

            var target = new OutputManagerFactory(messages, outputAccess.Object).Create();

            // Act
            Action actual = () => target.WriteToConsole(string.Empty);

            // Assert
            actual.Should().NotThrow<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
