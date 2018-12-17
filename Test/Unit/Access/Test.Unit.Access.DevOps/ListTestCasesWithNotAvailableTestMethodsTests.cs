using Moq;
using AutoFixture;
using System.Linq;
using FluentAssertions;
using System.Collections.Generic;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Access.TestCase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.DevOps
{
    [TestClass]
    public class ListTestCasesWithNotAvailableTestMethodsTests
    {
        private const string AutomatedName = "Automated";
        private const string NotAutomatedName = "Not Automated";

        [TestMethod]
        public void DevOpsAccess_ListTestCasesWithNotAvailableTestMethods_EmptyListTestCasesWithNotAvailableTestMethodsWhereAutomationStatusIsEqualToAutomatedName()
        {
            // Arrange
            var outputAccess = new Mock<IOutputAccess>();

            const bool verboseLogging = true;

            var fixture = new Fixture();
            var messages = new Messages();

            fixture.Customize<TestCase>(c => c.With(x => x.AutomationStatus, AutomatedName));
            var testCases =  fixture.Create<List<TestCase>>();
            var testMethods = testCases.Select(x => new TestMethod(x.Title, string.Empty, string.Empty)).ToList();

            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging).Create();

            // Act
            var actual = target.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);

            // Assert
            actual.Count.Should().Be(0);
        }


        [TestMethod]
        public void DevOpsAccess_ListTestCasesWithNotAvailableTestMethods_EmptyListTestCasesWithNotAvailableTestMethodsWhereAutomationStatusIsNotEqualToAutomatedName()
        {
            // Arrange
            var outputAccess = new Mock<IOutputAccess>();

            const bool verboseLogging = true;

            var fixture = new Fixture();
            var messages = new Messages();

            fixture.Customize<TestCase>(c => c.With(x => x.AutomationStatus, NotAutomatedName));
            var testCases = fixture.Create<List<TestCase>>();
            var testMethods = fixture.Create<List<TestMethod>>();

            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging).Create();

            // Act
            var actual = target.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);

            // Assert
            actual.Count.Should().Be(0);
        }

        [TestMethod]
        public void DevOpsAccess_ListTestCasesWithNotAvailableTestMethods_ListTestCasesWithNotAvailableTestMethods()
        {
            // Arrange
            var outputAccess = new Mock<IOutputAccess>();

            const bool verboseLogging = true;

            var messages = new Messages();
            var fixture = new Fixture();

            fixture.Customize<TestCase>(c => c.With(x => x.AutomationStatus, AutomatedName));
            var testCases = fixture.Create<List<TestCase>>();
            var testMethods = testCases.Select(x => new TestMethod(string.Empty, string.Empty, string.Empty)).ToList();

            var target = new DevOpsAccessFactory(messages, outputAccess.Object, verboseLogging).Create();

            // Act
            var actual = target.ListTestCasesWithNotAvailableTestMethods(testCases, testMethods);

            // Assert
            actual.Count.Should().Be(3);
        }
    }
}
