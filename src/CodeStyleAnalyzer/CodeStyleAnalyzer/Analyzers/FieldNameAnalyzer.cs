using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeStyleAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldNameAnalyzer : DiagnosticAnalyzer
    {
        public const string InstanceFieldDiagnosticId = "CSA0001";
        public const string StaticFieldDiagnosticId = "CSA0002";
        public const string ThreadStaticFieldDiagnosticId = "CSA0003";

        private static readonly LocalizableString InstanceFieldTitle = new LocalizableResourceString(nameof(Resources.FieldNameInstanceFieldTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString InstanceFieldMessageFormat = new LocalizableResourceString(nameof(Resources.FieldNameInstanceFieldMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString InstanceFieldDescription = new LocalizableResourceString(nameof(Resources.FieldNameInstanceFieldDescripition), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString StaticFieldTitle = new LocalizableResourceString(nameof(Resources.FieldNameStaticFieldTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString StaticFieldMessageFormat = new LocalizableResourceString(nameof(Resources.FieldNameStaticFieldMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString StaticFieldDescription = new LocalizableResourceString(nameof(Resources.FieldNameStaticFieldDescripition), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString ThreadStaticFieldTitle = new LocalizableResourceString(nameof(Resources.FieldNameThreadStaticFieldTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString ThreadStaticFieldMessageFormat = new LocalizableResourceString(nameof(Resources.FieldNameThreadStaticFieldMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString ThreadStaticFieldDescription = new LocalizableResourceString(nameof(Resources.FieldNameThreadStaticFieldDescripition), Resources.ResourceManager, typeof(Resources));

        private const string Category = "Naming";

        private static DiagnosticDescriptor InstanceFieldRule = new DiagnosticDescriptor(InstanceFieldDiagnosticId, InstanceFieldTitle, InstanceFieldMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: InstanceFieldDescription);
        private static DiagnosticDescriptor StaticFieldRule = new DiagnosticDescriptor(StaticFieldDiagnosticId, StaticFieldTitle, StaticFieldMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: StaticFieldDescription);
        private static DiagnosticDescriptor ThreadStaticFieldRule = new DiagnosticDescriptor(ThreadStaticFieldDiagnosticId, ThreadStaticFieldTitle, ThreadStaticFieldMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: ThreadStaticFieldDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(InstanceFieldRule, StaticFieldRule, ThreadStaticFieldRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var fieldSymbol = (IFieldSymbol)context.Symbol;

            if (fieldSymbol.IsStatic)
            {
                if (fieldSymbol.GetAttributes().Any(x => x.AttributeClass.Name == "ThreadStatic"))
                {
                    if (!fieldSymbol.Name.StartsWith("t_"))
                    {
                        var diagnostic = Diagnostic.Create(ThreadStaticFieldRule, fieldSymbol.Locations[0], fieldSymbol.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (!fieldSymbol.Name.StartsWith("s_"))
                {
                    var diagnostic = Diagnostic.Create(StaticFieldRule, fieldSymbol.Locations[0], fieldSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
            else if (!fieldSymbol.Name.StartsWith("_"))
            {
                var diagnostic = Diagnostic.Create(InstanceFieldRule, fieldSymbol.Locations[0], fieldSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
