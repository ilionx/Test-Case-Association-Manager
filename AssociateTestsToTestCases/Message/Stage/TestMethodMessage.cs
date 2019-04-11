namespace AssociateTestsToTestCases.Message.Stage
{
    public class TestMethodMessage
    {
        public readonly string Status = "Retrieving Test Methods...";
        public readonly string Success = "DLL Test Methods have been retrieved ({0}).\n";
        public readonly string Failure = "Could not retrieve the Test Methods (No Test Methods found). Program has been terminated.\n";
        public readonly string Duplicate = "Duplicate Test Methods have been found ({0}). Program has been terminated.";
    }
}
