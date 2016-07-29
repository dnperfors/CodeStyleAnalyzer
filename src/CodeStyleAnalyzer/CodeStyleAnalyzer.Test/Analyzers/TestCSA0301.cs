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
        protected override string CodeRuleMessage { get; } = "Field '{0}' is not prefixed correctly.";

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
using System;
class TestClass
{
    string privateField, x_, _, __, m_field;
    static string privateStaticField;
    [ThreadStatic] static string privateThreadStaticField;
}";

            var expectedDiagnostic = new[]
            {
                GetDiagnosticResult(5, 12, "privateField"),
                GetDiagnosticResult(5, 26, "x_"),
                GetDiagnosticResult(5, 37, "m_field"),
                GetDiagnosticResult(6, 19, "privateStaticField"),
                GetDiagnosticResult(7, 34, "privateThreadStaticField"),
            };

            var expectedSource = @"
using System;
class TestClass
{
    string _privateField, _x, _, __, _field;
    static string s_privateStaticField;
    [ThreadStatic] static string t_privateThreadStaticField;
}";

            VerifyCSharpDiagnostic(source, expectedDiagnostic);
            VerifyCSharpFix(source, expectedSource);
        }

        [Fact]
        public void PublicFields_WithoutPrefix_ShouldNotReturnDiagnostic()
        {
            var source = @"
using System;
class TestClass
{
    public string TestField;
    public readonly string ReadonlyTestField;
    public static string StaticTestField;
    [ThreadStatic]
    public static string ThreadStaticTestField;

    [ThreadStatic]
    [NonSerializable]
    private static string t_testField;

    private const string ConstField = ""Test"";
}";
            VerifyCSharpDiagnostic(source);
        }
    }
}
