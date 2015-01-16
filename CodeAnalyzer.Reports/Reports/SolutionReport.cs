using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.Analyzers.Diagnostics;
using CodeAnalyzer.Analyzers.References;
using CodeAnalyzer.Reports.Reports.Writers;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeAnalyzer.Reports.Reports
{
    public class SolutionReport
    {
        public static async Task AnalizeSolution(string solutionFile, ReportType reportType, string outputPath, bool writeOnlyCodeWithWarnings = false)
        {
            CheckOutputPath(outputPath);

            var basePath = Path.GetDirectoryName(solutionFile);
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionFile);


            var diagnosticAnalyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VariableEndedWithIdNamedCorrectlyDiagnosticAnalyzer(),
                new OldTermDiagnosticAnalyzer(), new AsyncMethodNotNamedCorrectlyDiagnosticAnalyzer());
            var referencesAnalyzers = ImmutableArray.Create<ReferencesDiagnosticAnalyzer>(new ControllerActionsReferencesDiagnosticAnalyzer());


            Console.WriteLine("References report");

            var projectOutputs = new List<ProjectReport>();

            foreach (var project in solution.Projects)
            {
                var projectReport = await ProjectReport.GenerateReport(project, basePath, solution, writeOnlyCodeWithWarnings, diagnosticAnalyzers, referencesAnalyzers);
                if (projectReport != null && (!projectReport.ReferencesReport.IsEmpty || Enumerable.Any(projectReport.Diagnostics)))
                {
                    projectOutputs.Add(projectReport);
                }
            }
            if (projectOutputs.Any())
            {
                SolutionReportWritter.WriteOutput(reportType, solutionFile, projectOutputs, outputPath, writeOnlyCodeWithWarnings);
            }
        }

        private static void CheckOutputPath(string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
        }
    }
}