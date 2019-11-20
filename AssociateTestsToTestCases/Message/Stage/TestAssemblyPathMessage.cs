namespace AssociateTestsToTestCases.Message.Stage
{
    public class TestAssemblyPathMessage
    {
        public readonly string Status = "Retrieving Test Assembly Paths...";
        public readonly string Success = "Test Assembly Paths have been retrieved ({0}).\n";
        public readonly string Failure = "Found 0 Test Assemblies Paths (Incorrect minimatch patterns). Program has been terminated.\n";
    }
}
