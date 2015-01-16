using System.Collections.Generic;
using CodeAnalyzer.Reports.Reports;

namespace CodeAnalyzer.Reports.Models
{
    public class IndexReportModel
    {
        public string SolutionFile { get; set; }
        public List<ProjectReport> Projects { get; set; }
    }
}