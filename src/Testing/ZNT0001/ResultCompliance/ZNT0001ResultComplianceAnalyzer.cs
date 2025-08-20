// <copyright file="ZNT0001ResultComplianceAnalyzer.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Immutable;

namespace Zentient.Analyzers.Testing.ZNT0001.ResultCompliance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ZNT0001ResultComplianceAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.NoSettableIsSuccess, Descriptors.MissingMessages, Descriptors.MissingErrors, Descriptors.MissingValueForGeneric);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(Start);
        }

        private static void Start(CompilationStartAnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        private static void AnalyzeType(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol is not INamedTypeSymbol type) return;
            if (type.TypeKind != TypeKind.Class || type.IsAbstract) return;
            if (!WellKnown.ImplementsIResult(type, ctx.Compilation, out var iresult, out var iresultOfT)) return;

            var members = type.GetMembers();
            var isSuccess = FindProperty(members, WellKnown.IsSuccessProp);

            if (isSuccess is { GetMethod: not null } &&
                isSuccess.SetMethod is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.NoSettableIsSuccess, isSuccess.Locations[0], type.Name));
            }

            var messages = FindProperty(members, WellKnown.MessagesProp);
            if (messages is null || messages.GetMethod is null)
            {
                var location = (messages is not null && messages.Locations.Length > 0)
                    ? messages.Locations[0]
                    : type.Locations[0];
                ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.MissingMessages, location, type.Name));
            }

            var errors = FindProperty(members, WellKnown.ErrorsProp);
            if (errors is null || errors.GetMethod is null)
            {
                var location = (errors is not null && errors.Locations.Length > 0)
                    ? errors.Locations[0]
                    : type.Locations[0];
                ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.MissingErrors, location, type.Name));
            }

            if (iresultOfT is not null)
            {
                var value = FindProperty(members, WellKnown.ValueProp);
                if (value is null || value.GetMethod is null)
                {
                    var location = (value is not null && value.Locations.Length > 0)
                        ? value.Locations[0]
                        : type.Locations[0];
                    ctx.ReportDiagnostic(Diagnostic.Create(Descriptors.MissingValueForGeneric, location, type.Name));
                }
            }
        }

        private static IPropertySymbol? FindProperty(
            ImmutableArray<ISymbol> members,
            string name)
        {
            foreach (var m in members)
            {
                if (m is IPropertySymbol p && p.Name == name)
                    return p;
            }

            return null;
        }
    }
}
