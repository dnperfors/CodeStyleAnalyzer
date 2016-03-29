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
        public static string DiagnosticId = "CSA0005";

        private static SyntaxKind[] s_visibilityDeclarations = new[] { SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration };
        private static SyntaxKind[] s_visibilityKeywords = new[] { SyntaxKind.InternalKeyword, SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword };

        private static DiagnosticDescriptor VisibilityModifierRule = new DiagnosticDescriptor(DiagnosticId, "Visibility modifier should be specified.", "Visibility modifier should be specified.", "Coding style", DiagnosticSeverity.Warning, true, "Visibility modifier should be specified.");

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
