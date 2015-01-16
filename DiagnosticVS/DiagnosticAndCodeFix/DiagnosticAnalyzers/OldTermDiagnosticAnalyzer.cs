using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiagnosticAnalyzerAndCodeFix.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OldTermDiagnosticAnalyzer : CodeAnalyzer.Analyzers.Diagnostics.OldTermDiagnosticAnalyzer
    {
       
    }
}