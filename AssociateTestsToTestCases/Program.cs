using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AssociateTestsToTestCases.Message;
using CommandLine;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimatch;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private const string AutomationName = "Automated";

        private static Messages _messages;
        private static MessageType _messageType;
        private static TestCaseAccess _testCaseAccess;

        private static bool _validationOnly;
        private static bool _verboseLogging;
        private static string[] _testAssemblyPaths;
        private static string _testType = string.Empty;

        private static void Main(string[] args)
        {
            _messages = new Messages();
            _messageType = new MessageType();

            WriteToConsole(_messageType.Stage, _messages.Stage.Arguments.Status);
            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(o =>
                    {
                        var minimatchPatterns = o.MinimatchPatterns.Split(';').Select(s => s.ToLowerInvariant()).ToArray();
                        var directory = o.Directory.ToLowerInvariant();
                        _testAssemblyPaths = ListTestAssemblyPaths(directory, minimatchPatterns);

                        _testCaseAccess = new TestCaseAccess(o.CollectionUri, o.PersonalAccessToken, o.ProjectName, o.TestPlanName);
                        _validationOnly = o.ValidationOnly;
                        _testType = o.TestType;
                        _verboseLogging = o.VerboseLogging;
                    });
            WriteToConsole(_messageType.Success, _messages.Stage.Arguments.Success);

            WriteToConsole(_messageType.Stage, _messages.Stage.DllTestMethods.Status);
            var testMethods = ListTestMethods(_testAssemblyPaths);

            if (testMethods.IsNullOrEmpty())
            {
                WriteToConsole(_messageType.Error, _messages.Stage.DllTestMethods.Failure);
                Environment.Exit(-1);
            }
            WriteToConsole(_messageType.Success, _messages.Stage.DllTestMethods.Success);

            WriteToConsole(_messageType.Stage, _messages.Stage.TestCases.Status);
            var testCases = _testCaseAccess.GetVstsTestCases();

            if (testCases.IsNullOrEmpty())
            {
                WriteToConsole(_messageType.Error, _messages.Stage.TestCases.Failure);
                Environment.Exit(-1);
            }
            WriteToConsole(_messageType.Success, _messages.Stage.TestCases.Success);

            //Console.WriteLine("Trying to reset the status of each test case");
            //var resetStatusTestCasesSuccess = testCaseAccess.ResetStatusTestCases();

            //if (!resetStatusTestCasesSuccess)
            //{
            //    Console.Write("[ERROR] Could not reset the status of each VSTS Test Case. Program has been terminated.\n");
            //    Environment.Exit(-1);
            //}
            //Console.WriteLine("[SUCCESS] VSTS Test Cases have been reset.\n");

            WriteToConsole(_messageType.Stage, _messages.Stage.Association.Status);
            AssociateTestCasesWithTestMethods(testMethods, testCases, _testCaseAccess, _validationOnly, _testType);
            WriteToConsole(_messageType.Success, _messages.Stage.Association.Success);

            WriteToConsole(_messageType.Stage, _messages.Stage.Summary.Status);
            WriteToConsole(_messageType.Summary, string.Format(_messages.Stage.Summary.Detailed, Counter.Counter.Success, Counter.Counter.Error, Counter.Counter.WarningMissingId + Counter.Counter.WarningTestMethodNotAvailable + Counter.Counter.WarningNoCorrespondingTestMethod, Counter.Counter.WarningMissingId, Counter.Counter.WarningTestMethodNotAvailable, Counter.Counter.WarningNoCorrespondingTestMethod));
            WriteToConsole(_messageType.Summary, string.Format(_messages.Stage.Summary.Overview, testCases.Count, Counter.Counter.Total));
        }

        private static void AssociateTestCasesWithTestMethods(MethodInfo[] testMethods, List<TestCase> testCases, TestCaseAccess vstsAccessor, bool validationOnly, string testType)
        {
            foreach (var testCase in testCases)
            {
                var testMethod = testMethods.SingleOrDefault(x => x.Name == testCase.Title);

                if (testCase.Id == null)
                {
                    WriteToConsole(_messageType.Warning, string.Format(_messages.Association.Failure.TestcaseWithNoIdSkipped, testCase.Title));
                    Counter.Counter.WarningMissingId += 1;
                    continue;
                }

                if (testCase.AutomationStatus == AutomationName && testMethod == null)
                {
                    WriteToConsole(_messageType.Warning, string.Format(_messages.Association.Failure.TestcaseCorrespondingTestMethodNotAvailable, testCase.Title, testCase.Id));
                    Counter.Counter.WarningTestMethodNotAvailable += 1;
                    continue;
                }

                if (testMethod == null)
                {
                    WriteToConsole(_messageType.Warning, string.Format(_messages.Association.Failure.TestcaseNoCorrespondingTestMethod, testCase.Title, testCase.Id));
                    Counter.Counter.WarningNoCorrespondingTestMethod += 1;
                    continue;
                }

                if (testCase.AutomationStatus == AutomationName)
                {
                    Counter.Counter.Total += 1;
                    continue;
                }

                var operationSuccess = vstsAccessor.AssociateTestCaseWithTestMethod((int)testCase.Id, $"{testMethod.DeclaringType.FullName}.{testMethod.Name}", testMethod.Module.Name, Guid.NewGuid().ToString(), validationOnly, testType);

                if (!operationSuccess)
                {
                    WriteToConsole(_messageType.Failure, string.Format(_messages.Association.Failure.TestcaseCouldNotBeAssociated, testCase.Title, testCase.Id));
                    Counter.Counter.Error += 1;
                    return;
                }

                if (_verboseLogging)
                {
                    WriteToConsole(_messageType.Success, string.Format(_messages.Association.Success, testCase.Title, testCase.Id));
                }

                Counter.Counter.Total += 1;
                Counter.Counter.Success += 1;
            }
        }

        private static MethodInfo[] ListTestMethods(string[] testAssemblyPaths)
        {
            var testMethods = new List<MethodInfo>();

            foreach (var testAssemblyPath in testAssemblyPaths)
            {
                var testAssembly = Assembly.LoadFrom(testAssemblyPath);
                testMethods.AddRange(testAssembly.GetTypes().Where(type => type.GetCustomAttribute<TestClassAttribute>() != null).SelectMany(type => type.GetMethods().Where(method => method.GetCustomAttribute<TestMethodAttribute>() != null)));
            }

            return testMethods.ToArray();
        }

        private static string[] ListTestAssemblyPaths(string directory, string[] minimatchPatterns)
        {
            var files = ListAllAccessibleFilesInDirectory(directory).Select(s => s.ToLowerInvariant()).ToArray();
            var matchingFilesToBeIncluded = new List<string>();

            foreach (var minimatchPattern in minimatchPatterns.Where(minimatchPattern => !minimatchPattern.StartsWith("!")))
            {
                var mm = new Minimatcher(minimatchPattern, new Minimatch.Options() { AllowWindowsPaths = true });
                matchingFilesToBeIncluded.AddRange(mm.Filter(files));
            }

            var matchingFilesToBeExcluded = new List<string>();
            foreach (var minimatchPattern in minimatchPatterns.Where(minimatchPattern => minimatchPattern.StartsWith("!")))
            {
                var actualMinimatchPattern = minimatchPattern.TrimStart('!');
                var mm = new Minimatcher(actualMinimatchPattern, new Minimatch.Options() { AllowWindowsPaths = true });
                matchingFilesToBeExcluded.AddRange(mm.Filter(files));
            }

            matchingFilesToBeIncluded.RemoveAll(x => matchingFilesToBeExcluded.Contains(x));

            return matchingFilesToBeIncluded.ToArray();
        }

        private static string[] ListAllAccessibleFilesInDirectory(string directory)
        {
            var files = new List<string>(Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly));
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                try
                {
                    files.AddRange(ListAllAccessibleFilesInDirectory(subDir));
                }
                catch (UnauthorizedAccessException)
                {
                    //Ignored by design, see https://stackoverflow.com/a/19137152. Comment is here to suppress analyzer warnings.
                }
            }

            return files.ToArray();
        }

        private static void WriteToConsole(string messageType, string message)
        {
            var space = new string(' ', _messageType.LongestTypeCount - messageType.Count());
            var outputMessage = $"[{messageType}]{space} {message}";
            Console.WriteLine(outputMessage);
        }

        private class Options
        {
            [Option('d', "directory", Required = true, HelpText = "The root directory to search in.")]
            public string Directory { get; set; }

            [Option('m', "minimatchpatterns", Required = true, HelpText = "Minimatch patterns to search for within the directory, separated by a semicolon.")]
            public string MinimatchPatterns { get; set; }

            [Option('p', "personalaccesstoken", Required = true, HelpText = "The personal access token used for accessing the Azure DevOps project.")]
            public string PersonalAccessToken { get; set; }

            [Option('u', "collectionuri", Required = true, HelpText = "The Azure DevOps collection Uri used for accessing the project test cases.")]
            public string CollectionUri { get; set; }

            [Option('n', "projectname", Required = true, HelpText = "The project name containing the test plan.")]
            public string ProjectName { get; set; }

            [Option('e', "testplanname", Required = true, HelpText = "The name of the test plan containing the test cases.")]
            public string TestPlanName { get; set; }

            [Option('t', "testtype", Required = false, Default = "", HelpText = "The automation test type.")]
            public string TestType { get; set; }

            [Option('v', "validationonly", Required = false, HelpText = "Indicates if you only want to validate the changes without saving the test cases.")]
            public bool ValidationOnly { get; set; }

            [Option('l', "verboselogging", Required = false, HelpText = "When Verbose logging is turned on it also outputs the successful matchings next to the warnings.")]
            public bool VerboseLogging { get; set; }
        }
    }
}
