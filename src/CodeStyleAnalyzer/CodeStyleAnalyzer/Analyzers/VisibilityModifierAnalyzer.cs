using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CodeStyleAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VisibilityModifierAnalyzer : DiagnosticAnalyzer
    {
        private static SyntaxKind[] s_visibilityDeclarations = new[] { SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration };
        private static SyntaxKind[] s_visibilityKeywords = new[] { SyntaxKind.InternalKeyword, SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword };

        private static readonly LocalizableString VisibilityModifierTitle= new LocalizableResourceString(nameof(Resources.VisibilityModifierTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString VisibilityModifierMessageFormat = new LocalizableResourceString(nameof(Resources.VisibilityModifierMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString VisibilityModifierDescription = new LocalizableResourceString(nameof(Resources.VisibilityModifierDescription), Resources.ResourceManager, typeof(Resources));
        private static DiagnosticDescriptor VisibilityModifierRule = new DiagnosticDescriptor(DiagnosticIds.SpecifyVisibility, VisibilityModifierTitle, VisibilityModifierMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, VisibilityModifierDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(VisibilityModifierRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CheckVisibility);
        }

        private static void CheckVisibility(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckVisibility, ImmutableArray.Create(s_visibilityDeclarations));
        }

        private static void CheckVisibility(SyntaxNodeAnalysisContext context)
        {
            bool foundVisibilityToken = context.Node.ChildTokens().Any(HasVisibilityKeyword);

            if (!foundVisibilityToken)
            {
                context.ReportDiagnostic(Diagnostic.Create(VisibilityModifierRule, context.Node.GetLocation()));
            }
        }

        private static bool HasVisibilityKeyword(SyntaxToken token)
        {
            return s_visibilityKeywords.Any(x => token.IsKind(x));
        }
    }
}
