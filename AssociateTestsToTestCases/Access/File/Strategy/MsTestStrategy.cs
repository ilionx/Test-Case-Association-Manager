using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssociateTestsToTestCases.Access.File.Strategy
{
    public class MsTestStrategy : ITestFrameworkStrategy
    {
        public TestFrameworkType TestFrameworkType { get; set; } = TestFrameworkType.MsTest;

        public IEnumerable<MethodInfo> RetrieveTestMethods(Assembly testAssembly)
        {
            return testAssembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<TestClassAttribute>() != null)
                    .SelectMany(type => type.GetMethods()
                        .Where(method => method.GetCustomAttribute<TestMethodAttribute>() != null));
        }
    }
}
