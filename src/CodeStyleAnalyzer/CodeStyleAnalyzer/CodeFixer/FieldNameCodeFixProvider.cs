using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeStyleAnalyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeStyleAnalyzer.CodeFixer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldNameCodeFixProvider)), Shared]
    public class FieldNameCodeFixProvider : CodeFixProvider
    {
        private const string title = "Prefix field";
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(FieldNameAnalyzer.DiagnosticId); }
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
            // Compute new uppercase name.
            var identifierToken = variableDeclaration.Identifier;
            var newName = prefix + identifierToken.Text;

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(variableDeclaration, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }
    }
}
