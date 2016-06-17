﻿using CodeStyleAnalyzer.Test.Verifiers;
using Xunit;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeStyleAnalyzer.Analyzers;

namespace CodeStyleAnalyzer.Test.Analyzers
{
    public class TestCSA0601 : CodeStyleAnalyzerVerifier
    {
        protected override string CodeRuleId { get; } = "CSA0601";

        protected override string CodeRuleMessage { get; } = "Namespace imports should be specified at the top of the file, outside of namespace declarations";

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new NamespaceAnalyzer();

        [Fact]
        public void CorrectFile_ShouldNotGiveError()
        {
            var source = @"
using System;
namespace Test
{
    internal class Test { }
}";
            VerifyCSharpDiagnostic(source);
        }

        [Fact]
        public void IncorrectFile_ShouldNotGiveError()
        {
            var source = @"
namespace Test
{
    using System;
    internal class Test { }
}";
            var expectedDiagnostic = GetDiagnosticResult(4, 5);
            VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }
    }
}
