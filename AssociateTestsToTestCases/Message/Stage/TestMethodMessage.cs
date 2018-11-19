namespace AssociateTestsToTestCases.Message.Stage
{
    public class TestMethodMessage
    {
        public readonly string Status = "Trying to retrieve the Test Methods...";
        public readonly string Success = "DLL Test Methods have been obtained ({0}).\n";
        public readonly string Failure = "Could not retrieve the Test Methods. Program has been terminated.\n";
        public readonly string Duplicate = "Duplicate Test Methods have been found ({0}). Program has been terminated.";
    }
}
