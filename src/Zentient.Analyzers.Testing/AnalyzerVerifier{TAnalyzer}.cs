// <copyright file="src/Zentient.Analyzers.Testing/AnalyzerVerifier{TAnalyzer}.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// MIT License. See LICENSE in the project root for license information.
// </copyright>

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;

namespace Zentient.Analyzers.Testing
{
    public static class AnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
            {
                TestCode = source,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            };

            // Add a minimal Abstractions with the attribute so the analyzer can bind
            test.TestState.Sources.Add("namespace Zentient.Abstractions { public sealed class ZentientStubAttribute : System.Attribute { public ZentientStubAttribute(System.Type t) {} } }");

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync();
        }
    }
}
