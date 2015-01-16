using System.Threading.Tasks;
using CodeAnalyzer.Analyzers.Diagnostics;
using CodeAnalyzer.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeAnalyzer.Analyzers.Tests
{
    [TestClass]
    public class OldTermDiagnosticAnalyzerTests
    {
        [TestMethod]
        public async Task No_old_term_in_var_declaration()
        {
            var analyzer = new OldTermDiagnosticAnalyzer();
            var diagnostics = await analyzer.GetDiagnosticsInSimpleCodeAsync("string imageId = null;");
            Assert.AreEqual(0,diagnostics.Length);
        }

        [TestMethod]
        public async Task Old_term_in_var_declaration()
        {
            var analyzer = new OldTermDiagnosticAnalyzer();
            var diagnostics = await analyzer.GetDiagnosticsInSimpleCodeAsync("string iid = null;");
            Assert.AreEqual(1,diagnostics.Length);
        }



        
    }
}