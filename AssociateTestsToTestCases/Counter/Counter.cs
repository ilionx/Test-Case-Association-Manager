namespace AssociateTestsToTestCases.Counter
{
    public sealed class Counter
    {
        public readonly Error Error;
        public readonly Warning Warning;
        public readonly Success Success;
        public readonly Unaffected Unaffected;

        public Counter()
        {
            Error = new Error(this);
            Warning = new Warning(this);
            Success = new Success(this);
            Unaffected = new Unaffected(this);
        }

        public int Total { get; internal set; }
    }
}
