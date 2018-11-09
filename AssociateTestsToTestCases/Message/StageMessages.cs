namespace AssociateTestsToTestCases.Message
{
    public class StageMessages
    {
        public readonly ArgumentsMessage Arguments = new ArgumentsMessage();
        public readonly DllTestMethodsMessage DllTestMethods = new DllTestMethodsMessage();
        public readonly TestCasesMessage TestCases = new TestCasesMessage();
        public readonly AssociationMessage Association = new AssociationMessage();
        public readonly SummaryMessage Summary = new SummaryMessage();
    }

    public class ArgumentsMessage
    {
        public readonly string Status = "Trying to parse arguments...";
        public readonly string Success = "Arguments have been parsed.\n";
    }

    public class DllTestMethodsMessage
    {
        public readonly string Status = "Trying to retrieve the DLL Test Methods...";
        public readonly string Success = "DLL Test Methods have been obtained ({0}).\n";
        public readonly string Failure = "Could not retrieve the DLL Test Methods. Program has been terminated.\n";
    }

    public class TestCasesMessage
    {
        public readonly string Status = "Trying to retrieve the Azure DevOps Test Cases...";
        public readonly string Success = "Azure DevOps Test Cases have been obtained ({0}).\n";
        public readonly string Failure = "Could not retrieve the Azure DevOps Test Cases. Program has been terminated.\n";
    }

    public class AssociationMessage
    {
        public readonly string Status = "Trying to Associate Test Cases with Test Methods...";
        public readonly string Success = "Test Cases and Test Methods have been associated ({0}).\n";
    }

    public class SummaryMessage
    {
        public readonly string Status = "Summary:";
        public readonly string Overview = "Total Azure DevOps Test Cases: {0} | Total Test Methods: {1} | Total Associations: {2}";
        public readonly string Detailed = "Success: {0} | Errors: {1} | Warnings: {2} [Missing Id: {3} - Test Method N\\A anymore: {4} - Test Method not found: {5}]";
    }
}
