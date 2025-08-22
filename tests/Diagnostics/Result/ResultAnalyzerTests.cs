// <copyright file="ResultAnalyzerTests.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Testing;

using Xunit;

using Zentient.Analyzers.Diagnostics;
using Zentient.Analyzers.Diagnostics.Result;
using Zentient.Analyzers.Tests.Internal;
using Zentient.Analyzers.Tests.Diagnostics.Result;

using static Zentient.Analyzers.Tests.Diagnostics.Result.ResultAnalyzerTestConstants;

namespace Zentient.Analyzers.Tests.Diagnostics.Result
{
    /// <summary>
    /// Complete and sophisticated tests for ResultAnalyzer.
    /// </summary>
    public class ResultAnalyzerTests : Verifier<ResultAnalyzer>
    {
        private const string TypeNamePlaceholder = "MyTestClass";

        private static async Task VerifyAnalyzerAsync(string testCode, DiagnosticResult expected)
        {
            var formattedCode = testCode.Replace(TypeNamePlaceholder, "MyTestClass");
            await VerifyAnalyzerAsync(formattedCode, expected);
        }

        // ZNT0003: MissingMessagesProperty
        [Fact]
        public async Task ReportsDiagnosticWhenMissingMessagesProperty()
        {
            var testCode = ResultAnalyzerTestConstants.MissingMessagesProperty.Replace("MissingMessages", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0003MissingMessagesProperty)
                .WithLocation(4, 21)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        // ZNT0004: MissingErrorsProperty
        [Fact]
        public async Task ReportsDiagnosticWhenMissingErrorsProperty()
        {
            var testCode = ResultAnalyzerTestConstants.MissingErrorsProperty.Replace("MissingErrors", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0004MissingErrorsProperty)
                .WithLocation(4, 21)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        // ZNT0005: MissingValueForGeneric
        [Fact]
        public async Task ReportsDiagnosticWhenMissingValuePropertyForGenericResult()
        {
            var testCode = ResultAnalyzerTestConstants.MissingValueForGeneric.Replace("MissingValue", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0005MissingValueForGeneric)
                .WithLocation(4, 36)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        // ZNT0002: IsSuccess must be derived from Errors
        [Fact]
        public async Task ReportsDiagnosticWhenIsSuccessHasPublicSetter()
        {
            var testCode = ResultAnalyzerTestConstants.IsSuccessWithSetter.Replace("ResultWithSetter", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0002IsSuccessDerivedFromErrors)
                .WithLocation(5, 17)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task ReportsDiagnosticWhenIsSuccessNotDerivedFromErrors()
        {
            var testCode = ResultAnalyzerTestConstants.IsSuccessNotDerivedFromErrors.Replace("ResultWrongIsSuccess", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0002IsSuccessDerivedFromErrors)
                .WithLocation(5, 17)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenIsSuccessDerivedFromErrors()
        {
            var testCode = ResultAnalyzerTestConstants.IsSuccessDerivedFromErrors.Replace("ResultCorrectIsSuccess", TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode);
        }

        // ZNT0010: IsSuccess must be derived from Errors or Code (Envelope)
        [Fact]
        public async Task ReportsDiagnosticWhenEnvelopeIsSuccessHasPublicSetter()
        {
            var testCode = ResultAnalyzerTestConstants.EnvelopeIsSuccessWithSetter.Replace("EnvelopeWithSetter", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode)
                .WithLocation(8, 17)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task ReportsDiagnosticWhenEnvelopeIsSuccessNotDerivedFromErrorsOrCode()
        {
            var testCode = ResultAnalyzerTestConstants.EnvelopeIsSuccessNotDerived.Replace("EnvelopeWrongIsSuccess", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode)
                .WithLocation(8, 17)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenEnvelopeIsSuccessDerivedFromErrorsOrCode()
        {
            var testCode = @"
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Zentient.Abstractions.Codes;
using Zentient.Abstractions.Codes.Definitions;
using Zentient.Abstractions.Common;
using Zentient.Abstractions.Envelopes;
using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Readers;
using Zentient.Abstractions.Relations;
using Zentient.Abstractions.Relations.Definitions;
using Zentient.Abstractions.Results;
    public sealed class MyEnvelope : IEnvelope<ICodeDefinition, IErrorDefinition>
    {
        private IResult _result;
        private MyEnvelope(
            IResult result,
            ICode<ICodeDefinition> code,
            IMetadata metadata)
        {
            _result = result ?? throw new ArgumentNullException(nameof(result));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }
        public bool IsSuccess => !Errors.Any();
        public ICode<ICodeDefinition> Code { get; }
        public IMetadata Metadata { get; }

        public IReadOnlyCollection<string> Messages => 
            _result.Messages ?? Array.Empty<string>();

        public IReadOnlyList<IErrorInfo<IErrorDefinition>> Errors => 
            _result.Errors.ToList();

        ICode<ICodeDefinition> IEnvelope<ICodeDefinition, IErrorDefinition>.Code => Code;

        public static IEnvelope<ICodeDefinition, IErrorDefinition> Create(
            IResult result,
            ICode<ICodeDefinition> code,
            IMetadata metadata)
            => new MyEnvelope(result, code, metadata);
    }
";
            var expected = new DiagnosticResult
            {
            };
            await VerifyAnalyzerAsync(testCode, DiagnosticResult.EmptyDiagnosticResults);
        }

        // ZNT0011: MissingCodeProperty (Envelope)
        [Fact]
        public async Task ReportsDiagnosticWhenEnvelopeMissingCodeProperty()
        {
            var testCode = ResultAnalyzerTestConstants.EnvelopeMissingCodeProperty.Replace("EnvelopeMissingCode", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0011MissingCodeProperty)
                .WithLocation(8, 21)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        // ZNT0012: Value must only exist on success (Envelope<T>)
        [Fact]
        public async Task ReportsDiagnosticWhenValuePropertyHasNonPrivateSetter()
        {
            var testCode = ResultAnalyzerTestConstants.EnvelopeGenericValueWithSetter.Replace("EnvelopeGenericWithSetter", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0012ValueMustOnlyExistOnSuccess)
                .WithLocation(8, 16)
                .WithArguments("Value", TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenValuePropertyHasPrivateSetter()
        {
            var testCode = ResultAnalyzerTestConstants.EnvelopeGenericValueWithPrivateSetter.Replace("EnvelopeGenericPrivateSetter", TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode);
        }

        // ZNT0013: MissingHeadersPropertyForHeaderedEnvelope
        [Fact]
        public async Task ReportsDiagnosticWhenHeaderedEnvelopeMissingHeadersProperty()
        {
            var testCode = ResultAnalyzerTestConstants.HeaderedEnvelopeMissingHeadersProperty.Replace("HeaderedEnvelopeMissingHeaders", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0013MissingHeadersPropertyForHeaderedEnvelope)
                .WithLocation(7, 21)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenHeaderedEnvelopeHasHeadersProperty()
        {
            var testCode = ResultAnalyzerTestConstants.HeaderedEnvelopeWithHeadersProperty.Replace("HeaderedEnvelopeWithHeaders", TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode);
        }

        // ZNT0014: MissingStreamPropertyForStreamableEnvelope
        [Fact]
        public async Task ReportsDiagnosticWhenStreamableEnvelopeMissingStreamProperty()
        {
            var testCode = ResultAnalyzerTestConstants.StreamableEnvelopeMissingStreamProperty.Replace("StreamableEnvelopeMissingStream", TypeNamePlaceholder);
            var expected = Diagnostic(Descriptors.ZNT0014MissingStreamPropertyForStreamableEnvelope)
                .WithLocation(7, 21)
                .WithArguments(TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode, expected);
        }

        [Fact]
        public async Task NoDiagnosticWhenStreamableEnvelopeHasStreamProperty()
        {
            var testCode = ResultAnalyzerTestConstants.FullyCompliantEnvelopeBase.Replace("FullyCompliantEnvelope", TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode);
        }

        // Edge case: All required properties present, no diagnostics
        [Fact]
        public async Task NoDiagnosticWhenAllRequiredPropertiesPresent()
        {
            var testCode = ResultAnalyzerTestConstants.FullyCompliantEnvelopeBase.Replace("FullyCompliantEnvelope", TypeNamePlaceholder);
            await VerifyAnalyzerAsync(testCode);
        }
    }
}
