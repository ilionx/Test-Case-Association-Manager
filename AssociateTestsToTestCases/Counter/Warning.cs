namespace AssociateTestsToTestCases.Counter
{
    public class Warning
    {
        private readonly Counter _counter;

        public Warning(Counter counter)
        {
            _counter = counter;
        }

        public int Total { get; internal set; }

        private int _testMethodNotAvailable;

        public int TestMethodNotAvailable
        {
            get
            {
                return _testMethodNotAvailable;
            }
            set
            {
                if (value != 0)
                {
                    var difference = value - _testMethodNotAvailable;

                    this.Total += difference;
                    _counter.Total += difference;
                }

                _testMethodNotAvailable = value;
            }
        }
    }
}
