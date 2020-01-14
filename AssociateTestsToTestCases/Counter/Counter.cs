using System.Linq;

namespace AssociateTestsToTestCases.Counter
{
    public sealed class Counter
    {
        private const string TotalName = "Total";

        public readonly Error Error;
        public readonly Warning Warning;
        public readonly Success Success;
        public readonly Unaffected Unaffected;

        private readonly string[] _excludedCounters;

        public Counter()
        {
            Error = new Error();
            Warning = new Warning();
            Success = new Success();
            Unaffected = new Unaffected();

            _excludedCounters = new string[] { "Error" };
        }

        public int Total
        {
            get
            {
                return GetType().GetFields().Where(x => !_excludedCounters.Contains(x.Name))
                    .Select(y => (int)y.GetValue(this).GetType().GetProperty(TotalName).GetValue(y.GetValue(this)))
                    .Sum();
            }
        }
    }
}
