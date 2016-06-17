using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeStyleAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamespaceAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString NamespacePositionTitle = new LocalizableResourceString(nameof(Resources.NamespacePositionTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString NamespacePositionMessageFormat = new LocalizableResourceString(nameof(Resources.NamespacePositionMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString NamespacePositionDescription = new LocalizableResourceString(nameof(Resources.NamespacePositionDescripition), Resources.ResourceManager, typeof(Resources));

        private static DiagnosticDescriptor NamespacePositionRule = new DiagnosticDescriptor(DiagnosticIds.UsingShouldBeOutsideNamespace, NamespacePositionTitle, NamespacePositionMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, NamespacePositionDescription);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(NamespacePositionRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ValidateNamespacePosition, SyntaxKind.UsingDirective);
        }

        private void ValidateNamespacePosition(SyntaxNodeAnalysisContext context)
        {
            if(context.Node.Ancestors().Any(x => x.IsKind(SyntaxKind.NamespaceDeclaration)))
            {
                context.ReportDiagnostic(Diagnostic.Create(NamespacePositionRule, context.Node.GetLocation()));
            }
        }
    }
}
