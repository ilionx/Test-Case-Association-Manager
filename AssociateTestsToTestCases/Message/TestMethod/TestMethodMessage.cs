using System.Linq;
using System.Reflection;

namespace AssociateTestsToTestCases.Message.TestMethod
{
    public class TestMethodMessage
    {
        private const string Separator = " & ";

        public readonly string Duplicate = "Test Method '{0}' ({1})";

        public virtual string GetDuplicateTestMethodNamesString(MethodInfo[] testMethods)
        {
            return string.Join(Separator, testMethods.Select(x => x.DeclaringType.FullName));
        }
    }
}
