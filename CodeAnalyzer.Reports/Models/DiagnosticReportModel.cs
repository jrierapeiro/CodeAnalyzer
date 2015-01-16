using System.Collections.Generic;
using CodeAnalyzer.Reports.Reports;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Reports.Models
{
    public class DiagnosticReportModel
    {
        public string ProjectName { get; set; }
        public List<ProjectReport> Projects { get; set; }
        public List<Diagnostic> Diagnostics { get; set; }
    }
}