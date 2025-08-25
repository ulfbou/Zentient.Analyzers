// <copyright file="ImmutabilityAnalyzerTests.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis.Testing;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using Zentient.Abstractions.Common;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
using Zentient.Analyzers.Diagnostics;
using Zentient.Analyzers.Diagnostics.Immutability;
using Zentient.Analyzers.Templates;
using Zentient.Analyzers.Tests.Internal;

namespace Zentient.Analyzers.Tests.Diagnostics.Immutability
{
    /// <summary>
    /// Complete and sophisticated tests for ImmutabilityAnalyzer.
    /// </summary>
    public class ImmutabilityAnalyzerTests : Verifier<ImmutabilityAnalyzer>
    {
        // ZNT0006: Concrete type must be sealed
        // In ImmutabilityAnalyzerTests.cs

        [Fact]
        public async Task ReportsDiagnosticWhenTypeIsNotSealed()
        {
            // Arrange
            var testCode = new ResultsTemplateBuilder()
                .AddCompliant("Result")
                .WithMutation(code => code.Replace("sealed class Result", "class NonSealedResult"))
                .Build();

            // Act & Assert
            var expected = Diagnostic(Descriptors.ZNT0006ConcreteTypeMustBeSealed)
                .WithLocation((testCode?.IndexOf("class NonSealedResult") ?? 0) + 6, 14)
                .WithArguments("NonSealedResult");

            await VerifyAnalyzerAsync(testCode, expected);
        }
        [Fact]
        public async Task OldReportsDiagnosticWhenTypeIsNotSealed()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
public class NonSealedResult : IResult
{
    private NonSealedResult() { }
    public static IResult Create() => new NonSealedResult();
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
}
";
            var expected = Diagnostic(Descriptors.ZNT0006ConcreteTypeMustBeSealed)
                .WithLocation(7, 14)
                .WithArguments("NonSealedResult");
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenTypeIsSealed()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
public sealed class SealedResult : IResult
{
    private SealedResult() { }
    public static IResult Create() => new SealedResult();
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        // ZNT0007: Properties must be get-only
        [Fact]
        public async Task ReportsDiagnosticWhenPropertyHasPublicSetter()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
public sealed class ResultWithSetter : IResult
{
    private ResultWithSetter() { }
    public string Name { get; set; }
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
    public static IResult Create() => new ResultWithSetter();
}
";
            var expected = Diagnostic(Descriptors.ZNT0007PropertiesMustBeGetOnly)
                .WithLocation(10, 19)
                .WithArguments("Name", "ResultWithSetter");
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenPropertiesAreGetOnly()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
public sealed class ResultGetOnly : IResult
{
    private ResultGetOnly() { }
    public string Name { get; }
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
    public static IResult Create() => new ResultGetOnly();
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task ReportsDiagnosticWhenConstructorIsPublicOrInternal()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
public sealed class ResultWithPublicCtor : IResult
{
    public ResultWithPublicCtor() { }
    internal ResultWithPublicCtor(int x) { }
    private ResultWithPublicCtor(string s) { }
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
    public static IResult Create() => new ResultWithPublicCtor();
}
";
            var expected1 = Diagnostic(Descriptors.ZNT0008NoPublicOrInternalConstructors)
                .WithLocation(9, 12)
                .WithArguments("ResultWithPublicCtor");
            var expected2 = Diagnostic(Descriptors.ZNT0008NoPublicOrInternalConstructors)
                .WithLocation(10, 14)
                .WithArguments("ResultWithPublicCtor");
            await VerifyAnalyzerAsync(testCode, expected1, expected2);
        }

        [Fact]
        public async Task NoDiagnosticWhenOnlyPrivateConstructors()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
public sealed class ResultPrivateCtor : IResult
{
    private ResultPrivateCtor() { }
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<string> Messages { get; } = new List<string>();
    public IEnumerable<IErrorInfo<IErrorDefinition>> Errors { get; } = new List<IErrorInfo<IErrorDefinition>>();
    public string? ErrorMessage => Errors.FirstOrDefault()?.Message;
    public static IResult Create() => new ResultPrivateCtor();
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        // ZNT0009: Must have static factory method (IValidationContext)
        [Fact]
        public async Task ReportsDiagnosticWhenNoStaticFactoryMethodForIValidationContext()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class NoFactoryValidationContext : IValidationContext
{
    private NoFactoryValidationContext() { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful => throw new System.NotImplementedException();
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
}
";
            var expected = Diagnostic(Descriptors.ZNT0009MustHaveStaticFactoryMethods)
                .WithLocation(7, 21)
                .WithArguments("NoFactoryValidationContext");
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenStaticFactoryMethodReturnsSelf()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class CompliantValidationContext : IValidationContext
{
    private CompliantValidationContext(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent) { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful => throw new System.NotImplementedException();
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
    public static CompliantValidationContext Create(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent)
        => new CompliantValidationContext(def, input, isSuccess, meta, parent);
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task NoDiagnosticWhenStaticFactoryMethodReturnsInterface()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class CompliantValidationContext : IValidationContext
{
    private CompliantValidationContext(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent) { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful => throw new System.NotImplementedException();
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
    public static IValidationContext Create(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent)
        => new CompliantValidationContext(def, input, isSuccess, meta, parent);
}
";
            await VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task ReportsDiagnosticWhenStaticFactoryMethodIsPrivate()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class InvalidFactoryValidationContext : IValidationContext
{
    private InvalidFactoryValidationContext(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent) { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful => throw new System.NotImplementedException();
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
    private static IValidationContext Create(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent)
        => new InvalidFactoryValidationContext(def, input, isSuccess, meta, parent);
}
";
            var expected = Diagnostic(Descriptors.ZNT0009MustHaveStaticFactoryMethods)
                .WithLocation(7, 21)
                .WithArguments("InvalidFactoryValidationContext");
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task ReportsDiagnosticWhenStaticFactoryMethodReturnsWrongType()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class WrongReturnValidationContext : IValidationContext
{
    private WrongReturnValidationContext(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent) { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful => throw new System.NotImplementedException();
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
    public static string Create(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent)
        => ""invalid"";
}
";
            var expected = Diagnostic(Descriptors.ZNT0009MustHaveStaticFactoryMethods)
                .WithLocation(7, 21)
                .WithArguments("WrongReturnValidationContext");
            await VerifyAnalyzerAsync(testCode, expected);
        }

        // Edge case: Multiple static factory methods, only one valid
        [Fact]
        public async Task NoDiagnosticWhenMultipleStaticFactoryMethodsAtLeastOneValid()
        {
            var testCode = @"
using System.Collections.Generic;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Validation;
using Zentient.Abstractions.Validation.Definitions;
public sealed class MultiFactoryValidationContext : IValidationContext
{
    private MultiFactoryValidationContext(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent) { }
    public IValidationDefinition ValidatorType { get; }
    public object? Input { get; }
    public bool IsSuccessful => throw new System.NotImplementedException();
    public IMetadata Metadata { get; }
    public IValidationContext? Parent { get; }
    public static string Create1() => ""invalid"";
    public static IValidationContext Create2(IValidationDefinition def, object? input, bool isSuccess, IMetadata meta, IValidationContext? parent)
        => new MultiFactoryValidationContext(def, input, isSuccess, meta, parent);
}
";
            await VerifyAnalyzerAsync(testCode);
        }
    }
}
