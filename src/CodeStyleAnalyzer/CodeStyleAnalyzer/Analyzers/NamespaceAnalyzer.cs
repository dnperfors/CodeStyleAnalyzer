using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStyleAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamespaceAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString s_namespacePositionTitle = new LocalizableResourceString(nameof(Resources.NamespacePositionTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_namespacePositionMessageFormat = new LocalizableResourceString(nameof(Resources.NamespacePositionMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_namespacePositionDescription = new LocalizableResourceString(nameof(Resources.NamespacePositionDescripition), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor s_namespacePositionRule = new DiagnosticDescriptor(DiagnosticIds.UsingShouldBeOutsideNamespace, s_namespacePositionTitle, s_namespacePositionMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, s_namespacePositionDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(s_namespacePositionRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(ValidateNamespacePosition, SyntaxKind.UsingDirective);
        }

        private void ValidateNamespacePosition(SyntaxNodeAnalysisContext context)
        {
            if (!IsAlias(context) && HasNamespaceDeclarationAsParent(ref context))
            {
                context.ReportDiagnostic(Diagnostic.Create(s_namespacePositionRule, context.Node.GetLocation()));
            }
        }

        private static bool IsAlias(SyntaxNodeAnalysisContext context)
        {
            return ((UsingDirectiveSyntax)context.Node).Alias != null;
        }

        private static bool HasNamespaceDeclarationAsParent(ref SyntaxNodeAnalysisContext context)
        {
            return context.Node.Ancestors().Any(x => x.IsKind(SyntaxKind.NamespaceDeclaration));
        }
    }
}
