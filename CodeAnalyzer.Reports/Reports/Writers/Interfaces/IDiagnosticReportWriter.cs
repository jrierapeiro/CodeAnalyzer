using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Reports.Reports.Writers.Interfaces
{
    public interface IDiagnosticReportWriter
    {
        string Write(string projectName, List<Diagnostic> diagnostics, List<ProjectReport> projects);
    }
}