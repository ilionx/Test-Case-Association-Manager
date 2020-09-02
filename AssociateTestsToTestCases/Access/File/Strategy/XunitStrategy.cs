using System;
using System.Collections.Generic;
using System.Reflection;

namespace AssociateTestsToTestCases.Access.File.Strategy
{
    public class XunitStrategy : ITestFrameworkStrategy
    {
        public TestFrameworkType TestFrameworkType { get; set; } = TestFrameworkType.Xunit;

        public IEnumerable<MethodInfo> RetrieveTestMethods(Assembly testAssembly)
        {
            // todo: retrieve collection, then retrieve facts & theories.
            throw new NotImplementedException();
        }
    }
}
