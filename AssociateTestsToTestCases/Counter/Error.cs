using System.Linq;

namespace AssociateTestsToTestCases.Counter
{
    public class Error
    {
        public int OperationFailed { get; set; }

        public int TestCaseNotFound { get; set; }

        public int Total
        {
            get
            {
                return GetType().GetProperties().Where(x => x.Name != "Total").Select(x => (int) x.GetValue(this)).Sum();
            }
        }
    }
}
