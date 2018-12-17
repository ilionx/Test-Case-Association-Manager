using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.Output;
using AssociateTestsToTestCases.Access.TestCase;

namespace AssociateTestsToTestCases.Manager.TestCase
{
    public class TestCaseManager : ITestCaseManager
    {
        private readonly Messages _messages;
        private readonly IOutputAccess _outputAccess;
        private readonly ITestCaseAccess _testCaseAccess;

        public TestCaseManager(Messages messages, IOutputAccess outputAccess, ITestCaseAccess testCaseAccess)
        {
            _messages = messages;
            _outputAccess = outputAccess;
            _testCaseAccess = testCaseAccess;
        }

        public List<Access.TestCase.TestCase> GetTestCases()
        {
           _outputAccess.WriteToConsole(_messages.Stages.TestCase.Status, _messages.Types.Stage);
            var testCases = _testCaseAccess.GetTestCases();

            if (testCases.IsNullOrEmpty())
            {
               _outputAccess.WriteToConsole(_messages.Stages.TestCase.Failure, _messages.Types.Error);
                throw new InvalidOperationException(_messages.Stages.TestCase.Failure);
            }

            var duplicateTestCases = _testCaseAccess.ListDuplicateTestCases(testCases);
            if (duplicateTestCases.Count != 0)
            {
               _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count), _messages.Types.Error);
                duplicateTestCases.ForEach(x =>_outputAccess.WriteToConsole(string.Format(_messages.TestCases.Duplicate, x.Name, _messages.TestCases.GetDuplicateTestCaseNamesString(x.TestCases)), _messages.Types.Info));
                throw new InvalidOperationException(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count));
            }

           _outputAccess.WriteToConsole(string.Format(_messages.Stages.TestCase.Success, testCases.Count), _messages.Types.Success);
            return testCases;
        }
    }
}
