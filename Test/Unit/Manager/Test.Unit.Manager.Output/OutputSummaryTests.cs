using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;
using AssociateTestsToTestCases.Counter;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
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
            var outputAccess = new Mock<IOutputAccess>();

            var fixture = new Fixture();
            var testCases = fixture.Create<List<TestCase>>();
            var methods = new MethodInfo[]
            {
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name),
                GetType().GetMethod(MethodBase.GetCurrentMethod().Name)
            };

            var counter = new Counter();

            var target = new OutputManagerFactory(messages, outputAccess.Object, counter).Create();

            // Act
            Action actual = () => target.OutputSummary(methods, testCases);

            // Assert
            actual.Should().NotThrow<InvalidOperationException>();
            outputAccess.Verify(x => x.WriteToConsole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
