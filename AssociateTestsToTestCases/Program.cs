using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimatch;

namespace AssociateTestsToTestCases
{
    internal static class Program
    {
        private const string AutomationName = "Automated";

        private static bool _validationOnly;
        private static string _testType = "";
        private static string[] _testAssemblyPaths;
        private static TestCaseAccess _testCaseAccess;

        private static int _errorAssociationCount;
        private static int _totalAssociationCount;
        private static int _successfulAssociationCount;
        private static int _warningMissingIdAssociationCount;
        private static int _warningNoCorrespondingAutomatedTestAssociationCount;
        private static int _warningAutomatedTestNotAvailableAnymoreAssociationCount;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(o =>
                    {
                        var minimatchPatterns = o.MinimatchPatterns.Split(';');
                        _testAssemblyPaths = ListTestAssemblyPaths(o.Directory, minimatchPatterns);

                        _testCaseAccess = new TestCaseAccess(o.CollectionUri, o.PersonalAccessToken);
                        _validationOnly = o.ValidationOnly;
                        _testType = o.TestType;
                    });

            Console.WriteLine("Trying to retrieve the DLL Test Methods...");
            var testMethods = ListTestMethods(_testAssemblyPaths);

            if (testMethods.IsNullOrEmpty())
            {
                Console.WriteLine("[ERROR] Could not retrieve the DLL Test Methods. Program has been terminated.\n");
                Environment.Exit(-1);
            }
            Console.WriteLine("[SUCCESS] DLL Test Methods have been obtained.\n");

            Console.WriteLine("Trying to retrieve the VSTS Test Cases...");
            var testCases = _testCaseAccess.GetVstsTestCases();

            if (testCases.IsNullOrEmpty())
            {
                Console.WriteLine("[ERROR] Could not retrieve the VSTS Test Cases. Program has been terminated.\n");
                Environment.Exit(-1);
            }
            Console.WriteLine("[SUCCESS] VSTS Test Cases have been obtained.\n");

            //Console.WriteLine("Trying to reset the status of each test case");
            //var resetStatusTestCasesSuccess = testCaseAccess.ResetStatusTestCases();

            //if (!resetStatusTestCasesSuccess)
            //{
            //    Console.Write("[ERROR] Could not reset the status of each VSTS Test Case. Program has been terminated.\n");
            //    Environment.Exit(-1);
            //}
            //Console.WriteLine("[SUCCESS] VSTS Test Cases have been reset.\n");

            Console.WriteLine("Trying to Associate Work Items with Test Methods...");
            AssociateWorkItemsWithTestMethods(testMethods, testCases, _testCaseAccess, _validationOnly, _testType);

            Console.WriteLine("[FINISH]  Work Items and Test Methods have been associated.");
            Console.WriteLine($"[SUMMARY] Success: {_successfulAssociationCount} | Errors: {_errorAssociationCount} | Warnings: [Missing Id: {_warningMissingIdAssociationCount} - Automated Test N\\A anymore: {_warningAutomatedTestNotAvailableAnymoreAssociationCount} - Automated Test not found: {_warningNoCorrespondingAutomatedTestAssociationCount}]");
            Console.WriteLine($"[SUMMARY] Total VSTS Test Cases: {testCases.Count} | Total Automated Test Cases: {_totalAssociationCount}");
        }

        private static void AssociateWorkItemsWithTestMethods(MethodInfo[] testMethods, List<TestCase> testCases, TestCaseAccess vstsAccessor, bool validationOnly, string testType)
        {
            foreach (var testCase in testCases)
            {
                var testMethod = testMethods.SingleOrDefault(x => x.Name == testCase.Title);

                if (testCase.Id == null)
                {
                    Console.WriteLine($"[WARNING] Test case '{testCase.Title}' does not contain an Id, and therefore it will be skipped.");
                    _warningMissingIdAssociationCount += 1;
                    continue;
                }

                if (testCase.AutomationStatus == AutomationName && testMethod == null)
                {
                    Console.WriteLine($"[WARNING] Test case '{testCase.Title}' [Id: {testCase.Id}] has been associated in the past, but the corresponding automated test is not available anymore.");
                    _warningAutomatedTestNotAvailableAnymoreAssociationCount += 1;
                    continue;
                }

                if (testMethod == null)
                {
                    Console.WriteLine($"[WARNING] Test case '{testCase.Title}' [Id: {testCase.Id}] has no corresponding automated test.");
                    _warningNoCorrespondingAutomatedTestAssociationCount += 1;
                    continue;
                }

                if (testCase.AutomationStatus == AutomationName)
                {
                    _totalAssociationCount += 1;
                    continue;
                }

                var operationSuccess = vstsAccessor.AssociateTestCaseWithTestMethod((int)testCase.Id, $"{testMethod.DeclaringType.FullName}.{testMethod.Name}", testMethod.Module.Name, Guid.NewGuid().ToString(), validationOnly, testType);

                if (!operationSuccess)
                {
                    Console.WriteLine($"[ERROR] Test case '{testCase.Title}' [Id: {testCase.Id}] could not be associated with the corresponding automated test.");
                    _errorAssociationCount += 1;
                    return;
                }

                Console.WriteLine($"[SUCCESS] Test case '{testCase.Title}' [Id: {testCase.Id}] has been associated with the corresponding automated test.");
                _totalAssociationCount += 1;
                _successfulAssociationCount += 1;
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
            var files = ListAllAccessibleFilesInDirectory(directory);
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

        private class Options
        {
            [Option('d', "directory", Required = true, HelpText = "The root directory to search")]
            public string Directory { get; set; }

            [Option('m', "minimatch patterns", Required = true, HelpText = "Supports multiple minimatch patterns, separated by a semicolon.")]
            public string MinimatchPatterns { get; set; }

            [Option('p', "personal access token", Required = true, HelpText = "The personal access token used for accessing the Visual Studio Team Services project")]
            public string PersonalAccessToken { get; set; }

            [Option('t', "test type", Required = false, Default = "", HelpText = "The automation test type")]
            public string TestType { get; set; }

            [Option('u', "collection uri", Required = true, HelpText = "The collection uri used for accessing the project work items")]
            public string CollectionUri { get; set; }

            [Option('v', "validation only", Required = false, HelpText = "The collection uri used for accessing the project work items")]
            public bool ValidationOnly { get; set; }
        }
    }
}
