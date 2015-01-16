using System.Collections.Generic;
using System.IO;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Reports.Core;
using CodeAnalyzer.Reports.Models;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;
using RazorEngine.Templating;

namespace CodeAnalyzer.Reports.Reports.Writers.Reports
{
    public class HtmlReportWriter : IReportWriter
    {
        public string Write(string basePath, NamespaceModel namespaceModel, List<ProjectReport> projects, bool writeOnlyCodeWithWarnings = false)
        {
            var model = new ReferencesReportModel {NamespaceModel = namespaceModel, Projects = projects, BasePath = basePath, WriteOnlyCodeWithWarnings = writeOnlyCodeWithWarnings};
            var templatePath = Path.Combine(Settings.TemplateFolderPath, "ReferencesHtml.cshtml");
            var templateService = new TemplateService();
            var htmlBody = templateService.Parse(File.ReadAllText(templatePath), model, null, null);
            return htmlBody;
        }
    }
}