using System.Linq;

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
            Error = new Error();
            Warning = new Warning();
            Success = new Success();
            Unaffected = new Unaffected();
        }

        public int Total
        {
            get
            {
                var excludedCounters = new string[] { "Error" };

                return GetType().GetFields().Where(x => !excludedCounters.Contains(x.Name)).Select(y => (int)y.GetValue(this).GetType().GetProperty("Total").GetValue(y.GetValue(this))).Sum();
            }
        }
    }
}
