using System.Collections.Generic;
using System.Reflection;

namespace AssociateTestsToTestCases.Access.File.Strategy
{
    public interface ITestFrameworkStrategy
    {
        public TestFrameworkType TestFrameworkType { get; set; }

        public IEnumerable<MethodInfo> RetrieveTestMethods(Assembly testAssembly);
    }
}
