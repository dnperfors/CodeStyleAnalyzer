using CodeStyleAnalyzer.Analyzers;
using CodeStyleAnalyzer.Test.Verifiers;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace CodeStyleAnalyzer.Test.Analyzers
{
    public class TestCSA0501 : CodeStyleAnalyzerVerifier
    {
        protected override string CodeRuleId { get; } = "CSA0501";
        protected override string CodeRuleMessage { get; } = "Visibility modifier should be specified.";

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new VisibilityModifierAnalyzer();
        }

        [Fact]
        public void TypesWithoutVisibilityModifier_ShouldReturnMessage()
        {
            var source = @"
namespace Test
{
    class TestClass { }
    struct TestStruct { }
    interface ITestInterface { }
    enum TestEnum { }
    public TestWithInnerClass
    {
        class InnerClass { }
    }
}";
            var expectedResults = new[]
            {
                GetDiagnosticResult(4, 5),
                GetDiagnosticResult(5, 5),
                GetDiagnosticResult(6, 5),
                GetDiagnosticResult(7, 5),
                GetDiagnosticResult(10, 9),
            };

            VerifyCSharpDiagnostic(source, expectedResults);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected")]
        [InlineData("protected internal")]
        public void ClassWithVisibilityModifier_ShouldNotReturnMessage(string modifier)
        {
            var source = $"namespace Test {{ {modifier} class TestClass {{ }} }}";

            VerifyCSharpDiagnostic(source);
        }

        [Fact]
        public void MembersWitoutVisibilityModifier_ShouldReturnMessage()
        {
            var source = @"
namespace Test
{
    public class TestClass
    {
        void TestMethod() { }
        string _testField;
        string TestProperty { get; set; }
    }
}";

            var expectedResults = new[]
            {
                GetDiagnosticResult(6, 9),
                GetDiagnosticResult(7, 9),
                GetDiagnosticResult(8, 9),
            };

            VerifyCSharpDiagnostic(source, expectedResults);
        }
    }
}
