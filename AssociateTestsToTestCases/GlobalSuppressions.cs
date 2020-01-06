// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3885:\"Assembly.Load\" should be used", Justification = "It is better to use LoadFrom as the location of the assembly file is known, plus we want to load its dependencies.", Scope = "member", Target = "~M:AssociateTestsToTestCases.Program.ListTestMethods(System.String[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3885:\"Assembly.Load\" should be used", Justification = "It is better to use LoadFrom as the location of the assembly file is known, plus we want to load its dependencies.", Scope = "member", Target = "~M:AssociateTestsToTestCases.Access.File.AssemblyHelper.LoadFrom(System.String)~System.Reflection.Assembly")]
