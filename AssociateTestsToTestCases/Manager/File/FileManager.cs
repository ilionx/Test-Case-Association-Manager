using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
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
        private readonly InputOptions _options;

        public FileManager(Messages messages, IFileAccess fileAccess, IOutputAccess outputAccess, InputOptions options)
        {
            _messages = messages;
            _fileAccess = fileAccess;
            _outputAccess = outputAccess;
            _options = options;
        }

        public bool TestMethodsPathIsEmpty()
        {
            return _fileAccess.ListTestMethods(_options.TestAssemblyPaths).IsNullOrEmpty();
        }

        public TestMethod[] GetTestMethods()
        {
            _outputAccess.WriteToConsole(_messages.Stages.TestMethod.Status, _messages.Types.Stage);

            var testMethods = _fileAccess.ListTestMethods(_options.TestAssemblyPaths);
            ValidateTestMethodsIsNullOrEmpty(testMethods);
            ValidateTestMethodsHasDuplicates(_fileAccess.ListDuplicateTestMethods(testMethods));

            _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestMethod.Success, testMethods.Length), _messages.Types.Success);

            return MapTestMethods(testMethods);
        }

        private TestMethod[] MapTestMethods(MethodInfo[] methods)
        {
            return methods.Select(x => new TestMethod(x.Name, x.Module.Name, x.DeclaringType.FullName, Guid.NewGuid())).ToArray();
        }

        #region Validations

        private void ValidateTestMethodsIsNullOrEmpty(MethodInfo[] testMethods)
        {
            if (testMethods.IsNullOrEmpty())
            {
                _outputAccess.WriteToConsole(_messages.Stages.TestMethod.Failure, _messages.Types.Error);
                throw new InvalidOperationException(_messages.Stages.TestMethod.Failure);
            }
        }

        private void ValidateTestMethodsHasDuplicates(List<DuplicateTestMethod> duplicateTestMethods)
        {
            if (duplicateTestMethods.Count != 0)
            {
                _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count), _messages.Types.Error);
                duplicateTestMethods.ForEach(x => _outputAccess.WriteToConsole(string.Format(_messages.TestMethods.Duplicate, x.Name, _messages.TestMethods.GetDuplicateTestMethodNamesString(x.TestMethods)), _messages.Types.Info));

                throw new InvalidOperationException(string.Format(_messages.Stages.TestMethod.Duplicate, duplicateTestMethods.Count));
            }
        }

        #endregion
    }
}
