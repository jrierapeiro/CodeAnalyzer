using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Core.Models
{
    public class SymbolModel
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public Accessibility DeclaredAccessibility { get; set; }
        public List<ReferenceLocationModel> ReferenceLocations { get; set; }
        public int ReferencesCount { get; set; }
        public bool ShouldHaveReferences { get; set; } = true;
        public List<string> Diagnostics { get; } = new List<string>();
    }
}