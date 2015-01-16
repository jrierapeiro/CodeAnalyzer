using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CodeAnalyzer.Analyzers.References;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Reports.Reports.Writers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace CodeAnalyzer.Reports.Reports
{
    public class ReferencesReport
    {
        private NamespaceModel _namespaceModel;
        private readonly string _basePath;
        private readonly INamespaceSymbol _currentNameSpace;
        private readonly ImmutableArray<ReferencesDiagnosticAnalyzer> _referencesAnalyzers;
        private readonly Solution _solution;
        private readonly bool _writeOnlyCodeWithWarnings;

        public ReferencesReport(string basePath, Solution solution, INamespaceSymbol currentNameSpace, bool writeOnlyCodeWithWarnings, ImmutableArray<ReferencesDiagnosticAnalyzer> referencesAnalyzers)
        {
            _basePath = basePath;
            _solution = solution;
            _currentNameSpace = currentNameSpace;
            _writeOnlyCodeWithWarnings = writeOnlyCodeWithWarnings;
            _referencesAnalyzers = referencesAnalyzers;
            Generate();
        }

        public bool IsEmpty => _namespaceModel == null || !_namespaceModel.NamedTypes.Any();

        private void Generate()
        {
            _namespaceModel = new NamespaceModel
            {
                Name = _currentNameSpace.ToString(),
                NamedTypes = new List<NamedTypeModel>()
            };
            CollectTypesInNamespace(_currentNameSpace);
        }

        public string ToString(ReportType reportType, List<ProjectReport> projects, bool writeOnlyCodeWithWarnings = false)
        {
            var reportWriter = SolutionReportWritter.GetReportWriter(reportType);
            return reportWriter.Write(_basePath, _namespaceModel, projects, writeOnlyCodeWithWarnings);
        }

        private void CollectTypesInNamespace(INamespaceSymbol namespaceSymbol)
        {
            foreach (var member in namespaceSymbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Namespace)
                {
                    CollectTypesInNamespace((INamespaceSymbol) member);
                }
                else if (member.Kind == SymbolKind.NamedType)
                {
                    CollectTypesInType((INamedTypeSymbol) member);
                }
            }
        }

        private void CollectTypesInType(INamedTypeSymbol namedTypeSymbol)
        {
            GetReferencesFromTypeMembers(namedTypeSymbol);

            foreach (var childType in namedTypeSymbol.GetTypeMembers())
            {
                CollectTypesInType(childType);
            }
        }

        private void GetReferencesFromTypeMembers(INamedTypeSymbol namedTypeSymbol)
        {
            if (IsAnalyzable(namedTypeSymbol))
            {
                var namedType = new NamedTypeModel
                {
                    Name = namedTypeSymbol.OriginalDefinition.ToString(),
                    ContainingNamespace = namedTypeSymbol.ContainingNamespace.ToString(),
                    Symbols = new List<SymbolModel>(),
                    InstanceConstructorReferencesCount = 0
                };


                //Check all constructors
                foreach (var instanceConstructor in namedTypeSymbol.InstanceConstructors)
                {
                    var instanceConstructorReferences = SymbolFinder.FindReferencesAsync(instanceConstructor, _solution).Result.ToList();
                    namedType.InstanceConstructorReferencesCount += instanceConstructorReferences.Sum(ic => ic.Locations.Count());
                }


                foreach (var memberName in namedTypeSymbol.MemberNames)
                {
                    foreach (var member in namedTypeSymbol.GetMembers(memberName))
                    {
                        if (member != null)
                        {
                            if (IsAnalyzable(member))
                            {
                                var memberReferences = SymbolFinder.FindReferencesAsync(member, _solution).Result.ToList();
                                var symbol = new SymbolModel
                                {
                                    TypeName = member.GetType().Name,
                                    Name = member.ToString().Replace(namedType.Name + ".", String.Empty),
                                    DeclaredAccessibility = member.DeclaredAccessibility,
                                    ReferencesCount = memberReferences.Sum(mr => mr.Locations.Count()),
                                    ReferenceLocations = new List<ReferenceLocationModel>()
                                };


                                foreach (var referencesDiagnosticAnalyzer in _referencesAnalyzers.Where(referencesDiagnosticAnalyzer => referencesDiagnosticAnalyzer.IsApplicable(namedTypeSymbol)))
                                {
                                    referencesDiagnosticAnalyzer.AnalyzeNode(symbol, namedTypeSymbol, member);
                                }


                                foreach (var location in memberReferences.SelectMany(reference => reference.Locations))
                                {
                                    symbol.ReferenceLocations.Add(new ReferenceLocationModel
                                    {
                                        FilePath = location.Document.FilePath
                                    });
                                }

                                if (_writeOnlyCodeWithWarnings)
                                {
                                    if (symbol.ShouldHaveReferences && symbol.ReferencesCount == 0)
                                    {
                                        namedType.Symbols.Add(symbol);
                                    }
                                }
                                else
                                {
                                    namedType.Symbols.Add(symbol);
                                }
                            }
                        }
                    }
                }

                if (_writeOnlyCodeWithWarnings)
                {
                    if (namedType.Symbols.Any(n => n.ShouldHaveReferences && n.ReferencesCount == 0))
                    {
                        _namespaceModel.NamedTypes.Add(namedType);
                    }
                }
                else
                {
                    _namespaceModel.NamedTypes.Add(namedType);
                }
            }
        }

        private bool IsAnalyzable(INamedTypeSymbol classSymbol)
        {
            var attributes = classSymbol.GetAttributes();
            if (attributes.Any(a => a.AttributeClass.Name == "TestClassAttribute"))
            {
                return false;
            }


            return true;
        }

        private bool IsAnalyzable(ISymbol memberSymbol)
        {
            //Don't check empty constructor
            if (memberSymbol.GetType().Name == "SourceConstructorSymbol" && !Enumerable.Any(((IMethodSymbol) memberSymbol).Parameters))
            {
                //Check is empty constructor
                return false;
            }

            return true;
        }
    }
}