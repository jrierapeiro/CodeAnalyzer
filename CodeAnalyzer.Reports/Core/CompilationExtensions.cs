using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzer.Reports.Core
{
    public static class CompilationExtensions
    {
        public static Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(this Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            options = options ?? new AnalyzerOptions(ImmutableArray<AdditionalStream>.Empty, ImmutableDictionary<string, string>.Empty);
            Compilation newCompilation = null;
            var analyzerDriver = AnalyzerDriver.Create(compilation, analyzers, options, out newCompilation, cancellationToken);

            // We need to generate compiler events in order for the event queue to be populated and the analyzer driver to return diagnostics.
            // So we'll call GetDiagnostics which will generate all events except for those on emit.
            newCompilation.GetDiagnostics(cancellationToken);

            return analyzerDriver.GetDiagnosticsAsync();
        }
    }
}