namespace AssociateTestsToTestCases.Message.Stage
{
    public class TestMethodMessage
    {
        public readonly string Status = "Trying to retrieve the DLL Test Methods...";
        public readonly string Success = "DLL Test Methods have been obtained ({0}).\n";
        public readonly string Failure = "Could not retrieve the DLL Test Methods. Program has been terminated.\n";
    }
}
