using System.Reflection;

namespace AssociateTestsToTestCases.Manager.File
{
    public interface IFileManager
    {
        MethodInfo[] GetTestMethods(string[] testAssemblyPaths);
    }
}
