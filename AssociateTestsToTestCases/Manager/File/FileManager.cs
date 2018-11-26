using System;
using System.Reflection;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;

namespace AssociateTestsToTestCases.Manager.File
{
    public class FileManager
    {
        private readonly Messages _messages;
        private readonly FileAccess _fileAccess;
        private readonly WriteToConsoleEventLogger _writeToConsoleEventLogger;

        public FileManager(Messages messages, FileAccess fileAccess, WriteToConsoleEventLogger writeToConsoleEventLogger)
        {
            _messages = messages;
            _fileAccess = fileAccess;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public MethodInfo[] GetTestMethods(string[] testAssemblyPaths)
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.TestMethod.Status, _messages.Types.Stage);
            var testMethods = _fileAccess.ListTestMethods(testAssemblyPaths);
            if (testMethods.IsNullOrEmpty())
            {
                _writeToConsoleEventLogger.Write(_messages.Stages.TestMethod.Failure, _messages.Types.Error);
                Environment.Exit(-1);
            }

            var duplicateTestMethods = _fileAccess.ListDuplicateTestMethods(testMethods);
            if (duplicateTestMethods.Count != 0)
            {
                _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count), _messages.Types.Error);
                duplicateTestMethods.ForEach(x => _writeToConsoleEventLogger.Write(string.Format(_messages.TestMethods.Duplicate, x.Name, _messages.TestMethods.GetDuplicateTestMethodNamesString(x.TestMethods)), _messages.Types.Info));
                Environment.Exit(-1);
            }

            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestMethod.Success, testMethods.Length), _messages.Types.Success);
            return testMethods;
        }
    }
}
