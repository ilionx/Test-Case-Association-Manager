namespace AssociateTestsToTestCases.Message.Stage
{
    public class HttpClientsMessage
    {
        public readonly string Status = "Initializing HttpClients...";
        public readonly string Success = "HttpClients have been initialized.\n";
        public readonly string FailureResourceNotFound = "The resource '{0}' cannot be found.";
        public readonly string FailureUserNotAuthorized = "The user is not authorized to access this resource '{0}'.";
    }
}
