using System.Reflection;

namespace AssociateTestsToTestCases.Access.File
{
    public class AssemblyHelper
    {
        public virtual Assembly LoadFrom(string testAssemblyPath)
        {
            return Assembly.LoadFrom(testAssemblyPath);
        }
    }
}
