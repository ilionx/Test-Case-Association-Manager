using System.Reflection;

namespace AssociateTestsToTestCases.Access.File
{
    public class DuplicateTestMethod
    {
        public readonly string Name;
        public readonly MethodInfo[] TestMethods;

        public DuplicateTestMethod(string name, MethodInfo[] testMethods)
        {
            Name = name;
            TestMethods = testMethods;
        }
    }
}
