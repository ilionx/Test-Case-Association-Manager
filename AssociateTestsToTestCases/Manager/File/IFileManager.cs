using System.Reflection;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.File
{
    public interface IFileManager
    {
        bool TestMethodsPathIsEmpty(string[] testAssemblyPaths);
        TestMethod[] GetTestMethods(string[] testAssemblyPaths);
    }
}
