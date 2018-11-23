using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.Access.Output
{
    [TestClass]
    public class CommandLineAccessTests
    {
        // Todo
        // Alternative: !_isLocal
        // Success: _isLocal: Console.WriteLine -> check output
        [TestMethod]
        public void WriteToConsole()
        {

        }

        // Todo
        // Alternative: messageType.Equals(types.Success) - messageType.Equals(types.Warning) - messageType.Equals(types.Error) - messageType.Equals(types.Failure) - messageType.Equals(types.Stage) - messageType.Equals(string.Empty)
        // Main: ConsoleColor.DarkCyan
        [TestMethod]
        public void GetConsoleColor()
        {

        }

        // Todo
        // Alternative: messageType.Equals(types.Success) - messageType.Equals(types.Warning) - messageType.Equals(types.Error) - messageType.Equals(types.Failure) - messageType.Equals(types.Stage) - messageType.Equals(string.Empty)
        // Alternative: colorOutput.Length <= _azureDevOpsColors.LongestTypeCount
        // Main: colorOutput.Length > _azureDevOpsColors.LongestTypeCount & _azureDevOpsColors.DefaultColor
        public void GetAzureDevOpsConsoleColor()
        {

        }
    }
}
