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
class TestClass { }
sealed partial class TestPartialClass { }
struct TestStruct { }
interface ITestInterface { }
enum TestEnum { }
delegate TestDelegate { }
";
            var expectedResults = new[]
            {
                GetDiagnosticResult(2, 1),
                GetDiagnosticResult(3, 1),
                GetDiagnosticResult(4, 1),
                GetDiagnosticResult(5, 1),
                GetDiagnosticResult(6, 1),
                GetDiagnosticResult(7, 1),
            };

            VerifyCSharpDiagnostic(source, expectedResults);
        }

        [Fact]
        public void NestedTypesWithoutVisibilityModifier_ShouldReturnMessage()
        {
            var source = @"
public TestWithInnerClass
{
    class TestClass { }
    sealed partial class TestPartialClass { }
    struct TestStruct { }
    interface ITestInterface { }
    enum TestEnum { }
    delegate TestDelegate { }
}";
            var expectedResults = new[]
            {
                GetDiagnosticResult(4, 5),
                GetDiagnosticResult(5, 5),
                GetDiagnosticResult(6, 5),
                GetDiagnosticResult(7, 5),
                GetDiagnosticResult(8, 5),
                GetDiagnosticResult(9, 5),
            };

            VerifyCSharpDiagnostic(source, expectedResults);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected")]
        [InlineData("protected internal")]
        public void TypesWithVisibilityModifier_ShouldNotReturnMessage(string modifier)
        {
            var source = $@"
{modifier} class TestClass {{ }}
{modifier} sealed partial class TestPartialClass {{ }}
{modifier} struct TestStruct {{ }}
{modifier} interface ITestInterface {{ }}
{modifier} enum TestEnum {{ }}
{modifier} delegate TestDelegate {{ }}
";

            VerifyCSharpDiagnostic(source);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected")]
        [InlineData("protected internal")]
        [InlineData("private")]
        public void NestedTypesWithVisibilityModifier_ShouldNotReturnMessage(string modifier)
        {
            var source = $@"
public TestWithInnerClass
{{
    {modifier} class TestClass {{ }}
    {modifier} sealed partial class TestPartialClass {{ }}
    {modifier} struct TestStruct {{ }}
    {modifier} interface ITestInterface {{ }}
    {modifier} enum TestEnum {{ }}
    {modifier} delegate TestDelegate {{ }}
}}";

            VerifyCSharpDiagnostic(source);
        }

        [Fact]
        public void MembersWithoutVisibilityModifier_ShouldReturnMessage()
        {
            var source = @"
public interface TestInterface
{
    void TestMethod();
    string TestProperty { get; set; }
    int this[int index] { get; set; }
    event EventHandler E;
}
public class TestClass : TestInterface
{
    static TestClass() { }
    TestClass() { }
    TestClass(string field) { }
    void TestMethod1() { }
    internal void TestMethod2() { }
    string TestProperty { get; set; }
    int this[int index] { get; set; }
    event EventHandler E;
    string testField;
}
public struct TestStruct
{
    void TestMethod1() { }
    internal void TestMethod2() { }
    string TestProperty { get; set; }
    int this[int index] { get; set; }
    event EventHandler E;
    string testField;
}
";

            var expectedResults = new[]
            {
                GetDiagnosticResult(12, 5),
                GetDiagnosticResult(13, 5),
                GetDiagnosticResult(14, 5),
                GetDiagnosticResult(16, 5),
                GetDiagnosticResult(17, 5),
                GetDiagnosticResult(18, 5),
                GetDiagnosticResult(19, 5),
                GetDiagnosticResult(23, 5),
                GetDiagnosticResult(25, 5),
                GetDiagnosticResult(26, 5),
                GetDiagnosticResult(27, 5),
                GetDiagnosticResult(28, 5),
            };

            VerifyCSharpDiagnostic(source, expectedResults);
        }

        [Fact]
        public void ExplicitMembersWithoutVisibilityModifier_ShouldReturnMessage()
        {
            var source = @"
public interface TestInterface
{
    void TestMethod();
    string TestProperty { get; set; }
    int this[int index] { get; set; }
    event EventHandler E;
}
public class TestClass : TestInterface
{
    void TestInterface.TestMethod();
    string TestInterface.TestProperty { get; set; }
    int TestInterface.this[int index] { get; set; }
    event EventHandler TestInterface.E;
}";

            VerifyCSharpDiagnostic(source);
        }
    }
}
