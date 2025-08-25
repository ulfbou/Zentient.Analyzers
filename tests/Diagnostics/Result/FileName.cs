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
    public class ResultAnalyzerTests2 : Verifier<ResultAnalyzer>
    {
        private const string TypeNamePlaceholder = "MyTestClass";
        private const string TypeName = "MyTestClass";

        private async Task VerifyAnalyzerAsync(string testCode, DiagnosticResult expected)
        {
            var formattedCode = testCode.Replace(TypeNamePlaceholder, "MyTestClass");
            await base.VerifyAnalyzerAsync(formattedCode, expected);
        }

        private static string Format(string template) => template.Replace("{0}", TypeNamePlaceholder);

        [Fact]
        public async Task NoDiagnosticForFullyCompliantEnvelope()
        {
            var testCode = Format(ResultAnalyzerTestConstants.FullyCompliantEnvelopeBase);
            await VerifyAnalyzerAsync(testCode);
        }

        // ZNT0005: MissingValueForGeneric
        [Fact]
        public async Task ReportsDiagnosticWhenMissingValuePropertyForGenericResult()
        {
            const string InvalidValueName = "InvalidValueName";
            // The test code is created with an incorrect property name,
            // which should trigger the diagnostic.
            var testCode = Format(ResultAnalyzerTestConstants.ResultWithValue).Replace("Value", InvalidValueName);

            // The expected diagnostic argument should be the type name of the class being analyzed.
            var expected = Diagnostic(Descriptors.ZNT0005MissingValueForGeneric)
                .WithLocation(4, 36)
                .WithArguments(TypeNamePlaceholder); // The type that is missing the 'Value' property.

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
        //// --- No Diagnostic: All compliant envelope scenarios in one test ---
        //[Fact]
        //public async Task NoDiagnosticForFullyCompliantEnvelopeVariants()
        //{
        //    var compliantCodes = new[]
        //    {
        //        Format(FullyCompliantEnvelopeBase),
        //        Format(HeaderedEnvelopeWithHeadersProperty),
        //        Format(StreamableEnvelopeWithStreamProperty),
        //        Format(EnvelopeGenericValueWithPrivateSetter),
        //        Format(EnvelopeIsSuccessDerived),
        //    };

        //    foreach (var code in compliantCodes)
        //    {
        //        try
        //        {
        //            await VerifyAnalyzerAsync(code);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            System.Console.WriteLine("Error with Code: {0}", code);
        //            throw;
        //        }
        //    }
        //}

        //// ZNT0003: MissingMessagesProperty
        //[Fact]
        //public async Task ReportsDiagnosticWhenMissingMessagesProperty()
        //{
        //    var testCode = Format(MissingMessagesProperty);
        //    var expected = Diagnostic(Descriptors.ZNT0003MissingMessagesProperty)
        //        .WithLocation(4, 21)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0004: MissingErrorsProperty
        //[Fact]
        //public async Task ReportsDiagnosticWhenMissingErrorsProperty()
        //{
        //    var testCode = Format(MissingErrorsProperty);
        //    var expected = Diagnostic(Descriptors.ZNT0004MissingErrorsProperty)
        //        .WithLocation(4, 21)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0005: MissingValueForGeneric
        //[Fact]
        //public async Task ReportsDiagnosticWhenMissingValuePropertyForGenericResult()
        //{
        //    var testCode = Format(MissingValueForGeneric);
        //    var expected = Diagnostic(Descriptors.ZNT0005MissingValueForGeneric)
        //        .WithLocation(4, 36)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0002: IsSuccess must be derived from Errors
        //[Fact]
        //public async Task ReportsDiagnosticWhenIsSuccessHasPublicSetter()
        //{
        //    var testCode = Format(IsSuccessWithSetter);
        //    var expected = Diagnostic(Descriptors.ZNT0002IsSuccessDerivedFromErrors)
        //        .WithLocation(5, 17)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //[Fact]
        //public async Task ReportsDiagnosticWhenIsSuccessNotDerivedFromErrors()
        //{
        //    var testCode = Format(IsSuccessNotDerivedFromErrors);
        //    var expected = Diagnostic(Descriptors.ZNT0002IsSuccessDerivedFromErrors)
        //        .WithLocation(5, 17)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0010: IsSuccess must be derived from Errors or Code (Envelope)
        //[Fact]
        //public async Task ReportsDiagnosticWhenEnvelopeIsSuccessHasPublicSetter()
        //{
        //    var testCode = Format(EnvelopeIsSuccessWithSetter);
        //    var expected = Diagnostic(Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode)
        //        .WithLocation(8, 17)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //[Fact]
        //public async Task ReportsDiagnosticWhenEnvelopeIsSuccessNotDerivedFromErrorsOrCode()
        //{
        //    var testCode = Format(EnvelopeIsSuccessNotDerived);
        //    var expected = Diagnostic(Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode)
        //        .WithLocation(8, 17)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0011: MissingCodeProperty (Envelope)
        //[Fact]
        //public async Task ReportsDiagnosticWhenEnvelopeMissingCodeProperty()
        //{
        //    var testCode = Format(EnvelopeMissingCodeProperty);
        //    var expected = Diagnostic(Descriptors.ZNT0011MissingCodeProperty)
        //        .WithLocation(8, 21)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0012: Value must only exist on success (Envelope<T>)
        //[Fact]
        //public async Task ReportsDiagnosticWhenValuePropertyHasNonPrivateSetter()
        //{
        //    var testCode = Format(EnvelopeGenericValueWithSetter);
        //    var expected = Diagnostic(Descriptors.ZNT0012ValueMustOnlyExistOnSuccess)
        //        .WithLocation(8, 16)
        //        .WithArguments("Value", TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0013: MissingHeadersPropertyForHeaderedEnvelope
        //[Fact]
        //public async Task ReportsDiagnosticWhenHeaderedEnvelopeMissingHeadersProperty()
        //{
        //    var testCode = Format(HeaderedEnvelopeMissingHeadersProperty);
        //    var expected = Diagnostic(Descriptors.ZNT0013MissingHeadersPropertyForHeaderedEnvelope)
        //        .WithLocation(7, 21)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}

        //// ZNT0014: MissingStreamPropertyForStreamableEnvelope
        //[Fact]
        //public async Task ReportsDiagnosticWhenStreamableEnvelopeMissingStreamProperty()
        //{
        //    var testCode = Format(StreamableEnvelopeMissingStreamProperty);
        //    var expected = Diagnostic(Descriptors.ZNT0014MissingStreamPropertyForStreamableEnvelope)
        //        .WithLocation(7, 21)
        //        .WithArguments(TypeName);
        //    await VerifyAnalyzerAsync(testCode, expected);
        //}
    }
}
