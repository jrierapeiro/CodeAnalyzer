using System.Collections.Generic;
using System.IO;
using CodeAnalyzer.Reports.Core;
using CodeAnalyzer.Reports.Models;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;
using RazorEngine.Templating;

namespace CodeAnalyzer.Reports.Reports.Writers.Index
{
    public class IndexHtmlReportWriter : IIndexReportWriter
    {
        public string Write(string solutionFile, List<ProjectReport> projects)
        {
            var model = new IndexReportModel {SolutionFile = solutionFile, Projects = projects};
            var templatePath = Path.Combine(Settings.TemplateFolderPath, "IndexHtml.cshtml");
            var templateService = new TemplateService();
            var htmlBody = templateService.Parse(File.ReadAllText(templatePath), model, null, null);
            return htmlBody;
        }
    }
}