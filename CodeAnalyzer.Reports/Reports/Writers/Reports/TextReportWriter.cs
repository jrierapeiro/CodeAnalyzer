using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Reports.Reports.Writers.Interfaces;

namespace CodeAnalyzer.Reports.Reports.Writers.Reports
{
    public class TextReportWriter : IReportWriter
    {
        private readonly StringBuilder _content;

        public TextReportWriter()
        {
            _content = new StringBuilder();
        }

        public string Write(string basePath, NamespaceModel namespaceModel, List<ProjectReport> projects, bool writeOnlyCodeWithWarnings = false)
        {
            AddNamespace(basePath, namespaceModel, writeOnlyCodeWithWarnings);

            return _content.ToString();
        }

        private void AddNamespace(string basePath, NamespaceModel currentNameSpace, bool writeOnlyCodeWithWarnings)
        {
            _content.AppendFormat("{0}", currentNameSpace);
            _content.AppendLine();
            foreach (var namedType in currentNameSpace.NamedTypes)
            {
                AddClassEntry(basePath, namedType, writeOnlyCodeWithWarnings);
            }
        }

        private void AddClassEntry(string basePath, NamedTypeModel namedTypeSymbol, bool writeOnlyCodeWithWarnings)
        {
            var writeClass = false;
            if (writeOnlyCodeWithWarnings)
            {
                if (namedTypeSymbol.Symbols.Any(n => n.ShouldHaveReferences && n.ReferencesCount == 0))
                {
                    writeClass = true;
                }
            }
            else
            {
                writeClass = true;
            }

            if (writeClass)
            {
                _content.AppendFormat("---- {2} {0}.{1}", namedTypeSymbol.ContainingNamespace, namedTypeSymbol.Name, namedTypeSymbol.GetType().Name);
                _content.AppendLine();
                foreach (var symbol in namedTypeSymbol.Symbols)
                {
                    if (writeOnlyCodeWithWarnings)
                    {
                        if (symbol.ShouldHaveReferences && symbol.ReferencesCount == 0)
                        {
                            AddMemberEntry(basePath, symbol);
                        }
                    }
                    else
                    {
                        AddMemberEntry(basePath, symbol);
                    }
                }
            }
        }

        private void AddMemberEntry(string basePath, SymbolModel member)
        {
            _content.AppendFormat("---- ---- {0} {1} {2} has {3} references:", member.GetType().Name, member.DeclaredAccessibility, member.Name, member.ReferencesCount);
            _content.AppendLine();
            foreach (var referenceLocation in member.ReferenceLocations)
            {
                AddLocationEntry(basePath, referenceLocation);
            }
        }

        private void AddLocationEntry(string basePath, ReferenceLocationModel location)
        {
            _content.AppendFormat("---- ---- ----  {0}", location.FilePath.Replace(basePath, String.Empty));
            _content.AppendLine();
        }
    }
}