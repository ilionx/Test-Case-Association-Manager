namespace AssociateTestsToTestCases.Counter
{
    public class Success
    {
        private readonly Counter _counter;

        public Success(Counter counter)
        {
            _counter = counter;
        }

        private int _total;

        public int FixedReference;

        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                if (value != 0)
                {
                    var difference = value - _total;

                    _counter.Total += difference;
                }
                _total = value;
            }
        }
    }
}
