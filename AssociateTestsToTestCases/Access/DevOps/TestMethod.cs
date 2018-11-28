namespace AssociateTestsToTestCases.Access.DevOps
{
    public class TestMethod
    {
        public readonly string Name;
        public readonly string AssemblyName;
        public readonly string FullClassName;

        public TestMethod(string name, string assemblyName, string fullClassName)
        {
            Name = name;
            AssemblyName = assemblyName;
            FullClassName = fullClassName;
        }
    }
}
