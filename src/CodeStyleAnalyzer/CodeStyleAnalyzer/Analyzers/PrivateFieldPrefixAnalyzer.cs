using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStyleAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PrivateFieldPrefixAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString s_privateFieldPrefixTitle = new LocalizableResourceString(nameof(Resources.PrivateFieldPrefixTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_privateFieldPrefixMessage = new LocalizableResourceString(nameof(Resources.PrivateFieldPrefixMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_privateFieldPrefixDescription = new LocalizableResourceString(nameof(Resources.PrivateFieldPrefixDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor s_privateFIeldPrefixRule = new DiagnosticDescriptor(DiagnosticIds.PrivateFieldPrefix, s_privateFieldPrefixTitle, s_privateFieldPrefixMessage, Categories.StyleGuide, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: s_privateFieldPrefixDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(s_privateFIeldPrefixRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var fieldSymbol = context.Symbol;
            if (!IsPrivateField((IFieldSymbol)fieldSymbol))
            {
                return;
            }
            var newFieldName = GetNewFieldName(fieldSymbol);
            if (newFieldName != fieldSymbol.Name)
            {
                var diagnostic = Diagnostic.Create(s_privateFIeldPrefixRule, fieldSymbol.Locations[0], fieldSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsPrivateField(IFieldSymbol fieldSymbol)
        {
            return fieldSymbol.DeclaredAccessibility == Accessibility.Private && !fieldSymbol.HasConstantValue;
        }

        private static string GetNewFieldName(ISymbol fieldSymbol)
        {
            var name = fieldSymbol.Name.Trim('_');
            if (name.Length > 2 && char.IsLetter(name[0]) && name[1] == '_')
            {
                name = name.Substring(2);
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
    }
}
