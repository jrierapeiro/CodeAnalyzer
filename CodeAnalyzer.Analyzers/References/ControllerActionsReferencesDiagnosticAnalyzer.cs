using System.Linq;
using CodeAnalyzer.Core.Models;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Analyzers.References
{
    public class ControllerActionsReferencesDiagnosticAnalyzer : ReferencesDiagnosticAnalyzer
    {
        public const string DiagnosticId = "ControllerActionsReferencesDiagnosticAnalyzer";
        private const string Description = "The non-static public actions inside a controller shouldn't have references";

        public override bool IsApplicable(INamedTypeSymbol parentSymbol)
        {
            return (parentSymbol.AllInterfaces.AsEnumerable().Any(i => i.ContainingAssembly.Name == "System.Web.Mvc" && i.Name == "IController"));
        }

        public override void AnalyzeNode(SymbolModel model, INamedTypeSymbol parentSymbol, ISymbol member)
        {
            var method = member as IMethodSymbol;
            if (method != null && !method.ReturnsVoid)
            {
                if (method.DeclaredAccessibility == Accessibility.Public && !method.IsStatic)
                {
                    //if (method.ReturnType.Name == "ActionResult" || method.ReturnType.Name == "Task")
                    //{
                    model.ShouldHaveReferences = false;
                    model.Diagnostics.Add(Description);
                    //}
                }
            }
        }
    }
}