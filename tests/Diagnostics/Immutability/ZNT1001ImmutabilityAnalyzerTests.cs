// <copyright file="ZNT1001ImmutabilityAnalyzerTests.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis.Testing;

using System.Threading.Tasks;

using Xunit;

using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
using Zentient.Analyzers.Diagnostics.Immutability;
using Zentient.Analyzers.Tests.Internal;

namespace Zentient.Analyzers.Tests.Diagnostics.Immutability
{
    /// <summary>
    /// Contains a complete set of tests for the ZNT1001ImmutabilityAnalyzer.
    /// </summary>
    public class ImmutabilityAnalyzerTests : Verifier<ImmutabilityAnalyzer>
    {
        // Case 1: ZNT1001A - Concrete Type Must Be Sealed
        [Fact]
        public async Task ConcreteTypeMustBeSealedNegativeReportsDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
public class NonCompliantResult : IResult {
    public NonCompliantResult() { }
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
}
";

            var expected1 = Diagnostic(Descriptors.ZNT1001A_ConcreteTypeMustBeSealed)
                .WithLocation(6, 14) // Update line number to 6
                .WithArguments("NonCompliantResult");

            //// Add the ZNT1001C diagnostic to the expected list since it's also being reported
            var expected2 = Diagnostic(Descriptors.ZNT1001C_NoPublicConstructors)
                .WithLocation(7, 12) // The public constructor is on line 7
                .WithArguments("NonCompliantResult");

            await VerifyAnalyzerAsync(testCode, expected1, expected2);
        }

        // Case 2: ZNT1001B - Properties Must Be Get-Only
        [Fact]
        public async Task PropertiesMustBeGetOnlyNegativeReportsDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
public sealed class NonCompliantResult : IResult {
    private NonCompliantResult() { }
    public IReadOnlyList<string> Messages { get; set; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; set; } = new List<IErrorInfo<IErrorDefinition>>();
}
";
            // Expecting two diagnostics for the properties that are not get-only
            var expected1 = Diagnostic(Descriptors.ZNT1001B_PropertiesMustBeGetOnly)
                .WithLocation(8, 34)
                .WithArguments("Messages", "NonCompliantResult");

            var expected2 = Diagnostic(Descriptors.ZNT1001B_PropertiesMustBeGetOnly)
                .WithLocation(9, 54)
                .WithArguments("Errors", "NonCompliantResult");

            await VerifyAnalyzerAsync(testCode, expected1, expected2);
        }

        // Case 3: ZNT1001C - No Public Constructors
        [Fact]
        public async Task NoPublicConstructorsNegativeReportsDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
public sealed class NonCompliantResult : IResult {
    public NonCompliantResult() { }
    internal NonCompliantResult(IReadOnlyList<string> messages) { Messages = messages; }
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
}
";
            // Expect two ZNT1001C diagnostics, one for each non-compliant constructor
            var expected1 = Diagnostic(Descriptors.ZNT1001C_NoPublicConstructors)
                .WithLocation(7, 12)
                .WithArguments("NonCompliantResult");
            var expected2 = Diagnostic(Descriptors.ZNT1001C_NoPublicConstructors)
                .WithLocation(8, 14)
                .WithArguments("NonCompliantResult");

            await VerifyAnalyzerAsync(testCode, expected1, expected2);
        }

        // Case 4: ZNT1001D - Must Have Static Factory Methods
        [Fact]
        public async Task MustHaveStaticFactoryMethodPositiveNoDiagnostic()
        {
            var testCode = @"
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class CompliantValidationContext : Zentient.Abstractions.Validation.IValidationContext
{
    private CompliantValidationContext(IValidationDefinition validatorType, object? input, bool isSuccessful, IMetadata metadata, IValidationContext? parent) 
    {
        ValidatorType = validatorType;
        Input = input;
        IsSuccessful = isSuccessful;
        Metadata = metadata;
        Parent = parent;
    }

    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful { get; }
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }

    public static CompliantValidationContext Create(IValidationDefinition validatorType, object? input, bool isSuccessful, IMetadata metadata, IValidationContext? parent)
    {
        return new CompliantValidationContext(validatorType, input, isSuccessful, metadata, parent);
    }
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task MustHaveStaticFactoryMethodNegativeReportsDiagnostic()
        {
            var testCode = @"
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class NonCompliantValidationContext : Zentient.Abstractions.Validation.IValidationContext
{
    private NonCompliantValidationContext() { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful { get; }
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
}
";
            var expected = Diagnostic(Descriptors.ZNT1001D_MustHaveStaticFabricMethods)
                .WithLocation(5, 21)
                .WithArguments("NonCompliantValidationContext");
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task MustHaveStaticFactoryMethodReturnsInterfacePositiveNoDiagnostic()
        {
            var testCode = @"
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class CompliantValidationContext : Zentient.Abstractions.Validation.IValidationContext
{
    private CompliantValidationContext(IValidationDefinition validatorType, object? input, bool isSuccessful, IMetadata metadata, IValidationContext? parent) 
    {
        ValidatorType = validatorType;
        Input = input;
        IsSuccessful = isSuccessful;
        Metadata = metadata;
        Parent = parent;
    }

    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful { get; }
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }

    public static IValidationContext Create(IValidationDefinition validatorType, object? input, bool isSuccessful, IMetadata metadata, IValidationContext? parent)
    {
        return new CompliantValidationContext(validatorType, input, isSuccessful, metadata, parent);
    }
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task IsSuccessPropertyWithSetterReportsDiagnostic()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Results;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
public sealed class NonCompliantResult : IResult {
    private NonCompliantResult() { }
    public bool IsSuccess { get; set; }
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
}
";

            var expected1 = Diagnostic(Descriptors.ZNT1001B_PropertiesMustBeGetOnly)
                .WithLocation(8, 17)
                .WithArguments("IsSuccess", "NonCompliantResult");

            var expected2 = Diagnostic(Descriptors.ZNT1002A_NoIsSuccessSetter)
                .WithLocation(8, 17)
                .WithArguments("NonCompliantResult");

            var expected3 = Diagnostic(Descriptors.ZNT1002B_IsSuccessDerivedFromErrors)
                .WithLocation(8, 17)
                .WithArguments("NonCompliantResult");

            await VerifyAnalyzerAsync(testCode, expected1, expected2, expected3);
        }
    }
}
