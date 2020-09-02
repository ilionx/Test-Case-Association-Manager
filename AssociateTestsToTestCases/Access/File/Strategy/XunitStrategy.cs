using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssociateTestsToTestCases.Access.File.Strategy
{
    public class XunitStrategy : ITestFrameworkStrategy
    {
        public TestFrameworkType TestFrameworkType { get; set; } = TestFrameworkType.Xunit;

        public IEnumerable<MethodInfo> RetrieveTestMethods(Assembly testAssembly)
        {
            return testAssembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<Xunit.TestCollectionOrdererAttribute>() != null)
                    .SelectMany(type => type.GetMethods()
                        .Where(method => method.GetCustomAttribute<Xunit.FactAttribute>() != null || method.GetCustomAttribute<Xunit.TheoryAttribute>() != null));
        }
    }
}
