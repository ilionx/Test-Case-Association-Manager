namespace AssociateTestsToTestCases.Message.Stage
{
    public class ArgumentMessage
    {
        public readonly string Status = "Parsing arguments...";
        public readonly string Success = "Arguments have been parsed.\n";
        public readonly string Failure = "Could not parse all arguments.\n";
    }
}
