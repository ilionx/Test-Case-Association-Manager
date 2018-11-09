using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssociateTestsToTestCases.Access.Association
{
    public class TestMethod
    {
        public readonly string Name;
        public readonly string FullName;

        public TestMethod(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }
    }
}
