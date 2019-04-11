using Moq;
using System;
using AutoFixture;
using FluentAssertions;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Manager.DevOps
{
    [TestClass]
    public class TestPlanIsEmpty
    {
        [TestMethod]
        public void DevOpsManager_TestPlanIsEmpty_ReturnsFalse()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            var fixture = new Fixture();
            var testCasesId = fixture.Create<int[]>();

            devOpsAccessMock.Setup(x => x.GetTestCasesId()).Returns(testCasesId);

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object).Create();

            // Act
            Action actual = () => target.TestPlanIsEmpty();

            // Assert
            actual.Should().NotThrow();
            actual.Should().Equals(false);
        }

        [TestMethod]
        public void DevOpsManager_TestPlanIsEmpty_ReturnsTrue()
        {
            // Arrange
            var outputManagerMock = new Mock<IOutputManager>();
            var devOpsAccessMock = new Mock<IDevOpsAccess>();

            devOpsAccessMock.Setup(x => x.GetTestCasesId()).Returns(new int[0]);

            var target = new DevOpsManagerFactory(devOpsAccessMock.Object, outputManagerMock.Object).Create();

            // Act
            Action actual = () => target.TestPlanIsEmpty();

            // Assert
            actual.Should().NotThrow();
            actual.Should().Equals(true);
        }
    }
}
