namespace AssociateTestsToTestCases.Access.AzureDevOps
{
    public class TestMethod
    {
        public readonly string Name;
        public readonly string FullName;

        public TestMethod(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }
    }
}
