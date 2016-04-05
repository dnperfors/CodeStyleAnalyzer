namespace CodeStyleAnalyzer
{
    // Id's are based on "<prefix><stylerulenumber><subrulenumber>
    public static class DiagnosticIds
    {
        public static string CurlybracesShouldBeginOnNewLine = CreateDiagnosticId(1, 1);
        public static string UseConsistentIndentation = CreateDiagnosticId(2, 1);
        public static string PrivateFieldPrefix = CreateDiagnosticId(3, 1);
        public static string PrivateFieldCanBeReadonly = CreateDiagnosticId(3, 2);
        public static string ReadOnlyKeywordShouldBeAfterStatic = CreateDiagnosticId(3, 3);
        public static string AvoidThis =  CreateDiagnosticId(4, 1);
        public static string SpecifyVisibility = CreateDiagnosticId(5, 1);
        public static string VisibilityModifierShouldBeFirstModifier = CreateDiagnosticId(5, 2);
        public static string UsingShouldBeOutsideNamespace = CreateDiagnosticId(6, 1);
        public static string AvoidMultipleBlankLines = CreateDiagnosticId(6, 1);
        public static string AvoidSpuriousWhitespace = CreateDiagnosticId(6, 1);
        public static string UseExistingStyle = CreateDiagnosticId(6, 1); // Can't be implemented
        public static string OnlyUseVarWhenObvious = CreateDiagnosticId(6, 1); // Can't be implemented
        public static string UseLanguageKeywords = CreateDiagnosticId(6, 1);
        public static string UsePascalCasingForConstantLocalVariablesAndFields = CreateDiagnosticId(6, 1);
        public static string UseNameOfOperator = CreateDiagnosticId(6, 1);

        private static string CreateDiagnosticId(int ruleId, int subRuleId)
        {
            const string Prefix = "CSA";
            return $"{Prefix}{ruleId:00}{subRuleId:00}";
        }
    }
}
