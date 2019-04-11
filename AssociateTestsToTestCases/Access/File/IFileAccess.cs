using System.Reflection;
using System.Collections.Generic;

namespace AssociateTestsToTestCases.Access.File
{
    public interface IFileAccess
    {
        MethodInfo[] ListTestMethods(string[] testAssemblyPaths);
        List<DuplicateTestMethod> ListDuplicateTestMethods(MethodInfo[] testMethods);
        string[] ListTestAssemblyPaths(string directory, string[] minimatchPatterns);
    }
}
