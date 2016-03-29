using CodeStyleAnalyzer.Analyzers;
using CodeStyleAnalyzer.CodeFixer;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace CodeStyleAnalyzer.Test.Analyzers
{
    public class TestFieldNameAnalyzer : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FieldNameAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FieldNameCodeFixProvider();
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "CSA0003",
                Message = "Field 'testField' is not prefixed with '_'",
                Severity = Microsoft.CodeAnalysis.DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 20) }
            };

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

            var expected = new DiagnosticResult
            {
                Id = "CSA0003",
                Message = "Field 'testField' is not prefixed with 's_'",
                Severity = Microsoft.CodeAnalysis.DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 27) }
            };

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

            var expected = new DiagnosticResult
            {
                Id = "CSA0003",
                Message = "Field 'testField' is not prefixed with 't_'",
                Severity = Microsoft.CodeAnalysis.DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 27) }
            };

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
