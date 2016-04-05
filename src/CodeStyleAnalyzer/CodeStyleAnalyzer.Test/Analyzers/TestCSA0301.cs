using CodeStyleAnalyzer.Analyzers;
using CodeStyleAnalyzer.CodeFixers;
using CodeStyleAnalyzer.Test.Verifiers;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace CodeStyleAnalyzer.Test.Analyzers
{
    public class TestCSA0301 : CodeStyleAnalyzerVerifier
    {
        protected override string CodeRuleId { get; } = "CSA0301";
        protected override string CodeRuleMessage { get; } = "Field '{0}' is not prefixed with '{1}'";

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new PrivateFieldPrefixAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new PrivateFieldPrefixCodeFixProvider();
        }

        [Fact]
        public void PrivateField_WithoutPrefix_ShouldReturnCorrectMessage()
        {
            var source = @"
    namespace Test
    {
        class TestClass
        {
            string testField;
        }
    }";

            var expectedDiagnostic = GetDiagnosticResult(6, 20, "testField", "_");

            var expectedSource = @"
    namespace Test
    {
        class TestClass
        {
            string _testField;
        }
    }";

            VerifyCSharpDiagnostic(source, expectedDiagnostic);
            VerifyCSharpFix(source, expectedSource);
        }

        [Fact]
        public void PrivateStaticField_WithoutPrefix_ShouldReturnCorrectMessage()
        {
            var source = @"
    namespace Test
    {
        class TestClass
        {
            static string testField;
        }
    }";

            var expected = GetDiagnosticResult(6, 27, "testField", "s_");

            var expectedSource = @"
    namespace Test
    {
        class TestClass
        {
            static string s_testField;
        }
    }";
            VerifyCSharpDiagnostic(source, expected);
            VerifyCSharpFix(source, expectedSource);
        }

        [Fact]
        public void PrivateThreadStaticField_WithoutPrefix_ShouldReturnCorrectMessage()
        {
            var source = @"
    namespace Test
    {
        class TestClass
        {
            [ThreadStatic]
            static string testField;
        }
    }";

            var expected = GetDiagnosticResult(7, 27, "testField", "t_");

            var expectedSource = @"
    namespace Test
    {
        class TestClass
        {
            [ThreadStatic]
            static string t_testField;
        }
    }";

            VerifyCSharpDiagnostic(source, expected);
            VerifyCSharpFix(source, expectedSource);
        }
    }
}
