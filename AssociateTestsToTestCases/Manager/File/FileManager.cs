using System;
using System.Reflection;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.File;
using AssociateTestsToTestCases.Access.Output;

namespace AssociateTestsToTestCases.Manager.File
{
    public class FileManager : IFileManager
    {
        private readonly Messages _messages;
        private readonly IFileAccess _fileAccess;
        private readonly IOutputAccess _outputAccess;

        public FileManager(Messages messages, IFileAccess fileAccess, IOutputAccess outputAccess)
        {
            _messages = messages;
            _fileAccess = fileAccess;
            _outputAccess = outputAccess;
        }

        public MethodInfo[] GetTestMethods(string[] testAssemblyPaths)
        {
           _outputAccess.WriteToConsole(_messages.Stages.TestMethod.Status, _messages.Types.Stage);
            var testMethods = _fileAccess.ListTestMethods(testAssemblyPaths);
            if (testMethods.IsNullOrEmpty())
            {
               _outputAccess.WriteToConsole(_messages.Stages.TestMethod.Failure, _messages.Types.Error);
                throw new InvalidOperationException(_messages.Stages.TestMethod.Failure);
            }

            var duplicateTestMethods = _fileAccess.ListDuplicateTestMethods(testMethods);
            if (duplicateTestMethods.Count != 0)
            {
               _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count), _messages.Types.Error);
                duplicateTestMethods.ForEach(x =>_outputAccess.WriteToConsole(string.Format(_messages.TestMethods.Duplicate, x.Name, _messages.TestMethods.GetDuplicateTestMethodNamesString(x.TestMethods)), _messages.Types.Info));
                throw new InvalidOperationException(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count));
            }

           _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestMethod.Success, testMethods.Length), _messages.Types.Success);
            return testMethods;
        }
    }
}
