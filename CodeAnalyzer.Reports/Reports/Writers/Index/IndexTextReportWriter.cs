using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;

namespace CodeAnalyzer.Reports.Reports.Writers.Index
{
    public class IndexTextReportWriter : IIndexReportWriter
    {
        private StringBuilder _content;

        public IndexTextReportWriter()
        {
            _content = new StringBuilder();
        }

        public string Write(string solutionFile, List<ProjectReport> projects)
        {
            _content.AppendLine(String.Format("Index for {0}", solutionFile));
            foreach (var project in projects)
            {
                _content.AppendLine(String.Format("{0}_Report.txt", project.Project.AssemblyName));
                if (project.Diagnostics.Any())
                {
                    _content.AppendLine(String.Format("{0}_Diagnostics.txt", project.Project.AssemblyName));
                }
            }
            return _content.ToString();
        }

        public void Dispose()
        {
            _content = null;
        }
    }
}