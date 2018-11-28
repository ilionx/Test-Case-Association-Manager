using System.Linq;

namespace AssociateTestsToTestCases.Message.TestCase
{
    public class TestCaseMessage
    {
        private const string Separator = ", ";
        private const string IdsName = "Ids: ";

        public readonly string Duplicate = "Test Case '{0}' ({1})";

        public virtual string GetDuplicateTestCaseNamesString(Access.TestCase.TestCase[] testCases)
        {
            return IdsName + string.Join(Separator, testCases.Select(x => x.Id));
        }
    }
}
