using System.Reflection;
using System.Collections.Generic;

namespace AssociateTestsToTestCases.Access.File
{
    public interface IFileAccess
    {
        MethodInfo[] ListTestMethods(string[] testAssemblyPaths);
        string[] ListTestAssemblyPaths(string directory, string[] minimatchPatterns);
        List<DuplicateTestMethod> ListDuplicateTestMethods(MethodInfo[] testMethods);
    }
}
