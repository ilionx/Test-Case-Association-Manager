namespace AssociateTestsToTestCases.Counter
{
    public class Unaffected
    {
        private readonly Counter _counter;

        public Unaffected(Counter counter)
        {
            _counter = counter;
        }

        private int _alreadyAutomated;

        public int Total { get; internal set; }

        public int AlreadyAutomated
        {
            get
            {
                return _alreadyAutomated;
            }
            set
            {
                if (value != 0)
                {
                    var difference = value - _alreadyAutomated;

                    this.Total += difference;
                    _counter.Total += difference;
                }
                _alreadyAutomated = value;
            }
        }
    }
}
