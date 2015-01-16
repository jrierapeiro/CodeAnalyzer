using System.Collections.Generic;

namespace CodeAnalyzer.Core.Models
{
    public class NamespaceModel
    {
        public string Name { get; set; }
        public List<NamedTypeModel> NamedTypes { get; set; }
    }
}