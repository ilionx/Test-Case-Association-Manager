namespace AssociateTestsToTestCases.Access.TestCase
{
    public class TestCase
    {
        public readonly int Id;
        public readonly string Title;
        public readonly string AutomationStatus;
        public readonly string AutomatedTestName;

        public TestCase(int id, string title, string automationStatus, string automatedTestName)
        {
            Id = id;
            Title = title;
            AutomationStatus = automationStatus;
            AutomatedTestName = automatedTestName;
        }
    }
}
