using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiagnosticAnalyzerAndCodeFix.DiagnosticAnalyzers
{
    
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodNotNamedCorrectlyDiagnosticAnalyzer : CodeAnalyzer.Analyzers.Diagnostics.AsyncMethodNotNamedCorrectlyDiagnosticAnalyzer
    {
       
    }
}
