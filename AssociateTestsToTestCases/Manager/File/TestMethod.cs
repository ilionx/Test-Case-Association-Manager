using System;

namespace AssociateTestsToTestCases.Manager.File
{
    public class TestMethod
    {
        public readonly string Name;
        public readonly string AssemblyName;
        public readonly string FullClassName;

        public readonly Guid TempId;

        public TestMethod(string name, string assemblyName, string fullClassName, Guid id)
        {
            Name = name;
            AssemblyName = assemblyName;
            FullClassName = fullClassName;

            TempId = id;
        }
    }
}
