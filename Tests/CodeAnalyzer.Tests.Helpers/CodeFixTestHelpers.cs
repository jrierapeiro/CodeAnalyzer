using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeAnalyzer.Tests.Helpers
{
    public static class CodeFixTestHelpers
    {
        public static async Task CheckSingleFixAsync(string code, string expectedCode, string expectedDescription,
            CodeFixProvider codeFixProvider, params DiagnosticAnalyzer[] analyzers)
        {
            var fixes = await GetFixesAsync(code, codeFixProvider, analyzers);

            Assert.AreEqual(1, fixes.Count);
            var fix = fixes.Single();
            Assert.AreEqual(expectedDescription, fix.Item1.Title);
            Assert.AreEqual(expectedCode, fix.Item2);
        }

        public static async Task CheckSingleFixAsync(string code, string spanText, string expectedCode,
            string expectedDescription, CodeFixProvider codeFixProvider, DiagnosticDescriptor diagnosticDescriptor,  params object[] messageArgs)
        {
            var fixes = await GetFixesAsync(code, codeFixProvider, diagnosticDescriptor, spanText, messageArgs);

            Assert.AreEqual(1, fixes.Count);
            var fix = fixes.Single();
            Assert.AreEqual(expectedDescription, fix.Item1.Title);
            Assert.AreEqual(expectedCode, fix.Item2);
        }

        public static void CheckSingleFix(string code, string spanText, string expectedCode,
            string expectedDescription, CodeFixProvider codeFixProvider, DiagnosticDescriptor diagnosticDescriptor, params object[] messageArgs)
        {
            var task = CheckSingleFixAsync(code, spanText, expectedCode, expectedDescription, codeFixProvider,
                diagnosticDescriptor);
            task.Wait();
        }

        public static async Task CheckSingleFixAsync(string code, string spanText, string expectedCode,
            CodeFixProvider codeFixProvider, DiagnosticDescriptor diagnosticDescriptor, params object[] messageArgs)
        {
            var fixes = await GetFixesAsync(code, codeFixProvider, diagnosticDescriptor, spanText);

            Assert.AreEqual(1, fixes.Count);
            var fix = fixes.Single();
            Assert.AreEqual(expectedCode, fix.Item2);
        }

        public static void CheckSingleFix(string code, string spanText, string expectedCode,
            CodeFixProvider codeFixProvider, DiagnosticDescriptor diagnosticDescriptor, params object[] messageArgs)
        {
            var task = CheckSingleFixAsync(code, spanText, expectedCode, codeFixProvider,
                diagnosticDescriptor);
            task.Wait();
        }

        public static async Task<ImmutableList<Tuple<CodeAction, string>>> GetFixesAsync(string code,
            CodeFixProvider codeFixProvider, DiagnosticDescriptor diagnosticDescriptor, string spanText, params object[] messageArgs)
        {
            var solution = await SingleDocumentTestSolution.CreateAsync(code);

            var span = SpanFromText(code, spanText);
            var location = Location.Create(solution.DocumentSyntaxTree, span);
            var diagnostic = Diagnostic.Create(diagnosticDescriptor, location, messageArgs);

            return await GetFixesAsync(codeFixProvider, solution.Document, ImmutableList.Create(diagnostic));
        }

        private static TextSpan SpanFromText(string code, string spanText)
        {
            if (spanText.StartsWith("#"))
            {
                var interesting = spanText.Select((c, i) => Tuple.Create(c != '#', i)).Where(t => t.Item1);
                var start = interesting.First().Item2;
                var end = interesting.Last().Item2;
                var length = end - start + 1;

                return new TextSpan(start, length);
            }
            else
            {
                var start = code.IndexOf(spanText);
                if (start == -1)
                {
                    throw new ArgumentException(
                        string.Format("Unable to find span '{0}' in '{1}'", spanText, code),
                        "spanText");
                }
                if (start != code.Length - 1 && code.IndexOf(spanText, start + 1) != -1)
                {
                    throw new ArgumentException(
                        string.Format("Found span '{0}' more than one time in '{1}'", spanText, code),
                        "spanText");
                }

                return new TextSpan(start, spanText.Length);
            }
        }

        public static async Task<ImmutableList<Tuple<CodeAction, string>>> GetFixesAsync(string code,
            CodeFixProvider codeFixProvider, params DiagnosticAnalyzer[] analyzers)
        {
            var solution = await SingleDocumentTestSolution.CreateAsync(code);

            var diagnostics = await GetFixableDiagnostics(codeFixProvider, analyzers.ToImmutableArray(),
                solution.DocumentSyntaxTree, solution.SolutionCompilation);

            return await GetFixesAsync(codeFixProvider, solution.Document, diagnostics);
        }

        private static async Task<ImmutableList<Tuple<CodeAction, string>>> GetFixesAsync( CodeFixProvider codeFixProvider, Document document, ImmutableList<Diagnostic> diagnostics)
        {
            var codeActions = await GetCodeActionsFromFixesAsync(codeFixProvider, document, diagnostics);

            var codeChangeTasks = codeActions.Select(a => GetChangedText(a, document.Id));
            var codeChanges = await Task.WhenAll(codeChangeTasks);

            var result = codeActions.Zip(codeChanges, (a, t) => Tuple.Create(a, t));

            return ImmutableList<Tuple<CodeAction, string>>.Empty.AddRange(result);
        }

        private static async Task<IEnumerable<CodeAction>> GetCodeActionsFromFixesAsync(
            CodeFixProvider codeFixProvider, Document document, ImmutableList<Diagnostic> diagnostics)
        {
            var codeActions = new ConcurrentBag<CodeAction>();
            var getTasks = ImmutableList<Task>.Empty;

            foreach (var diagnostic in diagnostics)
            {
                var context = new CodeFixContext(document, diagnostic,
                    (action, matchedDiags) => codeActions.Add(action), CancellationToken.None);
                var task = codeFixProvider.ComputeFixesAsync(context);
                getTasks = getTasks.Add(task);
            }

            await Task.WhenAll(getTasks);

            return codeActions;
        }

        private static async Task<ImmutableList<Diagnostic>> GetFixableDiagnostics(CodeFixProvider codeFixProvider,
            ImmutableArray<DiagnosticAnalyzer> analyzers, SyntaxTree tree, Compilation compilation)
        {
            var fixableDiagnosticIds = codeFixProvider.GetFixableDiagnosticIds();
            var allDiagnostics = await DiagnosticTestHelpers.AnalyzeTreeAsync(analyzers, tree, compilation);
            return allDiagnostics.Where(d => fixableDiagnosticIds.Contains(d.Id)).ToImmutableList();
        }

        private static async Task<string> GetChangedText(CodeAction codeAction, DocumentId documentId)
        {
            var operations = await codeAction.GetOperationsAsync(CancellationToken.None);
            var operation = (ApplyChangesOperation) operations.Single();
            var newDoc = operation.ChangedSolution.GetDocument(documentId);
            var text = await newDoc.GetTextAsync();
            return text.ToString();
        }
    }
}