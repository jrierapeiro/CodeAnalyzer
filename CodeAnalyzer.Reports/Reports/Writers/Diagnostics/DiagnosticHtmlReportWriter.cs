using System.Collections.Generic;
using System.IO;
using CodeAnalyzer.Reports.Core;
using CodeAnalyzer.Reports.Models;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;
using Microsoft.CodeAnalysis;
using RazorEngine.Templating;

namespace CodeAnalyzer.Reports.Reports.Writers.Diagnostics
{
    public class DiagnosticHtmlReportWriter : IDiagnosticReportWriter
    {
        public string Write(string projectName, List<Diagnostic> diagnostics, List<ProjectReport> projects)
        {
            var model = new DiagnosticReportModel {ProjectName = projectName, Projects = projects, Diagnostics = diagnostics};
            var templatePath = Path.Combine(Settings.TemplateFolderPath, "DiagnosticHtml.cshtml");
            var templateService = new TemplateService();
            var htmlBody = templateService.Parse(File.ReadAllText(templatePath), model, null, null);
            return htmlBody;
        }
    }
}