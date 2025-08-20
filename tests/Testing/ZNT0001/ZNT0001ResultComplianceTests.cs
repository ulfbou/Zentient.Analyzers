// <copyright file="ZNT0001ResultComplianceTests.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using System.Threading.Tasks;

using Xunit;

using Zentient.Abstractions.Results;
using Zentient.Analyzers.ReferenceImpl.Phase1;
using Zentient.Analyzers.Testing.ZNT0001.ResultCompliance;
using Zentient.Analyzers.Tests.Internal;

namespace Zentient.Analyzers.Tests.ZNT0001.ResultCompliance
{

    /// <summary>
    /// Contains a complete set of tests for the ZNT0001ResultComplianceAnalyzer.
    /// </summary>
    public class ZNT0001ResultComplianceTests : Verifier<ZNT0001ResultComplianceAnalyzer>
    {
        //        // =========================================================================
        //        // Test Cases for ZNT0001: No Settable IsSuccess
        //        // =========================================================================

        /// <summary>
        /// Verifies the analyzer reports a diagnostic when a type implementing IResult has a settable IsSuccess property.
        /// </summary>
        [Fact]
        public async Task ZNT0001IsSuccessWithSetterReportsDiagnostic()
        {
            // This code correctly implements IResult for .NET 8.0 but also includes a setter,
            // which should trigger your ZNT0001 diagnostic.
            var testCode = @"
        using System; // Line 2
        using System.Collections.Generic; // Line 3
        using System.Linq; // Line 4
        using Zentient.Abstractions.Errors; // Line 5
        using Zentient.Abstractions.Errors.Definitions; // Line 6
        using Zentient.Abstractions.Results; // Line 7

        public class MyResult : IResult // Line 9
        { // Line 10
            public IReadOnlyList<string> Messages { get; } = new List<string>(); // Line 11
            public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>(); // Line 12
            public bool IsSuccess { get; set; } // This is Line 13, Col 17
            public string? ErrorMessage => null; // Line 14
        }";

            // Correctly define the expected diagnostic for ZNT0001.
            // The location needs to match the line and column of the 'set' keyword.
            // It's line 12, column 25 in this test code.
            var expectedDiagnostic = Diagnostic(Descriptors.NoSettableIsSuccess)
                .WithLocation(13, 17)
                .WithArguments("MyResult");

            await VerifyAnalyzerAsync(testCode, expectedDiagnostic);
        }

        [Fact]
        public async Task ZNT0001AMissingMessagesReportsDiagnostic()
        {
            var testCode = @"
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using Zentient.Abstractions.Errors;
        using Zentient.Abstractions.Errors.Definitions;
        using Zentient.Abstractions.Results;

        public class MyResult : IResult
        {
            public bool IsSuccess => !Errors.Any();
            public bool IsFailure => !IsSuccess;
            public IEnumerable<IErrorInfo> Errors { get; }
        }
        ";

            var expectedDiagnostics = new[]
            {
                        // Your custom analyzer's diagnostic
                        Diagnostic(Descriptors.MissingMessages)
                            .WithLocation(9, 14)
                            .WithArguments("MyResult"),

                        // The compiler error for the missing interface member
                        DiagnosticResult.CompilerError("CS0535")
                            .WithLocation(9, 25)
                            .WithArguments("MyResult", "Zentient.Abstractions.Results.IResult.Messages"),

                        // The compiler error for the wrong return type on the 'Errors' property
                        DiagnosticResult.CompilerError("CS0738")
                            .WithLocation(9, 25)
                            .WithArguments("MyResult", "Zentient.Abstractions.Results.IResult.Errors", "MyResult.Errors", "System.Collections.Generic.IEnumerable<Zentient.Abstractions.Errors.IErrorInfo<Zentient.Abstractions.Errors.Definitions.IErrorDefinition>>"),

                        // The compiler error for the missing generic type argument
                        DiagnosticResult.CompilerError("CS0305")
                            .WithLocation(13, 24)
                            .WithArguments("Zentient.Abstractions.Errors.IErrorInfo<TErrorDefinition>", "type", "1"),
                    };

            await VerifyAnalyzerAsync(testCode, expectedDiagnostics);
        }

        // =========================================================================
        // Test Cases for ZNT0001B: Missing Errors Property
        // =========================================================================

        /// <summary>
        /// Verifies the analyzer reports a diagnostic for a missing Errors property.
        /// </summary>
        [Fact]
        public async Task ZNT0001BMissingErrorsReportsDiagnostic()
        {
            var testCode = @"
using System;
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Results;

public class MyResult : IResult
{
    public bool IsSuccess => !Errors.Any();
    public bool IsFailure => !IsSuccess;
    public IEnumerable<string> Messages { get; }
}
";

            var expectedDiagnostics = new[]
            {
        // Your custom analyzer's diagnostic
        Diagnostic(Descriptors.MissingErrors)
            .WithLocation(8, 14)
            .WithArguments("MyResult"),

        // The compiler error for the missing interface member
        DiagnosticResult.CompilerError("CS0535")
            .WithLocation(8, 25)
            .WithArguments("MyResult", "Zentient.Abstractions.Results.IResult.Errors"),
        
        // The compiler error for the wrong return type on the 'Messages' property
        DiagnosticResult.CompilerError("CS0738")
            .WithLocation(8, 25)
            .WithArguments("MyResult", "Zentient.Abstractions.Results.IResult.Messages", "MyResult.Messages", "System.Collections.Generic.IReadOnlyList<string>"),

        // The compiler error for 'Errors' not existing
        DiagnosticResult.CompilerError("CS0103")
            .WithLocation(10, 31)
            .WithArguments("Errors")
    };

            await VerifyAnalyzerAsync(testCode, expectedDiagnostics);
        }
    }
}
