using System.Collections.Generic;

namespace CodeAnalyzer.Core.Models
{
    public class NamedTypeModel
    {
        public string Name { get; set; }
        public string ContainingNamespace { get; set; }
        public List<SymbolModel> Symbols { get; set; }
        public int InstanceConstructorReferencesCount { get; set; }
    }
}