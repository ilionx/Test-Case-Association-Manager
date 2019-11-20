using System.Reflection;
using AssociateTestsToTestCases.Access.DevOps;

namespace AssociateTestsToTestCases.Manager.File
{
    public interface IFileManager
    {
        bool TestMethodAssembliesContainNoTestMethods(string[] testAssemblyPaths);
        TestMethod[] GetTestMethods(string[] testAssemblyPaths);
        string[] GetTestAssemblyPaths(string directory, string[] minimatchPatterns);
    }
}
