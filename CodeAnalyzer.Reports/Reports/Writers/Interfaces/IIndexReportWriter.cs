using System.Collections.Generic;

namespace CodeAnalyzer.Reports.Reports.Writers.Interfaces
{
    public interface IIndexReportWriter
    {
        string Write(string solutionFile, List<ProjectReport> projects);
    }
}