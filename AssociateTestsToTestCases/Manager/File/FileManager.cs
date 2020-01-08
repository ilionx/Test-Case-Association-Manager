using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Extensions;
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

        public bool TestMethodAssembliesContainNoTestMethods(string[] testAssemblyPaths)
        {
            return _fileAccess.ListTestMethods(testAssemblyPaths).IsNullOrEmpty();
        }

        public TestMethod[] GetTestMethods(string[] testAssemblyPaths)
        {
            _outputAccess.WriteToConsole(_messages.Stages.TestMethod.Status, _messages.Types.Stage);

            var rawTestMethods = _fileAccess.ListTestMethods(testAssemblyPaths);
            ValidateTestMethodsIsNullOrEmpty(rawTestMethods);

            var duplicateTestMethods = _fileAccess.ListDuplicateTestMethods(rawTestMethods);
            ValidateTestMethodsHasDuplicates(duplicateTestMethods);

            _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestMethod.Success, rawTestMethods.Length), _messages.Types.Success);

            return rawTestMethods.ToTestMethodArray();
        }

        public string[] GetTestAssemblyPaths(string directory, string[] minimatchPatterns)
        {
            _outputAccess.WriteToConsole(_messages.Stages.TestAssemblyPath.Status, _messages.Types.Stage);

            var testAssemblyPaths = _fileAccess.ListTestAssemblyPaths(directory, minimatchPatterns);
            ValidateTestAssemblyPathsNotFound(testAssemblyPaths);

            return testAssemblyPaths;
        }

        #region Validations

        private void ValidateTestMethodsIsNullOrEmpty(MethodInfo[] testMethods)
        {
            if (!testMethods.IsNullOrEmpty())
            {
                return;
            }

            _outputAccess.WriteToConsole(_messages.Stages.TestMethod.Failure, _messages.Types.Error);

            throw new InvalidOperationException(_messages.Stages.TestMethod.Failure);
        }

        private void ValidateTestMethodsHasDuplicates(List<DuplicateTestMethod> duplicateTestMethods)
        {
            if (duplicateTestMethods.Count == 0)
            {
                return;
            }

            _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count), _messages.Types.Error);

            foreach (var duplicateTestMethod in duplicateTestMethods)
            {
                var testMethodName = duplicateTestMethod.Name;
                var duplicateTestMethodNames = _messages.TestMethods.GetDuplicateTestMethodNamesString(duplicateTestMethod.TestMethods);
                var message = string.Format(_messages.TestMethods.Duplicate, testMethodName, duplicateTestMethodNames);

                _outputAccess.WriteToConsole(message, _messages.Types.Info);
            }

            throw new InvalidOperationException(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count));
        }

        private void ValidateTestAssemblyPathsNotFound(string[] testAssemblyPaths)
        {
            if (testAssemblyPaths != null)
            {
                _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestAssemblyPath.Success, testAssemblyPaths.Length), _messages.Types.Success);
                return;
            }

            _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestAssemblyPath.Failure), _messages.Types.Error);

            throw new InvalidOperationException(_messages.Stages.TestAssemblyPath.Failure);
        }

        #endregion Validations
    }
}
