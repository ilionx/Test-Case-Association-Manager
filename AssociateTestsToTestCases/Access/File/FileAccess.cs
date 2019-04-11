using System;
using System.IO;
using Minimatch;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssociateTestsToTestCases.Access.File
{
    public class FileAccess : IFileAccess
    {
        private readonly AssemblyHelper _assemblyHelper;

        public FileAccess(AssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        public MethodInfo[] ListTestMethods(string[] testAssemblyPaths)
        {
            var testMethods = new List<MethodInfo>();

            foreach (var testAssemblyPath in testAssemblyPaths)
            {
                var testAssembly = _assemblyHelper.LoadFrom(testAssemblyPath);
                testMethods.AddRange(testAssembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<TestClassAttribute>() != null)
                    .SelectMany(type => type.GetMethods()
                        .Where(method => method.GetCustomAttribute<TestMethodAttribute>() != null)));
            }

            return testMethods.ToArray();
        }

        public List<DuplicateTestMethod> ListDuplicateTestMethods(MethodInfo[] testMethods)
        {
            var duplicateTestMethods = new List<DuplicateTestMethod>();

            var duplicates = testMethods.Select(x => x.Name).GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            foreach (var duplicate in duplicates)
            {
                duplicateTestMethods.Add(new DuplicateTestMethod(duplicate, testMethods.Where(y => y.Name.Equals(duplicate)).ToArray()));
            }

            return duplicateTestMethods;
        }

        public string[] ListTestAssemblyPaths(string directory, string[] minimatchPatterns)
        {
            var files = ListAllAccessibleFilesInDirectory(directory);

            var matchingFilesToBeIncluded = new List<string>();
            foreach (var minimatchPattern in minimatchPatterns.Where(minimatchPattern => !minimatchPattern.StartsWith("!")))
            {
                var mm = new Minimatcher(minimatchPattern, new Minimatch.Options() { AllowWindowsPaths = true, IgnoreCase = true });
                matchingFilesToBeIncluded.AddRange(mm.Filter(files));
            }

            var matchingFilesToBeExcluded = new List<string>();
            foreach (var minimatchPattern in minimatchPatterns.Where(minimatchPattern => minimatchPattern.StartsWith("!")))
            {
                var actualMinimatchPattern = minimatchPattern.TrimStart('!');
                var mm = new Minimatcher(actualMinimatchPattern, new Minimatch.Options() { AllowWindowsPaths = true, IgnoreCase = true });
                matchingFilesToBeExcluded.AddRange(mm.Filter(files));
            }

            matchingFilesToBeIncluded.RemoveAll(x => matchingFilesToBeExcluded.Contains(x));

            return matchingFilesToBeIncluded.ToArray();
        }

        private string[] ListAllAccessibleFilesInDirectory(string directory)
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
    }
}
