using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStyleAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VisibilityModifierAnalyzer : DiagnosticAnalyzer
    {
        private static SyntaxKind[] s_typeDeclarations =
        {
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.DelegateDeclaration,
        };

        private static SyntaxKind[] s_memberDeclarations =
        {
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.EventFieldDeclaration,
            SyntaxKind.FieldDeclaration,
        };

        private static SyntaxKind[] s_visibilityKeywords = { SyntaxKind.InternalKeyword, SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword };

        private static readonly LocalizableString s_visibilityModifierTitle = new LocalizableResourceString(nameof(Resources.VisibilityModifierTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_visibilityModifierMessageFormat = new LocalizableResourceString(nameof(Resources.VisibilityModifierMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_visibilityModifierDescription = new LocalizableResourceString(nameof(Resources.VisibilityModifierDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor s_visibilityModifierRule = new DiagnosticDescriptor(DiagnosticIds.SpecifyVisibility, s_visibilityModifierTitle, s_visibilityModifierMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, s_visibilityModifierDescription);

        private static readonly LocalizableString s_visibilityModifierPositionTitle = new LocalizableResourceString(nameof(Resources.VisibilityModifierPositionTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_visibilityModifierPositionMessageFormat = new LocalizableResourceString(nameof(Resources.VisibilityModifierPositionMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_visibilityModifierPositionDescription = new LocalizableResourceString(nameof(Resources.VisibilityModifierPositionDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor s_visibilityModifierPositionRule = new DiagnosticDescriptor(DiagnosticIds.VisibilityModifierShouldBeFirstModifier, s_visibilityModifierPositionTitle, s_visibilityModifierPositionMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, s_visibilityModifierPositionDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(s_visibilityModifierRule, s_visibilityModifierPositionRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CheckVisibility);
        }

        private static void CheckVisibility(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(checkVisibility, ImmutableArray.Create(s_typeDeclarations));
            context.RegisterSyntaxNodeAction(checkMemberVisibility, ImmutableArray.Create(s_memberDeclarations));
        }

        private static void checkVisibility(SyntaxNodeAnalysisContext context)
        {
            var foundVisibilityToken = context.Node.ChildTokens().FirstOrDefault(HasVisibilityKeyword);

            if (foundVisibilityToken.IsKind(SyntaxKind.None))
            {
                context.ReportDiagnostic(Diagnostic.Create(s_visibilityModifierRule, context.Node.GetLocation()));
            }
            else if (context.Node.GetFirstToken() != foundVisibilityToken && foundVisibilityToken.GetPreviousToken().Kind() != SyntaxKind.CloseBracketToken)
            {
                context.ReportDiagnostic(Diagnostic.Create(s_visibilityModifierPositionRule, foundVisibilityToken.GetLocation()));
            }
        }

        private static void checkMemberVisibility(SyntaxNodeAnalysisContext context)
        {
            if (!ShouldHaveExplicitVisibility(context.Node))
            {
                return;
            }

            checkVisibility(context);
        }

        private static bool ShouldHaveExplicitVisibility(SyntaxNode node)
        {
            if (node.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
            {
                return false;
            }

            if (node.ChildNodes().OfType<ExplicitInterfaceSpecifierSyntax>().Any())
            {
                return false;
            }

            if (node.IsKind(SyntaxKind.ConstructorDeclaration) && node.ChildNodesAndTokens().Any(x => x.IsKind(SyntaxKind.StaticKeyword)))
            {
                return false;
            }

            if (node.IsKind(SyntaxKind.MethodDeclaration) && (node as MethodDeclarationSyntax).Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return false;
            }

            return true;
        }

        private static bool HasVisibilityKeyword(SyntaxToken token)
        {
            return s_visibilityKeywords.Any(x => token.IsKind(x));
        }
    }
}