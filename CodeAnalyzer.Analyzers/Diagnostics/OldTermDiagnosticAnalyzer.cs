using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzer.Analyzers.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OldTermDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "OldTerm";
        public const string Description = "A variable should not be named with old terms  \"dbid, hid, iid, tid, ucdmid\"";
        public const string Category = "Naming";
        public static readonly Dictionary<string, string> Terminology = new Dictionary<string, string>
        { {"dbid", "collectionId"}, {"hid", "hintId"}, {"iid", "imageId"}, {"tid", "treeId"}, {"ucdmid", "userId"}};
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var variable = context.Node as VariableDeclarationSyntax;

            if (variable != null)
            {
                var identifierToken = variable.Variables.FirstOrDefault(v => v.CSharpKind() == SyntaxKind.VariableDeclarator);
                if (identifierToken?.Identifier.Text != null && Terminology.Keys.Contains(identifierToken?.Identifier.Text))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, identifierToken.Identifier.GetLocation(),
                        identifierToken?.Identifier.Text));
                }
            }
        }

        public static readonly string MessageFormat = "Make sure the name of this variable ({0}) use the new terminology  \"CollectionId, HintId, ImageId, TreeId, UserId\".";
        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);
    }
}