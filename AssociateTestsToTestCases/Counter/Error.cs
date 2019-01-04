namespace AssociateTestsToTestCases.Counter
{
    public class Error
    {
        private readonly Counter _counter;

        public Error(Counter counter)
        {
            _counter = counter;
        }

        private int _operationFailed;
        private int _testCaseNotFound;

        public int Total { get; internal set; }

        public int OperationFailed
        {
            get
            {
                return _operationFailed;
            }
            set
            {
                if (value != 0)
                {
                    var difference = value - _operationFailed;

                    this.Total += difference;
                }
                _operationFailed = value;
            }
        }

        public int TestCaseNotFound
        {
            get
            {
                return _testCaseNotFound;
            }
            set
            {
                if (value != 0)
                {
                    var difference = value - _testCaseNotFound;

                    this.Total += difference;
                }
                _testCaseNotFound = value;
            }
        }
    }
}
