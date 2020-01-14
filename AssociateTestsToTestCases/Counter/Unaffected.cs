using System.Linq;

namespace AssociateTestsToTestCases.Counter
{
    public class Unaffected
    {
        public int AlreadyAutomated { get; set; }

        public int Total
        {
            get
            {
                return GetType().GetProperties().Where(x => x.Name != "Total").Select(x => (int)x.GetValue(this)).Sum();
            }
        }
    }
}
