using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.TestCase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.Output
{
    [TestClass]
    public class OutputSummaryTests
    {
        [TestMethod]
        public void OutputManager_OutputSummary_Success()
        {
            // Arrange
            var messages = new Messages();
            var writeToConsoleEventLoggerMock = new Mock<IWriteToConsoleEventLogger>();

            var fixture = new Fixture();
            var testCases = fixture.Create<List<TestCase>>();
            var methods = new MethodInfo[]
            {
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name)
            };

            var target = new OutputManagerFactory(messages, writeToConsoleEventLoggerMock.Object).Create();

            // Act
            Action actual = () => target.OutputSummary(methods, testCases);

            // Assert
            actual.Should().NotThrow<InvalidOperationException>();
            writeToConsoleEventLoggerMock.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
