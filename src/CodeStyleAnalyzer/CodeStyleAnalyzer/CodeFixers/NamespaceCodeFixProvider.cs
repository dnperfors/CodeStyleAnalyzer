using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using Microsoft.CodeAnalysis.Formatting;

namespace CodeStyleAnalyzer.CodeFixers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceCodeFixProvider)), Shared]
    public class NamespaceCodeFixProvider : CodeFixProvider
    {
        private const string title = "Move using outside namepspace";

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIds.UsingShouldBeOutsideNamespace); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            var root = await semanticModel.SyntaxTree.GetRootAsync(context.CancellationToken);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var usingDirective = root.FindNode(diagnosticSpan) as UsingDirectiveSyntax;

            context.RegisterCodeFix(CodeAction.Create(title, c => MoveUsing(context.Document, usingDirective, context.CancellationToken), title), diagnostic);
        }

        private async Task<Document> MoveUsing(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var syntaxRoot = (CompilationUnitSyntax)syntaxTree.GetRoot();
            var newNamespaceDeclaration = usingDirective.Parent.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia);

            SyntaxNode newSyntaxRoot = syntaxRoot
                .ReplaceNode(usingDirective.Parent, newNamespaceDeclaration)
                .AddUsings(usingDirective);
            newSyntaxRoot = Formatter.Format(newSyntaxRoot, document.Project.Solution.Workspace);

            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);
            return newDocument;
        }
    }
}
