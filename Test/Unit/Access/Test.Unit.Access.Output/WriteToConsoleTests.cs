using System;
using FluentAssertions;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.Output
{
    [TestClass]
    public class WriteToConsoleTests
    {
        [TestMethod]
        public void CommandLineAccess_WriteToConsole_SuccessWhereIsLocalIsFalse()
        {
            // Arrange
            const bool isLocal = false;
            const string messageType = "STAGE";
            const string message = "Trying to parse arguments...";

            var messages = new Messages();
            var azureDevOpsColors = new AzureDevOpsColors();

            var target = new OutputAccessFactory(isLocal, messages, azureDevOpsColors).Create();

            // Act
            Action actual = () => target.WriteToConsole(message, messageType);

            // Assert
            actual.Should().NotThrow();
        }

        [TestMethod]
        public void CommandLineAccess_WriteToConsole_SuccessWhereIsLocalIsTrue()
        {
            // Arrange
            const bool isLocal = true;
            const string messageType = "STAGE";
            const string message = "Trying to parse arguments...";

            var messages = new Messages();
            var azureDevOpsColors = new AzureDevOpsColors();

            var target = new OutputAccessFactory(isLocal, messages, azureDevOpsColors).Create();

            // Act
            Action actual = () => target.WriteToConsole(message, messageType);

            // Assert
            actual.Should().NotThrow();
        }
    }
}
