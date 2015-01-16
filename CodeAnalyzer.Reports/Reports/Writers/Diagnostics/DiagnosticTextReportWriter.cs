using System;
using System.Collections.Generic;
using System.Text;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Reports.Reports.Writers.Diagnostics
{
    public class DiagnosticTextReportWriter : IDiagnosticReportWriter
    {
        private StringBuilder _content;

        public DiagnosticTextReportWriter()
        {
            _content = new StringBuilder();
        }

        public string Write(string projectName, List<Diagnostic> diagnostics, List<ProjectReport> projects)
        {
            _content.AppendLine(String.Format("Diagnostics for {0}", projectName));
            foreach (var diagnostic in diagnostics)
            {
                var strDetail = String.Empty;
                strDetail += String.Format("Info: {0}", diagnostic);
                strDetail += "Warning Level: " + diagnostic.WarningLevel + Environment.NewLine;
                strDetail += "Severity Level: " + diagnostic.Severity + Environment.NewLine;
                strDetail += "Location: " + diagnostic.Location.Kind + Environment.NewLine;
                strDetail += "Character at: " + diagnostic.Location.GetLineSpan().StartLinePosition.Character +
                             Environment.NewLine;
                strDetail += "On Line: " + diagnostic.Location.GetLineSpan().StartLinePosition.Line +
                             Environment.NewLine;
                strDetail += Environment.NewLine;
                _content.AppendLine(strDetail);
            }
            return _content.ToString();
        }

        public void Dispose()
        {
            _content = null;
        }
    }
}