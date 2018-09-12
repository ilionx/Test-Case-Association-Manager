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
        private static void Main(string[] args)
        {
            VstsAccessor vstsAccessor = null;
            string[] testAssemblyPaths = null;

            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                var minimatchPatterns = o.MinimatchPatterns.Split(';');
                testAssemblyPaths = ListTestAssemblyPaths(o.Directory, minimatchPatterns);

                vstsAccessor = new VstsAccessor(o.CollectionUri, o.PersonalAccessToken);
            });

            Console.WriteLine("Trying to retrieve the DLL Test Methods...");
            var testMethods = ListTestMethods(testAssemblyPaths);

            if (testMethods.IsNullOrEmpty())
            {
                Console.WriteLine("[ERROR] Could not retrieve the DLL Test Methods. Program has been terminated.\n");
                Environment.Exit(-1); // Todo: will this even throw an error?
            }
            Console.WriteLine("[SUCCESS] DLL Test Methods have been obtained.\n");

            Console.WriteLine("Trying to retrieve the VSTS Test Cases...");
            var testCases = vstsAccessor.GetVstsTestCases();

            if (testCases.IsNullOrEmpty())
            {
                Console.WriteLine("[ERROR] Could not retrieve the VSTS Test Cases. Program has been terminated.\n");
                Environment.Exit(-1); // Todo: will this even throw an error?
            }
            Console.WriteLine("[SUCCESS] VSTS Test Cases have been obtained.\n");

            Console.WriteLine("Trying to Associate Work Items with Test Methods...");
            AssociateWorkItemsWithTestMethods(testMethods, testCases, vstsAccessor);

            Console.WriteLine("[FINISH] Workitems and Test Methods have been associated.");
        }

        private static void AssociateWorkItemsWithTestMethods(MethodInfo[] testMethods, List<VstsTestCase> testCases, VstsAccessor vstsAccessor)
        {
            foreach (var testCase in testCases)
            {
                var testMethod = testMethods.SingleOrDefault(x => x.Name == testCase.Title);

                if (testMethod == null)
                {
                    Console.WriteLine($"[WARNING] Test case '{testCase.Title}' [Id: {testCase.Id}] has no corresponding automated test."); // Todo: a lot of false positives will occur as the list of test cases is unfiltered. 
                    continue;
                }

                if (testCase.Id == null)
                {
                    Console.WriteLine($"[WARNING] Test case '{testCase.Title}' does not contain an Id, and therefore it will be skipped.");
                    continue;
                }

                var operationSuccess = vstsAccessor.AssociateTestCaseWithTestMethod((int)testCase.Id, $"{testMethod.DeclaringType.FullName}.{testMethod.Name}", testMethod.Module.Name, Guid.NewGuid().ToString());

                Console.WriteLine(operationSuccess
                    ? $"[SUCCESS] Test case '{testCase.Title}' [Id: {testCase.Id}] has been associated with the corresponding automated test."
                    : $"[ERROR] Test case '{testCase.Title}' [Id: {testCase.Id}] could not be associated with the corresponding automated test.");
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

            [Option('t', "personal access token", Required = true, HelpText = "The personal access token used for accessing the Visual Studio Team Services project")]
            public string PersonalAccessToken { get; set; }

            [Option('u', "collection uri", Required = true, HelpText = "The collection uri used for accessing the project work items")]
            public string CollectionUri { get; set; }
        }
    }
}
