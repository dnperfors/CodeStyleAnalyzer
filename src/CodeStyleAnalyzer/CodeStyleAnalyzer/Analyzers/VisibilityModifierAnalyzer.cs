using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        private static readonly LocalizableString VisibilityModifierTitle = new LocalizableResourceString(nameof(Resources.VisibilityModifierTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString VisibilityModifierMessageFormat = new LocalizableResourceString(nameof(Resources.VisibilityModifierMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString VisibilityModifierDescription = new LocalizableResourceString(nameof(Resources.VisibilityModifierDescription), Resources.ResourceManager, typeof(Resources));
        private static DiagnosticDescriptor VisibilityModifierRule = new DiagnosticDescriptor(DiagnosticIds.SpecifyVisibility, VisibilityModifierTitle, VisibilityModifierMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, VisibilityModifierDescription);

        private static readonly LocalizableString VisibilityModifierPositionTitle = new LocalizableResourceString(nameof(Resources.VisibilityModifierPositionTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString VisibilityModifierPositionMessageFormat = new LocalizableResourceString(nameof(Resources.VisibilityModifierPositionMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString VisibilityModifierPositionDescription = new LocalizableResourceString(nameof(Resources.VisibilityModifierPositionDescription), Resources.ResourceManager, typeof(Resources));
        private static DiagnosticDescriptor VisibilityModifierPositionRule = new DiagnosticDescriptor(DiagnosticIds.VisibilityModifierShouldBeFirstModifier, VisibilityModifierPositionTitle, VisibilityModifierPositionMessageFormat, Categories.StyleGuide, DiagnosticSeverity.Warning, true, VisibilityModifierPositionDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(VisibilityModifierRule, VisibilityModifierPositionRule); } }

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
                context.ReportDiagnostic(Diagnostic.Create(VisibilityModifierRule, context.Node.GetLocation()));
            }
            else
            {
                if (context.Node.GetFirstToken() != foundVisibilityToken && foundVisibilityToken.GetPreviousToken().Kind() != SyntaxKind.CloseBracketToken)
                {
                    context.ReportDiagnostic(Diagnostic.Create(VisibilityModifierPositionRule, foundVisibilityToken.GetLocation()));
                }
            }
        }

        private static void checkMemberVisibility(SyntaxNodeAnalysisContext context)
        {
            if (!ShouldHaveExplicitVisibility(context.Node)) return;
            checkVisibility(context);
        }

        private static bool ShouldHaveExplicitVisibility(SyntaxNode node)
        {
            if (node.Parent.IsKind(SyntaxKind.InterfaceDeclaration)) return false;
            if (node.ChildNodes().OfType<ExplicitInterfaceSpecifierSyntax>().Any()) return false;
            if (node.IsKind(SyntaxKind.ConstructorDeclaration) && node.ChildNodesAndTokens().Any(x => x.IsKind(SyntaxKind.StaticKeyword))) return false;
            return true;
        }

        private static bool HasVisibilityKeyword(SyntaxToken token)
        {
            return s_visibilityKeywords.Any(x => token.IsKind(x));
        }
    }
}
