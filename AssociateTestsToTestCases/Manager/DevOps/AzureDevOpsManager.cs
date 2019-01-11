﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Common;
using AssociateTestsToTestCases.Message;
using AssociateTestsToTestCases.Manager.File;
using AssociateTestsToTestCases.Access.DevOps;
using AssociateTestsToTestCases.Manager.Output;

namespace AssociateTestsToTestCases.Manager.DevOps
{
    public class AzureDevOpsManager : IDevOpsManager
    {
        private readonly Messages _messages;
        private readonly IOutputManager _outputManager;
        private readonly IDevOpsAccess _devOpsAccess;
        private readonly Counter.Counter _counter;

        public AzureDevOpsManager(Messages messages, IOutputManager outputManager, IDevOpsAccess devOpsAccess, Counter.Counter counter)
        {
            _messages = messages;
            _outputManager = outputManager;
            _devOpsAccess = devOpsAccess;
            _counter = counter;
        }

        public bool TestPlanIsEmpty()
        {
            return _devOpsAccess.GetTestCasesId().IsNullOrEmpty();
        }

        public TestCase[] GetTestCases()
        {
            _outputManager.WriteToConsole(_messages.Stages.TestCase.Status, _messages.Types.Stage);

            var testCases = _devOpsAccess.GetTestCases();
            ValidateTestCasesIsNullOrEmpty(testCases);
            ValidateTestCasesHasDuplicates(_devOpsAccess.ListDuplicateTestCases(testCases));

            _outputManager.WriteToConsole(string.Format(_messages.Stages.TestCase.Success, testCases.Length), _messages.Types.Success);
            return testCases;
        }

        public void Associate(TestMethod[] testMethods, TestCase[] testCases)
        {
            _outputManager.WriteToConsole(_messages.Stages.Association.Status, _messages.Types.Stage);

            ValidateTestMethodsNotAvailable(_devOpsAccess.ListTestCasesWithNotAvailableTestMethods(testMethods, testCases));
            ValidateAssociationErrors(_devOpsAccess.Associate(testMethods, GetTestCasesDictionary(testCases)), testMethods, testCases);

            _outputManager.WriteToConsole(string.Format(_messages.Stages.Association.Success, _counter.Success.Total), _messages.Types.Success);
        }

        private Dictionary<string, TestCase> GetTestCasesDictionary(TestCase[] testCases)
        {
            return testCases.ToDictionary(v => v.Title, t => t);
        }

        #region Validations

        private void ValidateTestCasesIsNullOrEmpty(TestCase[] testCases)
        {
            if (testCases.IsNullOrEmpty())
            {
                _outputManager.WriteToConsole(_messages.Stages.TestCase.Failure, _messages.Types.Error);

                throw new InvalidOperationException(_messages.Stages.TestCase.Failure);
            }
        }

        private void ValidateTestCasesHasDuplicates(List<DuplicateTestCase> duplicateTestCases)
        {
            if (duplicateTestCases.Count != 0)
            {
                _outputManager.WriteToConsole(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count), _messages.Types.Error);
                duplicateTestCases.ForEach(x => _outputManager.WriteToConsole(string.Format(_messages.TestCases.Duplicate, x.Name, _messages.TestCases.GetDuplicateTestCaseNamesString(x.TestCases)), _messages.Types.Info));

                throw new InvalidOperationException(string.Format(_messages.Stages.TestCase.Duplicate, duplicateTestCases.Count));
            }
        }

        private void ValidateTestMethodsNotAvailable(List<TestCase> testMethodsNotAvailable)
        {
            if (testMethodsNotAvailable.Count != 0)
            {
                testMethodsNotAvailable.ForEach(x => _outputManager.WriteToConsole(string.Format(_messages.Associations.TestCaseWithNotAvailableTestMethod, x.Title, x.Id, x.AutomatedTestName), _messages.Types.Warning, _messages.Reasons.AssociatedTestMethodNotAvailable));

                _counter.Warning.TestMethodNotAvailable += testMethodsNotAvailable.Count;
            }
        }

        private void ValidateAssociationErrors(int totalAssociationErrors, TestMethod[] testMethods, TestCase[] testCases)
        {
            if (totalAssociationErrors != 0)
            {
                _outputManager.WriteToConsole(string.Format(_messages.Stages.Association.Failure, totalAssociationErrors), _messages.Types.Error);
                _outputManager.OutputSummary(testMethods, testCases);

                throw new InvalidOperationException(string.Format(_messages.Stages.Association.Failure, totalAssociationErrors));
            }
        }

        #endregion
    }
}
