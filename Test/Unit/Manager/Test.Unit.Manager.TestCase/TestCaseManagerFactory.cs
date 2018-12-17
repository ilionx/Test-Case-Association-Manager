using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;
using AssociateTestsToTestCases.Manager.TestCase;

namespace Test.Unit.Manager.TestCase
{
    public class TestCaseManagerFactory
    {
        private readonly ITestCaseAccess _testCaseAccess;
        private readonly IOutputAccess _outputAccess;

        public TestCaseManagerFactory(ITestCaseAccess testCaseAccess, IOutputAccess outputAccess)
        {
            _testCaseAccess = testCaseAccess;
            _outputAccess = outputAccess;
        }

        public ITestCaseManager Create()
        {
            return new TestCaseManager(new Messages(), _outputAccess, _testCaseAccess);
        }
    }
}
