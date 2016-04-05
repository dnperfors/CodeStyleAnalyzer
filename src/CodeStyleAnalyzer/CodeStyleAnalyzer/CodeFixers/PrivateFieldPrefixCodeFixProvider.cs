using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace CodeStyleAnalyzer.CodeFixers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PrivateFieldPrefixCodeFixProvider)), Shared]
    public class PrivateFieldPrefixCodeFixProvider : CodeFixProvider
    {
        private const string title = "Prefix field";
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.PrivateFieldPrefix); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().First();
            var prefix = GetPrefix(declaration.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First());
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => PrefixFieldAsync(context.Document, declaration, prefix, c),
                    equivalenceKey: title),
                diagnostic);
        }
        private string GetPrefix(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.ChildTokens().Any(x => x.IsKind(SyntaxKind.StaticKeyword)))
            {
                if(fieldDeclaration.AttributeLists != null && fieldDeclaration.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.GetFirstToken().ValueText == "ThreadStatic")))
                {
                    return "t_";
                }
                return "s_";
            }
            return "_";
        }

        private async Task<Solution> PrefixFieldAsync(Document document, VariableDeclaratorSyntax variableDeclaration, string prefix, CancellationToken cancellationToken)
        {
            var identifierToken = variableDeclaration.Identifier;
            var newName = prefix + identifierToken.Text;

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(variableDeclaration, cancellationToken);

            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
