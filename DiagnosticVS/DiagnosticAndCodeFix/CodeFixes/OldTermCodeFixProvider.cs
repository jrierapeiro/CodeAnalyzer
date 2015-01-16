using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace DiagnosticAnalyzerAndCodeFix.CodeFixes
{
    [ExportCodeFixProvider(CodeAnalyzer.Analyzers.Diagnostics.OldTermDiagnosticAnalyzer.DiagnosticId, LanguageNames.CSharp), Shared]
    public class OldTermCodeFixProvider : CodeFixProvider
    {
      

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(CancellationToken.None);
            var token = root.FindToken(context.Span.Start);

            var variableDeclaration = token.Parent as VariableDeclaratorSyntax;
            if (variableDeclaration != null)
            {
                var semanticModel = await context.Document.GetSemanticModelAsync(CancellationToken.None);
                var symbol = semanticModel?.GetDeclaredSymbol(variableDeclaration, CancellationToken.None);
                var solution = context.Document.Project?.Solution;

                if (solution != null && symbol != null)
                {
                    var options = solution.Workspace.Options;
                    var newName = CodeAnalyzer.Analyzers.Diagnostics.OldTermDiagnosticAnalyzer.Terminology[token.Text];
                    var diagnostic = context.Diagnostics.First();
                    context.RegisterFix(CodeAction.Create("Change old terminology variable name to '" + newName + "'.", 
                        ct => Renamer.RenameSymbolAsync(solution, symbol, newName, options, ct)), diagnostic);
                }
            }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(CodeAnalyzer.Analyzers.Diagnostics.OldTermDiagnosticAnalyzer.DiagnosticId);
        }
    }
}