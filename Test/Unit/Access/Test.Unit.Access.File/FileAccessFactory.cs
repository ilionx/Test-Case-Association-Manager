using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Access.File.Strategy;

namespace Test.Unit.Access.File
{
    public class FileAccessFactory
    {
        private readonly AssemblyHelper _assemblyHelper;
        private readonly ITestFrameworkStrategy _testFrameWorkStrategy;

        public FileAccessFactory(AssemblyHelper assemblyAccess = null, ITestFrameworkStrategy testFrameWorkStrategy = null)
        {
            _assemblyHelper = assemblyAccess ?? new AssemblyHelper();
            _testFrameWorkStrategy = testFrameWorkStrategy ?? new MsTestStrategy();
        }

        public IFileAccess Create()
        {
            return new FileAccess(_assemblyHelper, _testFrameWorkStrategy);
        }
    }
}
