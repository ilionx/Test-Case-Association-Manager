namespace AssociateTestsToTestCases.Access.Output
{
    public interface IOutputAccess
    {
        void WriteToConsole(string message, string messageType = "", string reason = "");
    }
}
