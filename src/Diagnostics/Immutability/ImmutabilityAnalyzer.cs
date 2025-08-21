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
            Descriptors.ZNT1001A_ConcreteTypeMustBeSealed,
            Descriptors.ZNT1001B_PropertiesMustBeGetOnly,
            Descriptors.ZNT1001C_NoPublicConstructors,
            Descriptors.ZNT1001D_MustHaveStaticFabricMethods,
            Descriptors.ZNT1002A_NoIsSuccessSetter,
            Descriptors.ZNT1002B_IsSuccessDerivedFromErrors
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

            // Check if the type is a target for our immutability rules.
            // This includes both general immutable abstractions and the specific IValidationContext.
            if (!type.IsImmutableAbstraction())
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
                if (prop.SetMethod is not null && (prop.SetMethod.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
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
            if (type.IsValidationContext())
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

            // ZNT1002: IsSuccess property checks (only for types implementing IResult).
            if (type.IsResultAbstraction())
            {
                var isSuccessProperty = type.GetMembers("IsSuccess").OfType<IPropertySymbol>().FirstOrDefault();
                var errorsProperty = type.GetMembers("Errors").OfType<IPropertySymbol>().FirstOrDefault();

                if (isSuccessProperty is not null && errorsProperty is not null)
                {
                    // ZNT1002A: The IsSuccess property must not have a setter.
                    if (isSuccessProperty.SetMethod is not null && (isSuccessProperty.SetMethod.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal))
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            Descriptors.ZNT1002A_NoIsSuccessSetter,
                            isSuccessProperty.Locations[0],
                            type.Name));
                    }

                    // ZNT1002B: The IsSuccess property must be derived from the Errors collection.
                    var hasBackingField = isSuccessProperty.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");

                    if (hasBackingField)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                           Descriptors.ZNT1002B_IsSuccessDerivedFromErrors,
                           isSuccessProperty.Locations[0],
                           type.Name));
                    }
                    else if (isSuccessProperty.GetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is AccessorDeclarationSyntax accessor)
                    {
                        // A more robust check: does the getter reference the Errors property?
                        SyntaxNode? syntax = accessor.Body ?? (SyntaxNode?)accessor.ExpressionBody;

                        if (syntax is null || !syntax.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(id => id.Identifier.Text == errorsProperty.Name))
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(
                                Descriptors.ZNT1002B_IsSuccessDerivedFromErrors,
                                isSuccessProperty.Locations[0],
                                type.Name));
                        }
                    }
                }
            }
        }
    }
}
