using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            var root = await semanticModel.SyntaxTree.GetRootAsync(context.CancellationToken);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;


            var declaration = root.FindToken(diagnosticSpan.Start).Parent;
            var fieldSymbol = semanticModel.GetDeclaredSymbol(declaration);
            var newName = GetNewFieldName(fieldSymbol);
            if (fieldSymbol.Name != newName)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedSolution: c => RenameFieldAsync(context.Document, fieldSymbol, newName, c),
                        equivalenceKey: title),
                    diagnostic);
            }
        }

        private string GetNewFieldName(ISymbol fieldSymbol)
        {
            var name = fieldSymbol.Name.Trim('_');
            if (name.Length > 2 && char.IsLetter(name[0]) && name[1] == '_')
            {
                name = name.Substring(2);
            }

            // Some .NET code uses "ts_" prefix for thread static
            if (name.Length > 3 && name.StartsWith("ts_", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(3);
            }

            if (name.Length == 0)
            {
                return fieldSymbol.Name;
            }

            if (name.Length > 2 && char.IsUpper(name[0]) && char.IsLower(name[1]))
            {
                name = char.ToLower(name[0]) + name.Substring(1);
            }

            if (fieldSymbol.IsStatic)
            {
                // Check for ThreadStatic private fields.
                if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass.Name.Equals("ThreadStaticAttribute", StringComparison.Ordinal)))
                {
                    return "t_" + name;
                }
                else
                {
                    return "s_" + name;
                }
            }

            return "_" + name;
        }

        private async Task<Solution> RenameFieldAsync(Document document, ISymbol fieldSymbol, string newName, CancellationToken cancellationToken)
        {
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, fieldSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
