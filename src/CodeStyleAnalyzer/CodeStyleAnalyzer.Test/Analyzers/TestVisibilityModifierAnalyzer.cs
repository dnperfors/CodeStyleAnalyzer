using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;
using CodeStyleAnalyzer.Analyzers;
using Microsoft.CodeAnalysis;

namespace CodeStyleAnalyzer.Test.Analyzers
{
    public class TestVisibilityModifierAnalyzer : DiagnosticVerifier
    {
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
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 5) }
                },
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 5) }
                },
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 5) }
                },
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 5) }
                },
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 9) }
                },
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
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 9) }
                },
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 9) }
                },
                new DiagnosticResult
                {
                    Id = "CSA0005",
                    Message = "Visibility modifier should be specified.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 9) }
                },
            };

            VerifyCSharpDiagnostic(source, expectedResults);
        }
    }
}
