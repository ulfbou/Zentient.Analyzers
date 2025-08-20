// <copyright file="ZNT0001ResultComplianceTests.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Zentient.Analyzers.Tests.Internal
{
    /// <summary>
    /// This abstract class provides a configured test harness for C# analyzers,
    /// ensuring that all tests have the necessary dependencies and a consistent
    /// testing environment.
    /// </summary>
    public abstract class Verifier<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public Verifier()
        {
            ReferenceAssemblies = new ReferenceAssemblies(
                "net8.0",
                new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"),
                Path.Combine("ref", "net8.0"));

            TestState.AdditionalReferences.Add(typeof(IResult).Assembly);
        }

        protected DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => new DiagnosticResult(descriptor);

        protected Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
            => RunAsync(source, expected);

        private async Task RunAsync(string source, params DiagnosticResult[] expected)
        {
            TestState.Sources.Clear();
            TestState.Sources.Add(source);
            TestState.ExpectedDiagnostics.Clear();
            TestState.ExpectedDiagnostics.AddRange(expected);
            await RunAsync();
        }
    }
}
