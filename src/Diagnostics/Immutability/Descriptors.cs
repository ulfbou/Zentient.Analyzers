// <copyright file="Descriptors.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Zentient.Analyzers
{
    internal static partial class Descriptors
    {
        public static readonly DiagnosticDescriptor ZNT1001A_ConcreteTypeMustBeSealed = new(
            id: "ZNT1001A",
            title: "Concrete immutable type must be sealed",
            messageFormat: "Concrete type '{0}' implementing a core immutable abstraction must be declared 'sealed'",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces the immutability and encapsulation of core data contracts by requiring their concrete implementations to be sealed. This prevents unintended modification of object state through inheritance and ensures that creation logic is confined to designated builders or factories, maintaining a consistent and predictable API surface.");

        public static readonly DiagnosticDescriptor ZNT1001B_PropertiesMustBeGetOnly = new(
            id: "ZNT1001B",
            title: "Properties must be get-only",
            messageFormat: "Property '{0}' on type '{1}' must be get-only",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces immutability by preventing external code from altering the state of a result after it has been created.");

        public static readonly DiagnosticDescriptor ZNT1001C_NoPublicConstructors = new(
            id: "ZNT1001C",
            title: "Use a builder or factory instead of a public constructor",
            messageFormat: "Type '{0}' must not contain a public or internal constructor",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Enforces a consistent creation pattern for immutable types, directing developers toward designated builders or static factory methods.");

        public static readonly DiagnosticDescriptor ZNT1001D_MustHaveStaticFabricMethods = new(
            id: "ZNT1001D",
            title: "Type must provide static factory methods",
            messageFormat: "Type '{0}' must provide static factory methods for creation instead of public constructors",
            category: ImmutabilityCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Ensures that types lacking a dedicated builder are created through controlled, static factory methods, promoting immutability and encapsulation.");
    }
}
