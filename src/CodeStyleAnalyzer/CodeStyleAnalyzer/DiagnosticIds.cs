namespace CodeStyleAnalyzer
{
    // Id's are based on "<prefix><stylerulenumber><subrulenumber>
    public static class DiagnosticIds
    {
        public static string CurlybracesShouldBeginOnNewLine { get; } = CreateDiagnosticId(1, 1);

        public static string UseConsistentIndentation { get; } = CreateDiagnosticId(2, 1);

        public static string PrivateFieldPrefix { get; } = CreateDiagnosticId(3, 1);

        public static string PrivateFieldCanBeReadonly { get; } = CreateDiagnosticId(3, 2);

        public static string ReadOnlyKeywordShouldBeAfterStatic { get; } = CreateDiagnosticId(3, 3);

        public static string AvoidThis { get; } = CreateDiagnosticId(4, 1);

        public static string SpecifyVisibility { get; } = CreateDiagnosticId(5, 1);

        public static string VisibilityModifierShouldBeFirstModifier { get; } = CreateDiagnosticId(5, 2);

        public static string UsingShouldBeOutsideNamespace { get; } = CreateDiagnosticId(6, 1);

        public static string AvoidMultipleBlankLines { get; } = CreateDiagnosticId(6, 1);

        public static string AvoidSpuriousWhitespace { get; } = CreateDiagnosticId(6, 1);

        public static string UseExistingStyle { get; } = CreateDiagnosticId(6, 1); // Can't be implemented

        public static string OnlyUseVarWhenObvious { get; } = CreateDiagnosticId(6, 1); // Can't be implemented

        public static string UseLanguageKeywords { get; } = CreateDiagnosticId(6, 1);

        public static string UsePascalCasingForConstantLocalVariablesAndFields { get; } = CreateDiagnosticId(6, 1);

        public static string UseNameOfOperator { get; } = CreateDiagnosticId(6, 1);

        private static string CreateDiagnosticId(int ruleId, int subRuleId)
        {
            const string Prefix = "CSA";
            return $"{Prefix}{ruleId:00}{subRuleId:00}";
        }
    }
}
