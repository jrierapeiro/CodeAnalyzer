using CodeAnalyzer.Core.Models;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Analyzers.References
{
    public abstract class ReferencesDiagnosticAnalyzer
    {
        public abstract void AnalyzeNode(SymbolModel model, INamedTypeSymbol parentSymbol, ISymbol member);
        public abstract bool IsApplicable(INamedTypeSymbol parentSymbol);
    }
}