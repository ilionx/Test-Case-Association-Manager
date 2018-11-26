using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Event;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Access.TestCase;

namespace AssociateTestsToTestCases.Manager.TestCase
{
    public class TestCaseManager
    {
        private readonly Messages _messages;
        private readonly TestCaseAccess _testCaseAccess;
        private readonly WriteToConsoleEventLogger _writeToConsoleEventLogger;

        public TestCaseManager(Messages messages, TestCaseAccess testCaseAccess, WriteToConsoleEventLogger writeToConsoleEventLogger)
        {
            _messages = messages;
            _testCaseAccess = testCaseAccess;
            _writeToConsoleEventLogger = writeToConsoleEventLogger;
        }

        public List<Access.TestCase.TestCase> GetTestCases()
        {
            _writeToConsoleEventLogger.Write(_messages.Stages.TestCase.Status, _messages.Types.Stage);
            var testCases = _testCaseAccess.GetTestCases();

            if (testCases.IsNullOrEmpty())
            {
                _writeToConsoleEventLogger.Write(_messages.Stages.TestCase.Failure, _messages.Types.Error);
                Environment.Exit(-1);
            }

            var duplicateTestCases = _testCaseAccess.ListDuplicateTestCases(testCases);
            if (duplicateTestCases.Count != 0)
            {
                _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count), _messages.Types.Error);
                duplicateTestCases.ForEach(x => _writeToConsoleEventLogger.Write(string.Format(_messages.TestCases.Duplicate, x.Name, _messages.TestCases.GetDuplicateTestCaseNamesString(x.TestCases)), _messages.Types.Info));
                Environment.Exit(-1);
            }

            _writeToConsoleEventLogger.Write(string.Format(_messages.Stages.TestCase.Success, testCases.Count), _messages.Types.Success);
            return testCases;
        }
    }
}
