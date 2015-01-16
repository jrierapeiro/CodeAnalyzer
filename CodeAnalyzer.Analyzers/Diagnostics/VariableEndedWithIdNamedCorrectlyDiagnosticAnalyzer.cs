using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzer.Analyzers.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VariableEndedWithIdNamedCorrectlyDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "VariableEndedWithIdNamedCorrectly";
        private const string Description = "A variable that ends with ID should end with \"Id\"";
        private const string Category = "Naming";
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var variable = context.Node as VariableDeclarationSyntax;
            CheckDeclaration(context, variable);
        }

        private static void CheckDeclaration(SyntaxNodeAnalysisContext context, VariableDeclarationSyntax variable)
        {
            var identifierToken = variable?.Variables.FirstOrDefault(v => v.CSharpKind() == SyntaxKind.VariableDeclarator);
            if (identifierToken?.Identifier.Text != null && identifierToken.Identifier.Text.EndsWith("ID"))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, identifierToken.Identifier.GetLocation(), identifierToken?.Identifier.Text));
            }
        }

        private static readonly string MessageFormat = "Make sure the name of this variable ({0}) ends with \"Id\".";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);
    }
}