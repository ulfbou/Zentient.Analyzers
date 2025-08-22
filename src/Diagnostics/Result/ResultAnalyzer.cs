// <copyright file="ResultAnalyzer.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System;
using System.Collections.Immutable;
using System.Linq;

using Zentient.Analyzers.Internal;

namespace Zentient.Analyzers.Diagnostics.Result
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ResultAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            Descriptors.ZNT0002IsSuccessDerivedFromErrors,
            Descriptors.ZNT0003MissingMessagesProperty,
            Descriptors.ZNT0004MissingErrorsProperty,
            Descriptors.ZNT0005MissingValueForGeneric,
            Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode,
            Descriptors.ZNT0011MissingCodeProperty,
            Descriptors.ZNT0012ValueMustOnlyExistOnSuccess,
            Descriptors.ZNT0013MissingHeadersPropertyForHeaderedEnvelope,
            Descriptors.ZNT0014MissingStreamPropertyForStreamableEnvelope
        );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        private static void AnalyzeType(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class || type.IsAbstract)
            {
                return;
            }

            // Check if the type is a Result or Envelope abstraction using precise checks.
            var isResult = type.Implements(WellKnown.IResult) || type.Implements(WellKnown.IResultGeneric);
            var isEnvelope = type.Implements(WellKnown.IEnvelope) || type.Implements(WellKnown.IEnvelopeGeneric);

            if (!isResult && !isEnvelope)
            {
                return;
            }

            AnalyzeRequiredProperties(ctx, type);
            AnalyzeIsSuccessDerivation(ctx, type, isResult, isEnvelope);
            AnalyzeEnvelopeSpecificProperties(ctx, type, isEnvelope);
        }

        private static void AnalyzeRequiredProperties(SymbolAnalysisContext ctx, INamedTypeSymbol type)
        {
            // ZNT0003: MissingMessagesProperty
            if (!type.GetMembers("Messages").OfType<IPropertySymbol>().Any())
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0003MissingMessagesProperty, type.Locations[0], type.Name));
            }

            // ZNT0004: MissingErrorsProperty
            if (!type.GetMembers("Errors").OfType<IPropertySymbol>().Any())
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0004MissingErrorsProperty, type.Locations[0], type.Name));
            }

            // ZNT0005: MissingValueForGeneric
            if (type.Implements(WellKnown.IResultGeneric) || type.Implements(WellKnown.IEnvelopeGeneric))
            {
                if (!type.GetMembers("Value").OfType<IPropertySymbol>().Any())
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0005MissingValueForGeneric, type.Locations[0], type.Name));
                }
            }
        }

        private static void AnalyzeIsSuccessDerivation(SymbolAnalysisContext ctx, INamedTypeSymbol type, bool isResult, bool isEnvelope)
        {
            var isSuccessProperty = type.GetMembers("IsSuccess").OfType<IPropertySymbol>().FirstOrDefault();
            if (isSuccessProperty is null)
            {
                return;
            }

            // ZNT0002/ZNT0010: Check for an auto-implemented property by looking for a setter.
            if (isSuccessProperty.SetMethod is not null && isSuccessProperty.SetMethod.DeclaredAccessibility == Accessibility.Public)
            {
                if (isResult)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0002IsSuccessDerivedFromErrors, isSuccessProperty.Locations[0], type.Name));
                }
                else if (isEnvelope)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode, isSuccessProperty.Locations[0], type.Name));
                }
            }

            // Check if the property is a computed property (has a body or expression).
            if (isSuccessProperty.GetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is AccessorDeclarationSyntax accessor)
            {
                SyntaxNode? body = accessor.Body ?? (SyntaxNode?)accessor.ExpressionBody;
                if (body is not null)
                {
                    // For `IResult`, we must reference `Errors`.
                    if (isResult)
                    {
                        if (!body.ToString().Contains("Errors"))
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0002IsSuccessDerivedFromErrors, isSuccessProperty.Locations[0], type.Name));
                        }
                    }
                    // For `IEnvelope`, we must reference `Errors` OR `Code`.
                    else if (isEnvelope)
                    {
                        var bodyString = body.ToString();
                        bool isDerived = bodyString.Contains("Errors") || bodyString.Contains("Code");
                        if (!isDerived)
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0010IsSuccessDerivedFromErrorsOrCode, isSuccessProperty.Locations[0], type.Name));
                        }
                    }
                }
            }
        }

        private static void AnalyzeEnvelopeSpecificProperties(SymbolAnalysisContext ctx, INamedTypeSymbol type, bool isEnvelope)
        {
            if (!isEnvelope)
            {
                return;
            }

            // ZNT0011: MissingCodeProperty
            if (!type.GetMembers("Code").OfType<IPropertySymbol>().Any())
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0011MissingCodeProperty, type.Locations[0], type.Name));
            }

            // ZNT0012: ValueMustOnlyExistOnSuccess
            // This check is a simplification. The full data flow analysis is complex and may not be necessary.
            // This change instead flags if a `Value` property has a public or internal setter.
            if (type.Implements(WellKnown.IEnvelopeGeneric))
            {
                var valueProperty = type.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault();
                if (valueProperty?.SetMethod is not null && valueProperty.SetMethod.DeclaredAccessibility != Accessibility.Private)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0012ValueMustOnlyExistOnSuccess, valueProperty.Locations[0], type.Name));
                }
            }

            // ZNT0013: MissingHeadersPropertyForHeaderedEnvelope
            if (type.Implements(WellKnown.IHeaderedEnvelope))
            {
                if (!type.GetMembers("Headers").OfType<IPropertySymbol>().Any())
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0013MissingHeadersPropertyForHeaderedEnvelope, type.Locations[0], type.Name));
                }
            }

            // ZNT0014: MissingStreamPropertyForStreamableEnvelope
            if (type.Implements(WellKnown.IStreamableEnvelope))
            {
                if (!type.GetMembers("Stream").OfType<IPropertySymbol>().Any())
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.ZNT0014MissingStreamPropertyForStreamableEnvelope, type.Locations[0], type.Name));
                }
            }
        }
    }
}
