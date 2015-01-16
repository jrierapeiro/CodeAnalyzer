using System.Collections.Generic;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Reports.Reports;

namespace CodeAnalyzer.Reports.Models
{
    public class ReferencesReportModel
    {
        public NamespaceModel NamespaceModel { get; set; }
        public string BasePath { get; set; }
        public List<ProjectReport> Projects { get; set; }
        public bool WriteOnlyCodeWithWarnings { get; set; }
    }
}