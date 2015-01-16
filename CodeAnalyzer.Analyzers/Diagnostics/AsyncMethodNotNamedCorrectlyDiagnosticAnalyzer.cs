using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzer.Analyzers.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodNotNamedCorrectlyDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AsyncMethodNotNamedCorrectly";
        public const string Description = "A method ({0}) with an async modifier needs to end with \"Async\"";
        public const string Category = "Async";
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as MethodDeclarationSyntax;

            if (method != null && (method.Modifiers.Any(SyntaxKind.AsyncKeyword)
                                   && method.ReturnType is GenericNameSyntax
                                   && !method.Identifier.Text.EndsWith("Async")
                                   && method.DescendantNodes().OfType<AwaitExpressionSyntax>().Any()))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
            }
        }

        public static readonly string MessageFormat = "Make sure the name of this method ({0}) ends with \"Async\".";
        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);
    }
}