using System;
using System.Linq;
using System.Reflection;
using AssociateTestsToTestCases.Manager.File;

namespace AssociateTestsToTestCases.Extensions
{
    public static class MethodInfoExtensions
    {
        public static TestMethod[] ToTestMethodArray(this MethodInfo[] methods)
        {
            return methods.Select(x => new TestMethod(x.Name, x.Module.Name, x.DeclaringType.FullName, Guid.NewGuid())).ToArray();
        }
    }
}
