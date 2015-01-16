using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeAnalyzer.Reports.Reports.Writers.Diagnostics;
using CodeAnalyzer.Reports.Reports.Writers.Index;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;
using CodeAnalyzer.Reports.Reports.Writers.Reports;

namespace CodeAnalyzer.Reports.Reports.Writers
{
    public class SolutionReportWritter
    {
        public static void WriteOutput(ReportType reportType, string solutionFile, List<ProjectReport> projectOutputs, string outputPath, bool writeOnlyCodeWithWarnings = false)
        {
            //Write index
            var indexWriter = GetIndexReportWriter(reportType);
            var indexOutput = indexWriter.Write(Path.GetFileName(solutionFile), projectOutputs);
            File.WriteAllText(Path.Combine(outputPath, String.Format("Index.{0}", GetExtension(reportType))), indexOutput);


            foreach (var projectOutput in projectOutputs)
            {
                File.WriteAllText(Path.Combine(outputPath, String.Format(@"{0}_Report.{1}", projectOutput.Project.AssemblyName, GetExtension(reportType))),
                    projectOutput.ReferencesReport.ToString(reportType, projectOutputs, writeOnlyCodeWithWarnings));

                if (Enumerable.Any(projectOutput.Diagnostics))
                {
                    //Write Diagnostics
                    var diagnosticReportWriter = GetDiagnosticReportWriter(reportType);
                    var write = diagnosticReportWriter.Write(projectOutput.Project.AssemblyName, projectOutput.Diagnostics.ToList(), projectOutputs);
                    File.WriteAllText(Path.Combine(outputPath, String.Format("{1}_Diagnostics.{0}", GetExtension(reportType), projectOutput.Project.AssemblyName)), write);
                }
            }
        }

        public static string GetExtension(ReportType reportType)
        {
            switch (reportType)
            {
                case ReportType.PlainText:
                    return "txt";


                case ReportType.Html:
                    return "html";


                default:
                    throw new ArgumentOutOfRangeException("reportType");
            }
        }

        public static IReportWriter GetReportWriter(ReportType reportType)
        {
            IReportWriter reportWriter;
            switch (reportType)
            {
                case ReportType.PlainText:
                    reportWriter = new TextReportWriter();

                    break;
                case ReportType.Html:
                    reportWriter = new HtmlReportWriter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reportType");
            }
            return reportWriter;
        }

        public static IIndexReportWriter GetIndexReportWriter(ReportType reportType)
        {
            IIndexReportWriter reportWriter;
            switch (reportType)
            {
                case ReportType.PlainText:
                    reportWriter = new IndexTextReportWriter();

                    break;
                case ReportType.Html:
                    reportWriter = new IndexHtmlReportWriter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reportType");
            }
            return reportWriter;
        }

        public static IDiagnosticReportWriter GetDiagnosticReportWriter(ReportType reportType)
        {
            IDiagnosticReportWriter reportWriter;
            switch (reportType)
            {
                case ReportType.Html:
                    reportWriter = new DiagnosticHtmlReportWriter();
                    break;
                case ReportType.PlainText:
                // reportWriter = new IndexTextReportWriter();

                //break;

                default:
                    throw new ArgumentOutOfRangeException("reportType");
            }
            return reportWriter;
        }
    }
}