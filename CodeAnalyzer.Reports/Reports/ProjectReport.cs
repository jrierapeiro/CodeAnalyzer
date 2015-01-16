using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzer.Analyzers.References;
using CodeAnalyzer.Reports.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzer.Reports.Reports
{
    public class ProjectReport
    {
        public ProjectReport(Project project, ReferencesReport referencesReport, ImmutableArray<Diagnostic> diagnostics)
        {
            ReferencesReport = referencesReport;
            Diagnostics = diagnostics;
            Project = project;
        }

        public Project Project { get; private set; }
        public ReferencesReport ReferencesReport { get; private set; }
        public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

        public static async Task<ProjectReport> GenerateReport(Project project, string basePath, Solution solution, bool writeOnlyCodeWithWarnings, ImmutableArray<DiagnosticAnalyzer> diagnosticAnalyzers, ImmutableArray<ReferencesDiagnosticAnalyzer> referencesAnalyzers)
        {
            ProjectReport projectReport = null;
            var compilation = await project.GetCompilationAsync();
            var subNamespaces = project.AssemblyName.Split('.');
            var currentNameSpace = compilation.GlobalNamespace;
            foreach (var subNamespace in subNamespaces)
            {
                currentNameSpace = currentNameSpace.GetNamespaceMembers().FirstOrDefault(n => n.Name == subNamespace);
                if (currentNameSpace == null)
                {
                    Console.WriteLine("Project {0} not found in compilation", project.AssemblyName);
                    break;
                }
            }

            if (currentNameSpace != null)
            {
                var report = new ReferencesReport(basePath, solution, currentNameSpace, writeOnlyCodeWithWarnings, referencesAnalyzers);
                var diagnostics = await compilation.GetDiagnosticsAsync(diagnosticAnalyzers, null);
                projectReport = new ProjectReport(project, report, diagnostics);
                Console.WriteLine("Project {0} processed", project.AssemblyName);
            }
            return projectReport;
        }
    }
}