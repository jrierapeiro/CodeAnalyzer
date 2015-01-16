using CodeAnalyzer.Analyzers.Diagnostics;
using DiagnosticAnalyzerAndCodeFix.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiagnosticAndCodeFix.Tests
{
    [TestClass]
    public class OldTermCodeFixTests
    {
        [TestMethod]
        public void Fix_on_standard_call()
        {
            CodeAnalyzer.Tests.Helpers.CodeFixTestHelpers.CheckSingleFixAsync(
                "using System;class Foo{void Bar(){string iid = null;}}",
                "iid",
                "using System;class Foo{void Bar(){string imageId = null;}}",
                "Change old terminology variable name to 'imageId'.",
                new OldTermCodeFixProvider(),
                OldTermDiagnosticAnalyzer.Rule, "iid").Wait();
        }
    }
}