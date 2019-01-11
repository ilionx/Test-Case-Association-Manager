namespace AssociateTestsToTestCases.Access.DevOps
{
    public class DuplicateTestCase
    {
        public readonly string Name;
        public readonly TestCase[] TestCases;

        public DuplicateTestCase(string name, TestCase[] testCases)
        {
            Name = name;
            TestCases = testCases;
        }
    }
}
