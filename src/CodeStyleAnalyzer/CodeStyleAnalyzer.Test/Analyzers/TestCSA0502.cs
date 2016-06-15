using CodeStyleAnalyzer.Test.Verifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStyleAnalyzer.Analyzers;

namespace CodeStyleAnalyzer.Test.Analyzers
{
    public class TestCSA0502 : CodeStyleAnalyzerVerifier
    {
        protected override string CodeRuleId { get; } = "CSA0502";
        protected override string CodeRuleMessage { get; } = "Visibility modifier should be the first modifier.";

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new VisibilityModifierAnalyzer();
        }

        [Fact]
        public void VisibilityOperatorInTheWrongPlace_ShouldReturnCorrectMessage()
        {
            var source = @"
namespace Test
{
    abstract internal class Test { }
}
";
            var expectedDiagnostic = GetDiagnosticResult(4, 14);
            VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public void VisibilityOperatorInCorrectPlace_ShouldReturnCorrectMessage()
        {
            var source = @"
namespace Test
{
    internal abstract class Test { }
}
";
            VerifyCSharpDiagnostic(source);
        }
    }
}
