using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace DiagnosticAnalyzerAndCodeFix.CodeFixes
{
    [ExportCodeFixProvider(CodeAnalyzer.Analyzers.Diagnostics.AsyncMethodNotNamedCorrectlyDiagnosticAnalyzer.DiagnosticId, LanguageNames.CSharp), Shared]
    public class AsyncMethodNotNamedCorrectlyCodeFixProvider : CodeFixProvider
    {

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var token = root.FindToken(context.Span.Start);

            var methodDeclaration = token.Parent as MethodDeclarationSyntax;
            if (methodDeclaration != null)
            {
                var semanticModel = await context.Document.GetSemanticModelAsync();
                
                var symbol = semanticModel?.GetDeclaredSymbol(methodDeclaration);
                var solution = context.Document.Project?.Solution;

                if (symbol != null && solution != null)
                {
                    var options = solution.Workspace.Options;
                    var newName = token.Text + "Async";
                    var diagnostic = context.Diagnostics.First();
                    context.RegisterFix(CodeAction.Create("Change method name to '" + newName + "'.", 
                        (ct) => Renamer.RenameSymbolAsync(solution, symbol, newName, options, ct)), diagnostic);
                }
            }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return ImmutableArray.Create(CodeAnalyzer.Analyzers.Diagnostics.AsyncMethodNotNamedCorrectlyDiagnosticAnalyzer.DiagnosticId);
        }
    }
}