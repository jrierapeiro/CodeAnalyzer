using System.Collections.Generic;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Reports.Reports.Writers.Interfaces
{
    public interface IReportWriter
    {
        string Write(string basePath, NamespaceModel namespaceModel, List<ProjectReport> projects, bool writeOnlyCodeWithWarnings = false);
    }
}