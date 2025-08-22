// <copyright file="ImmutabilityAnalyzer.cs" author="Ulf Bourelius">
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

namespace Zentient.Analyzers.Diagnostics.Immutability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ImmutabilityAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            Descriptors.ZNT0006ConcreteTypeMustBeSealed,
            Descriptors.ZNT0007PropertiesMustBeGetOnly,
            Descriptors.ZNT0008NoPublicOrInternalConstructors,
            Descriptors.ZNT0009MustHaveStaticFactoryMethods
        );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        private static void AnalyzeType(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
            {
                return;
            }

            // The analyzer should only apply to non-abstract immutable types.
            if (!type.IsImmutableAbstraction() || type.IsAbstract)
            {
                return;
            }

            // ZNT0006: Concrete type must be sealed
            if (!type.IsSealed)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.ZNT0006ConcreteTypeMustBeSealed,
                    type.Locations[0],
                    type.Name));
            }

            // ZNT0007: Properties must be get-only
            AnalyzeGetOnlyProperties(ctx, type);

            // ZNT0008: No public or internal constructors
            AnalyzeConstructors(ctx, type);

            // ZNT0009: Must have static factory method for IValidationContext
            if (type.Implements(WellKnown.IValidationContext))
            {
                AnalyzeValidationContextFactoryMethod(ctx, type);
            }
        }

        private static void AnalyzeGetOnlyProperties(SymbolAnalysisContext ctx, INamedTypeSymbol type)
        {
            var publicPropertiesWithSetter = type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p =>
                    p.DeclaredAccessibility == Accessibility.Public &&
                    p.SetMethod != null &&
                    p.SetMethod.DeclaredAccessibility != Accessibility.Private)
                .ToList();

            foreach (var property in publicPropertiesWithSetter)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.ZNT0007PropertiesMustBeGetOnly,
                    property.Locations[0],
                    property.Name,
                    type.Name));
            }
        }

        private static void AnalyzeConstructors(SymbolAnalysisContext ctx, INamedTypeSymbol type)
        {
            var constructors = type.Constructors;
            foreach (var ctor in constructors)
            {
                if (ctor.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.ZNT0008NoPublicOrInternalConstructors,
                        ctor.Locations[0],
                        type.Name));
                }
            }
        }

        private static void AnalyzeValidationContextFactoryMethod(SymbolAnalysisContext ctx, INamedTypeSymbol type)
        {
            // Check if the type has any public static methods
            var hasPublicStaticMethod = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Any(m => m.IsStatic && m.DeclaredAccessibility == Accessibility.Public);

            if (!hasPublicStaticMethod)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.ZNT0009MustHaveStaticFactoryMethods,
                    type.Locations[0],
                    type.Name));
                return;
            }

            // Now, check if any of these public static methods are valid factory methods
            var validationContextType = ctx.Compilation.GetTypeByMetadataName(WellKnown.IValidationContext);

            if (validationContextType is null)
            {
                // If the IValidationContext type is not found, we cannot perform this check.
                // It's a valid scenario, so we simply return without reporting a diagnostic.
                return;
            }

            var hasValidFactoryMethod = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Any(m => m.IsStatic &&
                          m.DeclaredAccessibility == Accessibility.Public &&
                          (m.ReturnType.Equals(type, SymbolEqualityComparer.Default) ||
                           m.ReturnType.IsAssignableTo(validationContextType)));

            if (!hasValidFactoryMethod)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.ZNT0009MustHaveStaticFactoryMethods,
                    type.Locations[0],
                    type.Name));
            }
        }
    }
}
