using AssociateTestsToTestCases.Access.File;

namespace Test.Unit.Access.File
{
    public class FileAccessFactory
    {
        private readonly AssemblyHelper _assemblyAccess;

        public FileAccessFactory(AssemblyHelper assemblyAccess = null)
        {
            _assemblyAccess = assemblyAccess ?? new AssemblyHelper();
        }

        public IFileAccess Create()
        {
            return new FileAccess(_assemblyAccess);
        }

    }
}
