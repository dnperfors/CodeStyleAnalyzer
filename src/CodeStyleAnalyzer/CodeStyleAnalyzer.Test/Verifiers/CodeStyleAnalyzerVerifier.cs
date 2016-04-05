using TestHelper;

namespace CodeStyleAnalyzer.Test.Verifiers
{
    public abstract class CodeStyleAnalyzerVerifier : CodeFixVerifier
    {
        protected abstract string CodeRuleId { get; }
        protected abstract string CodeRuleMessage { get; }

        protected DiagnosticResult GetDiagnosticResult(int line, int column, params string[] args)
        {
            return new DiagnosticResult
            {
                Id = CodeRuleId,
                Message = string.Format(CodeRuleMessage, args),
                Severity = Microsoft.CodeAnalysis.DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) }
            };
        }
    }
}
