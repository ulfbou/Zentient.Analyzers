// <copyright file="ZNT1001ImmutabilityAnalyzer.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Immutable;
using System.Linq;

using Zentient.Abstractions.Errors;
using Zentient.Abstractions.Errors.Definitions;
using Zentient.Abstractions.Results;
using Zentient.Analyzers;
using Zentient.Analyzers.Internal;

namespace Zentient.Analyzers.Diagnostics.Immutability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ZNT1001ImmutabilityAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            Descriptors.ZNT1001A_ConcreteTypeMustBeSealed,
            Descriptors.ZNT1001B_PropertiesMustBeGetOnly,
            Descriptors.ZNT1001C_NoPublicConstructors,
            Descriptors.ZNT1001D_MustHaveStaticFabricMethods
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

            // Check if the type is a target for our immutability rules
            if (!WellKnown.ImplementsAnyOf(type, WellKnown.ImmutableAbstractions.ToArray()))
            {
                return;
            }

            // ZNT1001A: Concrete type must be sealed.
            if (!type.IsSealed)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.ZNT1001A_ConcreteTypeMustBeSealed,
                    type.Locations[0],
                    type.Name));
            }

            // ZNT1001B: Properties must be get-only.
            foreach (var prop in type.GetMembers().OfType<IPropertySymbol>())
            {
                if (prop.SetMethod is not null && (prop.SetMethod.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal))
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZNT1001B_PropertiesMustBeGetOnly,
                        prop.Locations[0],
                        prop.Name,
                        type.Name));
                }
            }

            // ZNT1001C: No public or internal constructors.
            foreach (var ctor in type.Constructors)
            {
                if (ctor.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZNT1001C_NoPublicConstructors,
                        ctor.Locations[0],
                        type.Name));
                }
            }

            // ZNT1001D: Must have a static factory method (only for IValidationContext).
            if (type.AllInterfaces.Any(i => i.ToDisplayString().StartsWith(WellKnown.IValidationContext, StringComparison.Ordinal)))
            {
                var hasFactoryMethod = type.GetMembers().OfType<IMethodSymbol>()
                    .Any(m => m.IsStatic && m.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal && m.ReturnType is not null && type.IsAssignableTo(m.ReturnType));

                if (!hasFactoryMethod)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZNT1001D_MustHaveStaticFabricMethods,
                        type.Locations[0],
                        type.Name));
                }
            }
        }
    }
}
