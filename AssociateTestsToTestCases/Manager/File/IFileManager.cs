using System.Reflection;

namespace AssociateTestsToTestCases.Manager.File
{
    public interface IFileManager
    {
        bool TestMethodsPathIsEmpty();
        MethodInfo[] GetTestMethods();
    }
}
